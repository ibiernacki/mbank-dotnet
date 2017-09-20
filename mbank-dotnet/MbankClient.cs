using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ib.mbank.Serialization;

namespace ib.mbank
{
    public class MbankClient : IMbankClient
    {
        public IRestClient Client { get; private set; }
        public static readonly string DefaultBaseAddress = "https://online.mbank.pl/pl";
        private static string _tokenParameterName = "X-Request-Verification-Token";
        private static string _tabIdParameterName = "X-Tab-Id";

        public MbankClient()
        {
            Client = new RestClient(DefaultBaseAddress)
            {
                CookieContainer = new CookieContainer()
            };
        }

        public async Task<IMbankResponse<AccountInfo>> GetAccountInfo() => await GetAccountInfo(default(CancellationToken));
        public async Task<IMbankResponse<AccountInfo>> GetAccountInfo(CancellationToken cancellationToken)
        {
            if (Client.DefaultParameters.All(p => p.Name != _tokenParameterName) || Client.DefaultParameters.All(p => p.Name != _tabIdParameterName))
            {
                throw new InvalidOperationException("You must Login before using this method");
            }
            var accountsListRequest = new RestRequest("/Accounts/Accounts/List", Method.POST);
            accountsListRequest.AddParameter("X-Requested-With", "XMLHttpRequest", ParameterType.HttpHeader);

            var accountsListResponse = await Client.ExecuteTaskAsync(accountsListRequest, cancellationToken);

            if (accountsListResponse.StatusCode != HttpStatusCode.OK)
            {
                return MbankResponse<AccountInfo>.Failed(accountsListResponse);
            }

            var jobject = JsonConvert.DeserializeObject<JObject>(accountsListResponse.Content);
            var accountInfo = jobject["properties"].ToObject<AccountInfo>();
            accountInfo.Name = jobject["name"].Value<string>();
            return new MbankResponse<AccountInfo>(accountsListResponse, true, accountInfo);
        }

        public async Task<IMbankResponse<IList<Transaction>>> GetTransactions() => await GetTransactions(default(CancellationToken));
        public async Task<IMbankResponse<IList<Transaction>>> GetTransactions(CancellationToken cancellationToken)
        {
            if (Client.DefaultParameters.All(p => p.Name != _tokenParameterName) || Client.DefaultParameters.All(p => p.Name != _tabIdParameterName))
            {
                throw new InvalidOperationException("You must Login before using this method");
            }

            var transactionHistoryRequest = new RestRequest("/Pfm/TransactionHistory", Method.GET);
            var transactionHistoryResponse = await Client.ExecuteTaskAsync(transactionHistoryRequest, cancellationToken);

            if (transactionHistoryResponse.StatusCode != HttpStatusCode.OK)
            {
                return MbankResponse<IList<Transaction>>.Failed(transactionHistoryResponse);
            }

            var transactionHistoryHtmlDocument = new HtmlDocument();
            transactionHistoryHtmlDocument.LoadHtml(transactionHistoryResponse.Content);
            var nodes = transactionHistoryHtmlDocument.DocumentNode.SelectNodes("//ul[@class=\"content-list-body\"]/li");

            var transactions = nodes.Select(node => new Transaction()
            {
                Id = node.GetAttributeValue("data-id", null),
                Type = HttpUtility.HtmlDecode(node.SelectSingleNode("header/div[@class=\"column type\"]")?.InnerText?.Trim('\r', '\n', ' ')),
                Date = JToken.FromObject(node.GetAttributeValue("data-timestamp", null)).ToObject<DateTime>(),
                Title = HttpUtility.HtmlDecode( node.SelectSingleNode("header/div[@class=\"column description\"]/span/span/@data-original-title")?.InnerText),
                Category = HttpUtility.HtmlDecode(node.SelectSingleNode("header/div[@class=\"column category\"]/div[1]/span")?.InnerText),
                Amount = double.Parse(node.GetAttributeValue("data-amount", null).Replace(',', '.'), CultureInfo.InvariantCulture),
                Currency = node.GetAttributeValue("data-currency", null)
            });

            return new MbankResponse<IList<Transaction>>(transactionHistoryResponse, true, new List<Transaction>(transactions));
        }

        public async Task<IMbankResponse<LoginInfo>> Login(string login, string password, AccountType accountType) => await Login(login, password, accountType, default(CancellationToken));
        public async Task<IMbankResponse<LoginInfo>> Login(string login, string password, AccountType accountType, CancellationToken cancellationToken)
        {
            var loginRequest = new RestRequest("/Account/JsonLogin", Method.POST);
            loginRequest.AddParameter("UserName", login, ParameterType.QueryString);
            loginRequest.AddParameter("Password", password, ParameterType.QueryString);
            loginRequest.AddParameter("Seed", "", ParameterType.QueryString);
            loginRequest.AddParameter("Lang", "", ParameterType.QueryString);
            var loginResponse = await Client.ExecuteTaskAsync(loginRequest, cancellationToken);

            if (loginResponse.StatusCode != HttpStatusCode.OK)
                return MbankResponse<LoginInfo>.Failed(loginResponse);

            var loginInfo = JsonConvert.DeserializeObject<LoginInfo>(loginResponse.Content,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            if(!loginInfo.Successful)
                return new MbankResponse<LoginInfo>(loginResponse, false, loginInfo);

            Client.BaseUrl = new Uri($"{Client.BaseUrl.GetLeftPart(UriPartial.Scheme | UriPartial.Authority)}{loginInfo.RedirectUrl}");

            var tokenRequest = new RestRequest();
            var tokenResponse = await Client.ExecuteTaskAsync(tokenRequest, cancellationToken);

            if (tokenResponse.StatusCode != HttpStatusCode.OK)
                return MbankResponse<LoginInfo>.Failed(tokenResponse);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(tokenResponse.Content);
            var tokenNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='__AjaxRequestVerificationToken']");
            var token = tokenNode.GetAttributeValue("content", null);

            loginInfo.Token = token;

            Client.AddDefaultParameter("X-Request-Verification-Token", token, ParameterType.HttpHeader);
            Client.AddDefaultParameter("X-Tab-Id", loginInfo.TabId, ParameterType.HttpHeader);


            var activateAccountRequest = new RestRequest("/LoginMain/Account/JsonActivateProfile", Method.POST);
            switch (accountType)
            {
                case AccountType.Individual:
                    activateAccountRequest.AddParameter("profileCode", "I", ParameterType.QueryString);
                    break;
                case AccountType.Business:
                    activateAccountRequest.AddParameter("profileCode", "B", ParameterType.QueryString);
                    break;
                default:
                    throw new NotImplementedException();
            }
            var activateAccountResponse = await Client.ExecuteTaskAsync(activateAccountRequest, cancellationToken);

            if (activateAccountResponse.StatusCode != HttpStatusCode.OK)
                return MbankResponse<LoginInfo>.Failed(activateAccountResponse);

            return new MbankResponse<LoginInfo>(tokenResponse, true, loginInfo);
        }

        public string GetSessionState()
        {
            var defaultParameters = Client.DefaultParameters;
            var tabIdParameter = defaultParameters.FirstOrDefault(p => p.Name == _tabIdParameterName);
            var tokenParameter = defaultParameters.FirstOrDefault(p => p.Name == _tokenParameterName);

            if (tokenParameter == null || tabIdParameter == null)
            {
                return null;
            }

            var sessionState = new MBankSessionState
            {
                CookieContainer = Client.CookieContainer,
                TabId = tabIdParameter.Value.ToString(),
                VerificationToken = tokenParameter.Value.ToString(),
                BaseUri = Client.BaseUrl
            };
            return sessionState.ToBase64String();
        }

        public bool SetSessionState(string serializedSessionState)
        {
            try
            {
                var sessionState = SerializationHelpers.FromBase64String<MBankSessionState>(serializedSessionState);

                Client.RemoveDefaultParameter(_tabIdParameterName);
                Client.RemoveDefaultParameter(_tokenParameterName);
                Client.AddDefaultParameter(_tabIdParameterName, sessionState.TabId, ParameterType.HttpHeader);
                Client.AddDefaultParameter(_tokenParameterName, sessionState.VerificationToken, ParameterType.HttpHeader);
                Client.CookieContainer = sessionState.CookieContainer;
                Client.BaseUrl = sessionState.BaseUri;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}

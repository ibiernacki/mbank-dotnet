using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ib.mbank
{
    public interface IMbankClient
    {
        /// <summary>
        /// RestClient for accessing cookies, certificates or to modify base URL
        /// </summary>
        IRestClient Client { get; }

        /// <summary>
        /// Login to mbank account
        /// </summary>
        /// <param name="login">Plain text login</param>
        /// <param name="password">Plain text password</param>
        /// <param name="accountType">Individual or Business</param>
        Task<IMbankResponse<LoginInfo>> Login(string login, string password, AccountType accountType);
        /// <summary>
        /// Login to mbank account
        /// </summary>
        /// <param name="login">Plain text login</param>
        /// <param name="password">Plain text password</param>
        /// <param name="accountType">Individual or Business</param>
        Task<IMbankResponse<LoginInfo>> Login(string login, string password, AccountType accountType, CancellationToken cancellationToken);

        /// <summary>
        ///<para>Acquires list and summaries of current bank accounts</para> 
        /// <exception cref="InvalidOperationException">User is not authenticated. Use Login method first</exception>
        /// </summary>
        Task<IMbankResponse<AccountInfo>> GetAccountInfo();
        /// <summary>
        ///<para>Acquires list and summaries of current bank accounts</para> 
        /// <exception cref="InvalidOperationException">User is not authenticated. Use Login method first</exception>
        /// </summary>
        Task<IMbankResponse<AccountInfo>> GetAccountInfo(CancellationToken cancellationToken);

        /// <summary>
        /// Acquires 25 most recent transactions. Before using this method, Login method must be used
        /// <exception cref="InvalidOperationException">User is not authenticated. Use Login method first</exception>
        /// </summary>
        Task<IMbankResponse<IList<Transaction>>> GetTransactions();

        // <summary>
        /// Acquires 25 most recent transactions. Before using this method, Login method must be used
        /// <exception cref="InvalidOperationException">User is not authenticated. Use Login method first</exception>
        /// </summary>
        Task<IMbankResponse<IList<Transaction>>> GetTransactions(CancellationToken cancellationToken);
    }
}

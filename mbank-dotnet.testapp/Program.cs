using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ib.mbank.testapp
{
    class Program
    {
        private static string SessionFileName = "mbank.session";

        static void Main(string[] args)
        {
            var mbank = new MbankClient();

            if (File.Exists(SessionFileName))
            {
                var sessionState = File.ReadAllText(SessionFileName);
                if (mbank.SetSessionState(sessionState))
                {
                    Console.WriteLine($"Restored session state from {SessionFileName}");
                }
                else
                {
                    Console.WriteLine($"Failed to restore session state from {SessionFileName}");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Session state not found, please provide credentials");
                Console.WriteLine();
                var credentials = GetCredentialsFromUser();
                var loginInfo = mbank.Login(credentials.username, credentials.password, AccountType.Individual).Result;
                var sessionState = mbank.GetSessionState();
                if (sessionState != null)
                {
                    File.WriteAllText(SessionFileName, sessionState);
                }
                if (!loginInfo.IsSuccess)
                {
                    Console.WriteLine("Login Failed");
                    Console.WriteLine($"Title: {loginInfo.Result.ErrorMessageTitle}");
                    Console.WriteLine($"Message: {loginInfo.Result.ErrorMessageBody}");
                    Console.ReadKey();
                    return;
                }
            }

            var accounts = mbank.GetAccountInfo().Result;
            var trans = mbank.GetTransactions().Result;

            Console.WriteLine("Accounts:");
            int i = 0;
            foreach (var account in accounts.Result.AllAccountsSummary)
            {
                Console.WriteLine($"{i}: {account.BalanceAmount:0.00} {account.Currency}");
            }
            Console.WriteLine();
            Console.WriteLine("Recent transactions: ");
            Console.WriteLine(trans.Result.OrderByDescending(t => t.Date).Aggregate("", (str, t) => str + $"{t.Date}\t{t.Amount}\t{t.Title}\n").TrimEnd('\n'));
            Console.ReadKey();
        }

        static (string username, string password) GetCredentialsFromUser()
        {
            Console.Write("Input your login: ");
            var login = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                login.Append(keyInfo.KeyChar);
            }
            Console.WriteLine();
            Console.Write("Input your password: ");
            var password = new StringBuilder();
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                password.Append(keyInfo.KeyChar);
            }
            return (login.ToString(), password.ToString());
        }
    }
}

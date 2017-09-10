using System;
using System.Linq;
using System.Text;

namespace ib.mbank.testapp
{
    class Program
    {
        static void Main(string[] args)
        {
            IMbankClient mbankClient = new MbankClient();

            Console.Write("Input your login: ");
            var login = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            while((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
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

            Console.WriteLine();

            var loginInfo = mbankClient.Login(login.ToString(), password.ToString(), AccountType.Individual).Result;

            if(!loginInfo.IsSuccess)
            {
                Console.WriteLine("Login Failed");
                Console.WriteLine($"Title: {loginInfo.Result.ErrorMessageTitle}");
                Console.WriteLine($"Message: {loginInfo.Result.ErrorMessageBody}");
                Console.ReadKey();
                return;
            }


            var accs = mbankClient.GetAccountInfo().Result;
            var trans = mbankClient.GetTransactions().Result;

            Console.WriteLine(trans.Result.Aggregate("", (str, t) => str + $"{t.Date}\t{t.Amount}\t{t.Title}\n").TrimEnd('\n'));
            Console.ReadKey();
        }
    }
}

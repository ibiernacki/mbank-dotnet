using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ib.mbank
{
    public class LoginInfo
    {

        public bool Button { get; set; }
        public string ErrorMessageBody { get; set; }
        public string ErrorMessageTitle { get; set; }
        public string RedirectUrl { get; set; }
        public bool Successful { get; set; }
        public string TabId { get; set; }
        public string SessionKeyForUW { get; set; }
        public string Token { get; set; }
    }
}

using System;
using System.Net;

namespace ib.mbank.Serialization
{
    [Serializable]
    internal class MBankSessionState
    {
        public CookieContainer CookieContainer { get; set; }
        public string TabId { get; set; }
        public string VerificationToken { get; set; }
        public Uri BaseUri { get; set; }
    }
}

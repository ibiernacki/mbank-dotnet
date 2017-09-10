using Newtonsoft.Json;
using System.Collections.Generic;

namespace ib.mbank
{
    public class AccountInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        public IList<AccountDetails> CurrentAccountsList { get; set; }
        public IList<AccountDetails> SavingAccountsList { get; set; }
        public IList<AccountDetails> OtherAccountsList { get; set; }
        public IList<AccountSummary> AllAccountsSummary { get; set; }
        public IList<AccountSummary> CurrentAccountsSummary { get; set; }
        public IList<AccountSummary> SavingAccountsSummary { get; set; }
        public IList<AccountSummary> OtherAccountsSummary { get; set; }
        [JsonProperty("profile")]
        public string Profile { get; set; }
    }
}

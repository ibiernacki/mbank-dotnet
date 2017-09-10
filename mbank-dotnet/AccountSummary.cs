using Newtonsoft.Json;

namespace ib.mbank
{
    public class AccountSummary
    {
        [JsonProperty("cCurrency")]
        public string Currency { get; set; }
        [JsonProperty("isDefaultCurrency")]
        public bool IsDefaultCurrency { get; set; }
        [JsonProperty("mBalanceAmount")]
        public double BalanceAmount { get; set; }
        [JsonProperty("mOwnBalanceAmount")]
        public double OwnBalanceAmount { get; set; }
        [JsonProperty("mAvailableBalanceAmount")]
        public double AvailableBalanceAmount { get; set; }
    }
}

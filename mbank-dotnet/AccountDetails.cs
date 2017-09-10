using Newtonsoft.Json;

namespace ib.mbank
{
    public class AccountDetails
    {
        [JsonProperty("cID")]
        public string Id { get; set; }
        [JsonProperty("cProductName")]
        public string ProductName { get; set; }
        [JsonProperty("cSubTitle")]
        public string SubTitle { get; set; }
        [JsonProperty("cAccountNumberForDisp")]
        public string DisplayAccountNumber { get; set; }
        [JsonProperty("mBalance")]
        public double Balance { get; set; }
        [JsonProperty("mAvailableBalance")]
        public double AvailableBalance { get; set; }
        [JsonProperty("mOwnBalance")]
        public double OwnBalance { get; set; }
        [JsonProperty("cCurrency")]
        public string Currency { get; set; }
        [JsonProperty("isAuxiliary")]
        public bool IsAuxiliary { get; set; }
    }
}

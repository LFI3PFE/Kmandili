using Newtonsoft.Json;

namespace Kmandili.Models.LocalModels
{
    public class TokenResponse
    {
        public int UserId { get; set; }
        public string Type { get; set; }
        public string access_token { get; set; }
        [JsonProperty(PropertyName = ".expires")]
        public string expires { get; set; }
    }
}

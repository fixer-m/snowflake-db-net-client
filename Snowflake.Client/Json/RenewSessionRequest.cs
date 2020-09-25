using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class RenewSessionRequest : BaseRequest
    {
        [JsonPropertyName("oldSessionToken")]
        public string OldSessionToken { get; set; }

        [JsonPropertyName("requestType")]
        public string RequestType { get; set; }
    }
}
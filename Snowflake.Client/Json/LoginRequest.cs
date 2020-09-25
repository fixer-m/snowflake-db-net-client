using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class LoginRequest : BaseRequest
    {
        [JsonPropertyName("data")]
        public LoginRequestData Data { get; set; }
    }
}
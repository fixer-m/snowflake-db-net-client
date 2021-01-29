using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class NullDataResponse : BaseResponse
    {
        [JsonPropertyName("data")]
        public object data { get; set; }
    }
}
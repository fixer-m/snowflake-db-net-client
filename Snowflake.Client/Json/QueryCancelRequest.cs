using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class CancelQueryRequest : BaseRequest
    {
        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }
    }
}
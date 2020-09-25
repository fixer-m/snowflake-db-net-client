using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class QueryCancelRequest : BaseRequest
    {
        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }
    }
}
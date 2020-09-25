using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class QueryRequest : BaseRequest
    {
        [JsonPropertyName("sqlText")]
        public string SqlText { get; set; }

        [JsonPropertyName("describeOnly")]
        public bool DescribeOnly { get; set; }

        [JsonPropertyName("bindings")]
        public Dictionary<string, ParamBinding> Bindings { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class ParamBinding
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}; Value: {Value}";
        }
    }
}
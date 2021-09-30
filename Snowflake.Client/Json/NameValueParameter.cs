using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class NameValueParameter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
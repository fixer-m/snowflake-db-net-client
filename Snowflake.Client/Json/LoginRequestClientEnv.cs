using System.Text.Json.Serialization;

namespace Snowflake.Client.Json
{
    public class LoginRequestClientEnv
    {
        [JsonPropertyName("APPLICATION")]
        public string Application { get; set; }

        [JsonPropertyName("OS_VERSION")]
        public string OSVersion { get; set; }

        [JsonPropertyName("NET_RUNTIME")]
        public string NETRuntime { get; set; }

        [JsonPropertyName("NET_VERSION")]
        public string NETVersion { get; set; }

        public override string ToString()
        {
            return $"Application: {Application}; OS_Version: {OSVersion}; NET_Runtime: {NETRuntime}; NET_Version: {NETVersion}";
        }
    }
}
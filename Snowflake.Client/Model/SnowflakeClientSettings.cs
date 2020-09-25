using System.Text.Json;

namespace Snowflake.Client.Model
{
    public class SnowflakeClientSettings
    {
        public AuthInfo AuthInfo { get; private set; }
        public UrlInfo UrlInfo { get; private set; }
        public SessionInfo SessionInfo { get; private set; }
        public JsonSerializerOptions JsonMapperOptions { get; private set; }

        public SnowflakeClientSettings(AuthInfo authInfo, SessionInfo sessionInfo = null, UrlInfo urlInfo = null, JsonSerializerOptions jsonMapperOptions = null)
        {
            AuthInfo = authInfo ?? new AuthInfo();
            SessionInfo = sessionInfo ?? new SessionInfo();
            UrlInfo = urlInfo ?? new UrlInfo();
            JsonMapperOptions = jsonMapperOptions ?? new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            UrlInfo.Host = authInfo.GetHostName();
        }
    }
}
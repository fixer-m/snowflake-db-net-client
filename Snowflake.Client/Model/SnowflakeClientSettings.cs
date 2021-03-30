using System.Text.Json;

namespace Snowflake.Client.Model
{
    /// <summary>
    /// Configuration for SnowlfakeClient
    /// </summary>
    public class SnowflakeClientSettings
    {
        /// <summary>
        /// Data used to authenticate in Snowflake: user, password, account and region
        /// </summary>
        public AuthInfo AuthInfo { get; private set; }

        /// <summary>
        /// Snowflake URL: host, protocol and port
        /// </summary>
        public UrlInfo UrlInfo { get; private set; }

        /// <summary>
        /// Snowflake session objects to set: role, schema, database and warehouse
        /// </summary>
        public SessionInfo SessionInfo { get; private set; }

        /// <summary>
        /// Serializeer options used to map data response to your model
        /// </summary>
        public JsonSerializerOptions JsonMapperOptions { get; private set; }

        public SnowflakeClientSettings(AuthInfo authInfo, SessionInfo sessionInfo = null, UrlInfo urlInfo = null, JsonSerializerOptions jsonMapperOptions = null)
        {
            AuthInfo = authInfo ?? new AuthInfo();
            SessionInfo = sessionInfo ?? new SessionInfo();
            UrlInfo = urlInfo ?? new UrlInfo();
            JsonMapperOptions = jsonMapperOptions ?? new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            if (string.IsNullOrEmpty(UrlInfo.Host))
                UrlInfo.Host = authInfo.GetHostName();
        }
    }
}
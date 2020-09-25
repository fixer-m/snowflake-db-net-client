using System.Collections.Generic;

namespace Snowflake.Client.Json
{
    public class LoginResponseData
    {
        public string MasterToken { get; set; }
        public string Token { get; set; }
        public int ValidityInSeconds { get; set; }
        public int MasterValidityInSeconds { get; set; }
        public string DisplayUserName { get; set; }
        public string ServerVersion { get; set; }
        public bool FirstLogin { get; set; }
        public string RemMeToken { get; set; }
        public int RemMeValidityInSeconds { get; set; }
        public int HealthCheckInterval { get; set; }
        public string NewClientForUpgrade { get; set; }
        public long SessionId { get; set; }
        public List<NameValueParameter> Parameters { get; set; }
        public SessionInfoRaw SessionInfo { get; set; }
        public string IdToken { get; set; }
        public int IdTokenValidityInSeconds { get; set; }
    }
}
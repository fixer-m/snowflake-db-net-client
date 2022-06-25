using Snowflake.Client.Json;

namespace Snowflake.Client.Model
{
    /// <summary>
    /// Snowflake Session information
    /// </summary>
    public class SnowflakeSession
    {
        public string MasterToken { get; private set; }
        public string SessionToken { get; private set; }
        public int ValidityInSeconds { get; private set; }
        public int MasterValidityInSeconds { get; private set; }
        public string DisplayUserName { get; private set; }
        public string ServerVersion { get; private set; }
        public bool FirstLogin { get; private set; }
        public string RemMeToken { get; private set; }
        public int RemMeValidityInSeconds { get; private set; }
        public int HealthCheckInterval { get; private set; }
        public string NewClientForUpgrade { get; private set; }
        public long SessionId { get; private set; }
        public string IdToken { get; private set; }
        public int IdTokenValidityInSeconds { get; private set; }
        public string DatabaseName { get; private set; }
        public string SchemaName { get; private set; }
        public string WarehouseName { get; private set; }
        public string RoleName { get; private set; }

        public SnowflakeSession(LoginResponseData loginResponseData)
        {
            SessionToken = loginResponseData.Token;

            MasterToken = loginResponseData.MasterToken;
            ValidityInSeconds = loginResponseData.ValidityInSeconds;
            MasterValidityInSeconds = loginResponseData.MasterValidityInSeconds;
            DisplayUserName = loginResponseData.DisplayUserName;
            ServerVersion = loginResponseData.ServerVersion;
            FirstLogin = loginResponseData.FirstLogin;
            RemMeToken = loginResponseData.RemMeToken;
            RemMeValidityInSeconds = loginResponseData.RemMeValidityInSeconds;
            HealthCheckInterval = loginResponseData.HealthCheckInterval;
            NewClientForUpgrade = loginResponseData.NewClientForUpgrade;
            SessionId = loginResponseData.SessionId;
            IdToken = loginResponseData.IdToken;
            IdTokenValidityInSeconds = loginResponseData.IdTokenValidityInSeconds;
            DatabaseName = loginResponseData.SessionInfo.DatabaseName;
            SchemaName = loginResponseData.SessionInfo.SchemaName;
            WarehouseName = loginResponseData.SessionInfo.WarehouseName;
            RoleName = loginResponseData.SessionInfo.RoleName;
        }

        internal void Renew(RenewSessionResponseData renewSessionResponseData)
        {
            SessionToken = renewSessionResponseData.SessionToken;

            MasterToken = renewSessionResponseData.MasterToken;
            SessionId = renewSessionResponseData.SessionId;
            ValidityInSeconds = renewSessionResponseData.ValidityInSecondsST;
            MasterValidityInSeconds = renewSessionResponseData.ValidityInSecondsMT;
        }

        public override string ToString()
        {
            return $"User: {DisplayUserName}; Role: {RoleName}; Warehouse: {WarehouseName}";
        }
    }
}
using System;
using Snowflake.Client.Json;

namespace Snowflake.Client
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
            this.SessionToken = loginResponseData.Token;

            this.MasterToken = loginResponseData.MasterToken;
            this.ValidityInSeconds = loginResponseData.ValidityInSeconds;
            this.MasterValidityInSeconds = loginResponseData.MasterValidityInSeconds;
            this.DisplayUserName = loginResponseData.DisplayUserName;
            this.ServerVersion = loginResponseData.ServerVersion;
            this.FirstLogin = loginResponseData.FirstLogin;
            this.RemMeToken = loginResponseData.RemMeToken;
            this.RemMeValidityInSeconds = loginResponseData.RemMeValidityInSeconds;
            this.HealthCheckInterval = loginResponseData.HealthCheckInterval;
            this.NewClientForUpgrade = loginResponseData.NewClientForUpgrade;
            this.SessionId = loginResponseData.SessionId;
            this.IdToken = loginResponseData.IdToken;
            this.IdTokenValidityInSeconds = loginResponseData.IdTokenValidityInSeconds;
            this.DatabaseName = loginResponseData.SessionInfo.DatabaseName;
            this.SchemaName = loginResponseData.SessionInfo.SchemaName;
            this.WarehouseName = loginResponseData.SessionInfo.WarehouseName;
            this.RoleName = loginResponseData.SessionInfo.RoleName;
        }

        public override string ToString()
        {
            return $"User: {DisplayUserName}; Role: {RoleName}; Warehouse: {WarehouseName}";
        }
    }
}
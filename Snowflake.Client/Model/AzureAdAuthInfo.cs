namespace Snowflake.Client.Model
{
    public class AzureAdAuthInfo : AuthInfo
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ServicePrincipalObjectId { get; set; }
        public string TenantId { get; set; }
        public string Scope { get; set; }
        public string Host {get; set; }
        public string Role {get; set; }


        public AzureAdAuthInfo(string clientId, string clientSecret, string servicePrincipalObjectId, string tenantId, string scope, string region, string account, string user, string host, string role)
            : base(user, account, region)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            ServicePrincipalObjectId = servicePrincipalObjectId;
            TenantId = tenantId;
            Scope = scope;
            Region = region;
            Account = account;
            User = user;
            Host = host;
            Role = role;
        }
    }
}
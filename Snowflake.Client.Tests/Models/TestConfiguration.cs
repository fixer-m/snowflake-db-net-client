namespace Snowflake.Client.Tests.Models
{
    public class TestConfiguration
    {
        public SnowflakeConnectionInfo Connection { get; set; }
        public string AdClientId { get; set; }
        public string AdClientSecret { get; set; }
        public string AdServicePrincipalObjectId { get; set; }
        public string AdTenantId { get; set; }
        public string AdScope { get; set; }
    }
}

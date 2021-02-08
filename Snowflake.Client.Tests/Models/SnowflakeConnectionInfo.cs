namespace Snowflake.Client.Tests.IntegrationTests.Models
{
    public class SnowflakeConnectionInfo
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Account { get; set; }
        public string Region { get; set; }
        public string Warehouse { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string Role { get; set; }
        public string Host { get; set; }
        public string Protocol { get; set; }
        public int Port { get; set; }
    }
}

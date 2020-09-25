namespace Snowflake.Client.Json
{
    public class RenewSessionResponseData
    {
        public string SessionToken { get; set; }
        public int ValidityInSecondsST { get; set; }
        public string MasterToken { get; set; }
        public int ValidityInSecondsMT { get; set; }
        public long SessionId { get; set; }
    }
}
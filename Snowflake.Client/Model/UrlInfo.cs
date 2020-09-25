namespace Snowflake.Client.Model
{
    public class UrlInfo
    {
        public string Host { get; set; }
        public string Protocol { get; set; }
        public int Port { get; set; }

        public UrlInfo()
        {
            Protocol = "https";
            Port = 443;
        }
    }
}
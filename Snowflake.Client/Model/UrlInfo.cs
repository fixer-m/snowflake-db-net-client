namespace Snowflake.Client.Model
{
    /// <summary>
    /// Represents information about Snowflake URL.
    /// </summary>
    public class UrlInfo
    {
        /// <summary>
        /// Snowflake URL host. If not specified, will be constructed as {Account}.{Region}.snowflakecomputing.com.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Supported values: "https" (default) and "http"
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Port number. Should be 443 for https (default) or 80 for http.
        /// </summary>
        public int Port { get; set; }

        public UrlInfo()
        {
            Protocol = "https";
            Port = 443;
        }
    }
}
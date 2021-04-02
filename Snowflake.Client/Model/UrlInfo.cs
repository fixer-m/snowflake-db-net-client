using System;

namespace Snowflake.Client.Model
{
    /// <summary>
    /// Represents information about Snowflake URL.
    /// </summary>
    public class UrlInfo
    {
        /// <summary>
        /// Snowflake URL host. Should end up with ".snowflakecomputing.com".
        /// If not specified, will be constructed as Account.Region.Cloud.snowflakecomputing.com.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Supported values: "https" (default) and "http"
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Port number. Should be 443 (default) for https or 80 for http.
        /// </summary>
        public int Port { get; set; }

        public UrlInfo()
        {
            Protocol = "https";
            Port = 443;
        }

        public UrlInfo(string host, string protocol = "https", int port = 443)
        {
            Host = host;
            Protocol = protocol;
            Port = port;
        }

        public UrlInfo(Uri snowflakeUrl) : this(snowflakeUrl.Host, snowflakeUrl.Scheme, snowflakeUrl.Port)
        {
        }

        public override string ToString()
        {
            return $"{Protocol}://{Host}:{Port}";
        }
    }
}
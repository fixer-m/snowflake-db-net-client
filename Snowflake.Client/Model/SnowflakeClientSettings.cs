using Snowflake.Client.Helpers;
using System;
using System.Text.Json;

namespace Snowflake.Client.Model
{
    /// <summary>
    /// Configuration for SnowflakeClient
    /// </summary>
    public class SnowflakeClientSettings
    {
        /// <summary>
        /// Data used to authenticate in Snowflake: user, password, account and region
        /// </summary>
        public AuthInfo AuthInfo { get; }

        /// <summary>
        /// Snowflake URL: host, protocol and port
        /// </summary>
        public UrlInfo UrlInfo { get; }

        /// <summary>
        /// Snowflake session objects to set: role, schema, database and warehouse
        /// </summary>
        public SessionInfo SessionInfo { get; }

        /// <summary>
        /// Serializer options used to map data response to your model
        /// </summary>
        public JsonSerializerOptions JsonMapperOptions { get; }

        /// <summary>
        /// Options used in ChunksDownloader
        /// </summary>
        public ChunksDownloaderOptions ChunksDownloaderOptions { get; }

        /// <summary>
        /// Snowflake can return response data in a table form ("rowset") or in chunks or both.
        /// Set this parameter to true to fetch chunks, so the whole data set will be in a rowset. 
        /// Default value: False 
        /// </summary>
        public bool DownloadChunksForQueryRawResponses { get; set; }

        public SnowflakeClientSettings(AuthInfo authInfo, SessionInfo sessionInfo = null, UrlInfo urlInfo = null,
            JsonSerializerOptions jsonMapperOptions = null, ChunksDownloaderOptions chunksDownloaderOptions = null,
            bool downloadChunksForQueryRawResponses = false)
        {
            AuthInfo = authInfo ?? new AuthInfo();
            SessionInfo = sessionInfo ?? new SessionInfo();
            UrlInfo = urlInfo ?? new UrlInfo();
            JsonMapperOptions = jsonMapperOptions ?? new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            ChunksDownloaderOptions = chunksDownloaderOptions ?? new ChunksDownloaderOptions() { PrefetchThreadsCount = 4 };
            DownloadChunksForQueryRawResponses = downloadChunksForQueryRawResponses;

            UrlInfo.Host = string.IsNullOrEmpty(UrlInfo.Host)
                ? BuildHostName(AuthInfo.Account, AuthInfo.Region)
                : ReplaceUnderscores(UrlInfo.Host);
        }

        private static string BuildHostName(string account, string region)
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("Account name cannot be empty.");

            var hostname = $"{ReplaceUnderscores(account)}.";

            if (!string.IsNullOrEmpty(region) && region.ToLower() != "us-west-2")
                hostname += $"{region}.";

            var cloudTag = SnowflakeUtils.GetCloudTagByRegion(region);

            if (!string.IsNullOrEmpty(cloudTag))
                hostname += $"{cloudTag}.";

            hostname += "snowflakecomputing.com";

            return hostname.ToLower();
        }

        // Underscores in hostname will lead to SSL cert verification issue.
        // See https://github.com/snowflakedb/snowflake-connector-net/issues/160#issuecomment-692883663
        private static string ReplaceUnderscores(string account)
        {
            return account.Replace("_", "-");
        }
    }
}
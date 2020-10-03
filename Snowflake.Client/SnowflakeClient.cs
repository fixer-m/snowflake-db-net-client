using Snowflake.Client.Json;
using Snowflake.Client.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Snowflake.Client
{
    public class SnowflakeClient
    {
        /// <summary>
        /// Current Snowflake session.
        /// </summary>
        public SnowflakeSession SnowflakeSession { get => session; }

        private SnowflakeSession session;
        private readonly RestClient restClient;
        private readonly RequestBuilder requestBuilder;
        private readonly SnowflakeClientSettings clientSettings;

        /// <summary>
        /// Creates new client and initializes new Snowflake session. 
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        /// <param name="account">Account</param>
        /// <param name="region">Region ("us-east-1" by default)</param>
        public SnowflakeClient(string user, string password, string account, string region = null)
         : this(new AuthInfo(user, password, account, region))
        {
        }

        /// <summary>
        /// Creates new client and initializes new Snowflake session. 
        /// </summary>
        /// <param name="authInfo">Auth information: user, password, account, region</param>
        /// <param name="sessionInfo">Session information: role, schema, database, warehouse</param>
        /// <param name="urlInfo">URL information: host, protocol and port</param>
        /// <param name="jsonMapperOptions">JsonSerializerOptions which will be used to map response to your model</param>
        public SnowflakeClient(AuthInfo authInfo, SessionInfo sessionInfo = null, UrlInfo urlInfo = null, JsonSerializerOptions jsonMapperOptions = null)
        : this(new SnowflakeClientSettings(authInfo))
        {
        }

        /// <summary>
        /// Creates new client and initializes new Snowflake session. 
        /// </summary>
        /// <param name="settings">Client settings to initialize new session.</param>
        public SnowflakeClient(SnowflakeClientSettings settings)
        {
            ValidateClientSettings(settings);

            clientSettings = settings;
            restClient = new RestClient();
            requestBuilder = new RequestBuilder(settings.UrlInfo);
            SnowflakeDataMapper.SetJsonMapperOptions(settings.JsonMapperOptions);

            CreateNewSession(settings.AuthInfo, settings.SessionInfo);
        }

        private void ValidateClientSettings(SnowflakeClientSettings settings)
        {
            if (settings == null)
                throw new SnowflakeException($"Settings object cannot be null.");

            if (string.IsNullOrEmpty(settings.AuthInfo.User))
                throw new SnowflakeException($"User name is either empty or null.");

            if (string.IsNullOrEmpty(settings.AuthInfo.Password))
                throw new SnowflakeException($"User password is either empty or null.");

            if (string.IsNullOrEmpty(settings.AuthInfo.Account))
                throw new SnowflakeException($"Snowflake account is either empty or null.");

            if (settings.UrlInfo.Protocol != "https" && settings.UrlInfo.Protocol != "http")
                throw new SnowflakeException($"URL Protocol should be either http or https.");

            if (string.IsNullOrEmpty(settings.UrlInfo.Host))
                throw new SnowflakeException($"URL Host cannot be empty.");
        }

        /// <summary>
        /// Initializes new Snowflake session.
        /// </summary>
        /// <param name="authInfo">Auth information: user, password, account, region</param>
        /// <param name="sessionInfo">Session information: role, schema, database, warehouse</param>
        /// <returns>True</returns>
        public bool CreateNewSession(AuthInfo authInfo, SessionInfo sessionInfo)
        {
            session = Authenticate(authInfo, sessionInfo);
            requestBuilder.SetSessionToken(session.SessionToken);

            return true;
        }

        private SnowflakeSession Authenticate(AuthInfo authInfo, SessionInfo sessionInfo)
        {
            var loginRequest = requestBuilder.BuildLoginRequest(authInfo, sessionInfo);

            var response = restClient.Send<LoginResponse>(loginRequest);

            if (!response.Success)
                throw new SnowflakeException($"Athentication failed. Message: {response.Message}", response.Code);

            return new SnowflakeSession(response.Data);
        }

        /// <summary>
        /// Execute SQL that selects a single value.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <returns>The first cell value returned as string.</returns>
        public string ExecuteScalar(string sql, object sqlParams = null)
        {
            var response = QueryInternal(sql, sqlParams);
            var rawResult = response.Data.RowSet[0][0];

            return rawResult;
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <returns>A sequence of data of the supplied type: one instance per row.</returns>
        public IEnumerable<T> Query<T>(string sql, object sqlParams = null)
        {
            var response = QueryInternal(sql, sqlParams);

            if (response.Data.Chunks.Count > 0)
                throw new SnowflakeException($"Downloading data from chunks is not implemented yet.");

            var result = SnowflakeDataMapper.Map<T>(response.Data);
            return result;
        }

        /// <summary>
        /// Executes a query, returning the raw data returned by REST API (rows and columns).
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <param name="describeOnly">Return only columns information.</param>
        /// <returns>Rows and columns.</returns>
        public SnowflakeRawData QueryRaw(string sql, object sqlParams = null, bool describeOnly = false)
        {
            var response = QueryInternal(sql, sqlParams, describeOnly);

            if (response.Data.Chunks.Count > 0)
                throw new SnowflakeException($"Downloading data from chunks is not implemented yet.");

            var result = new SnowflakeRawData()
            {
                Columns = response.Data.RowType,
                Rows = response.Data.RowSet
            };

            return result;
        }

        private QueryExecResponse QueryInternal(string sql, object sqlParams = null, bool describeOnly = false)
        {
            var queryRequest = requestBuilder.BuildQueryRequest(sql, sqlParams, describeOnly);
            var response = restClient.Send<QueryExecResponse>(queryRequest);

            if (!response.Success)
                throw new SnowflakeException($"Query execution failed. Message: {response.Message}", response.Code);

            return response;
        }

        /// <summary>
        /// Closes current Snowflake session. 
        /// </summary>
        /// <returns>True if session was successfully closed.</returns>
        public bool CloseSession()
        {
            var closeSessionRequest = requestBuilder.BuildCloseSessionRequest();
            var response = restClient.Send<CloseResponse>(closeSessionRequest);

            if (!response.Success)
                throw new SnowflakeException($"Closing session failed. Message: {response.Message}", response.Code);

            requestBuilder.ClearSessionToken();

            return response.Success;
        }
    }
}
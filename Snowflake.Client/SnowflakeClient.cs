using Snowflake.Client.Json;
using Snowflake.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public class SnowflakeClient : ISnowflakeClient
    {
        /// <summary>
        /// Current Snowflake session.
        /// </summary>
        public SnowflakeSession SnowflakeSession { get => _session; }

        private SnowflakeSession _session;
        private readonly RestClient _restClient;
        private readonly RequestBuilder _requestBuilder;
        private readonly SnowflakeClientSettings _clientSettings;

        /// <summary>
        /// Creates new Snowflake client. 
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
        /// Creates new Snowflake client. 
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
        /// Creates new Snowflake client. 
        /// </summary>
        /// <param name="settings">Client settings to initialize new session.</param>
        public SnowflakeClient(SnowflakeClientSettings settings)
        {
            ValidateClientSettings(settings);

            _clientSettings = settings;
            _restClient = new RestClient();
            _requestBuilder = new RequestBuilder(settings.UrlInfo);
            SnowflakeDataMapper.SetJsonMapperOptions(settings.JsonMapperOptions);
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
        /// <returns>True if session succesfully initialized</returns>
        public async Task<bool> InitNewSessionAsync()
        {
            _session = await AuthenticateAsync(_clientSettings.AuthInfo, _clientSettings.SessionInfo).ConfigureAwait(false);
            _requestBuilder.SetSessionToken(_session.SessionToken);

            return true;
        }

        private async Task<SnowflakeSession> AuthenticateAsync(AuthInfo authInfo, SessionInfo sessionInfo)
        {
            var loginRequest = _requestBuilder.BuildLoginRequest(authInfo, sessionInfo);

            var response = await _restClient.SendAsync<LoginResponse>(loginRequest).ConfigureAwait(false);

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
        public async Task<string> ExecuteScalarAsync(string sql, object sqlParams = null)
        {
            var response = await QueryInternalAsync(sql, sqlParams).ConfigureAwait(false);
            var rawResult = response.Data.RowSet.FirstOrDefault()?.FirstOrDefault();

            return rawResult;
        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="sqlParams">The parameters to use for this query.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<long> ExecuteAsync(string sql, object sqlParams = null)
        {
            var response = await QueryInternalAsync(sql, sqlParams).ConfigureAwait(false);
            long affectedRows = SnowflakeUtils.GetAffectedRowsCount(response);

            return affectedRows;
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <returns>A sequence of data of the supplied type: one instance per row.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object sqlParams = null)
        {
            var response = await QueryInternalAsync(sql, sqlParams).ConfigureAwait(false);

            if (response.Data.Chunks != null && response.Data.Chunks.Count > 0)
                throw new SnowflakeException($"Downloading data from chunks is not implemented yet.");

            var result = SnowflakeDataMapper.MapTo<T>(response.Data.RowType, response.Data.RowSet);
            return result;
        }

        /// <summary>
        /// Executes a query, returning the raw data returned by REST API (rows, columns and query information).
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <param name="describeOnly">Return only columns information.</param>
        /// <returns>Rows and columns.</returns>
        public async Task<SnowflakeQueryRawResponse> QueryRawResponseAsync(string sql, object sqlParams = null, bool describeOnly = false)
        {
            var response = await QueryInternalAsync(sql, sqlParams, describeOnly).ConfigureAwait(false);

            return new SnowflakeQueryRawResponse(response.Data);
        }

        private async Task<bool> EnsureSessionIniatializedAsync()
        {
            return _session == null ? await InitNewSessionAsync().ConfigureAwait(false) : true;
        }

        private async Task<QueryExecResponse> QueryInternalAsync(string sql, object sqlParams = null, bool describeOnly = false)
        {
            await EnsureSessionIniatializedAsync().ConfigureAwait(false);

            var queryRequest = _requestBuilder.BuildQueryRequest(sql, sqlParams, describeOnly);
            var response = await _restClient.SendAsync<QueryExecResponse>(queryRequest).ConfigureAwait(false);

            if (!response.Success)
                throw new SnowflakeException($"Query execution failed. Message: {response.Message}", response.Code);

            return response;
        }

        /// <summary>
        /// Closes current Snowflake session. 
        /// </summary>
        /// <returns>True if session was successfully closed.</returns>
        public async Task<bool> CloseSessionAsync()
        {
            var closeSessionRequest = _requestBuilder.BuildCloseSessionRequest();
            var response = await _restClient.SendAsync<CloseResponse>(closeSessionRequest).ConfigureAwait(false);

            _session = null;
            _requestBuilder.ClearSessionToken();

            if (!response.Success)
                throw new SnowflakeException($"Closing session failed. Message: {response.Message}", response.Code);

            return response.Success;
        }
    }
}
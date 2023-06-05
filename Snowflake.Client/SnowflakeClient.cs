using Snowflake.Client.Helpers;
using Snowflake.Client.Json;
using Snowflake.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Snowflake.Client
{
    public class SnowflakeClient : ISnowflakeClient
    {
        /// <summary>
        /// Current Snowflake session. Null if not initialized.
        /// </summary>
        public SnowflakeSession SnowflakeSession => _snowflakeSession;

        /// <summary>
        /// Snowflake Client settings
        /// </summary>
        public SnowflakeClientSettings Settings => _clientSettings;

        /// <summary>
        /// Azure AD Token Provider
        /// </summary>
        private readonly AzureAdTokenProvider _azureAdTokenProvider;

        private SnowflakeSession _snowflakeSession;
        private readonly RestClient _restClient;
        private readonly RequestBuilder _requestBuilder;
        private readonly SnowflakeClientSettings _clientSettings;

        /// <summary>
        /// Creates new Snowflake client.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <param name="servicePrincipalObjectId">Service Principal Object ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="scope">Scope</param>
        /// <param name="region">Region: "us-east-1", etc. Required for all except for US West Oregon (us-west-2).</param>
        /// <param name="account">Account</param>
        /// <param name="user">Username</param>
        /// <param name="host">Host</param>
        /// <param name="role">Role</param>
        public SnowflakeClient(string clientId, string clientSecret, string servicePrincipalObjectId, string tenantId, string scope, string region, string account, string user, string host, string role)
            : this(new AzureAdAuthInfo(clientId, clientSecret, servicePrincipalObjectId, tenantId, scope, region, account, user, host, role), urlInfo: new UrlInfo
            {
                Host = host,
            },
            sessionInfo: new SessionInfo
            {
                Role = role,
            })
        {
        }

        /// <summary>
        /// Creates new Snowflake client. 
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="password">Password</param>
        /// <param name="account">Account</param>
        /// <param name="region">Region: "us-east-1", etc. Required for all except for US West Oregon (us-west-2).</param>
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
            : this(new SnowflakeClientSettings(authInfo, sessionInfo, urlInfo, jsonMapperOptions))
        {
        }

        public SnowflakeClient(AzureAdAuthInfo authInfo, SessionInfo sessionInfo = null, UrlInfo urlInfo = null, JsonSerializerOptions jsonMapperOptions = null)
            : this(new SnowflakeClientSettings(authInfo, sessionInfo, urlInfo, jsonMapperOptions))
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
            _azureAdTokenProvider = new AzureAdTokenProvider();

            SnowflakeDataMapper.Configure(settings.JsonMapperOptions);
            ChunksDownloader.Configure(settings.ChunksDownloaderOptions);
        }

        private static void ValidateClientSettings(SnowflakeClientSettings settings)
        {
            if (settings == null)
                throw new ArgumentException("Settings object cannot be null.");

            if (string.IsNullOrEmpty(settings.AuthInfo?.User))
                throw new ArgumentException("User name is either empty or null.");

            if (string.IsNullOrEmpty(settings.AuthInfo?.Password))
                throw new ArgumentException("User password is either empty or null.");

            if (string.IsNullOrEmpty(settings.AuthInfo?.Account))
                throw new ArgumentException("Snowflake account is either empty or null.");

            if (settings.UrlInfo?.Protocol != "https" && settings.UrlInfo?.Protocol != "http")
                throw new ArgumentException("URL Protocol should be either http or https.");

            if (string.IsNullOrEmpty(settings.UrlInfo?.Host))
                throw new ArgumentException("URL Host cannot be empty.");

            if (!settings.UrlInfo.Host.ToLower().EndsWith(".snowflakecomputing.com"))
                throw new ArgumentException("URL Host should end up with '.snowflakecomputing.com'.");
        }

        /// <summary>
        /// Initializes new Snowflake session.
        /// </summary>
        /// <returns>True if session successfully initialized</returns>
        public async Task<bool> InitNewSessionAsync(CancellationToken ct = default)
        {
            _snowflakeSession = await AuthenticateAsync(_clientSettings.AuthInfo, _clientSettings.SessionInfo, ct).ConfigureAwait(false);
            _requestBuilder.SetSessionTokens(_snowflakeSession.SessionToken, _snowflakeSession.MasterToken);

            return true;
        }

        /// <summary>
        /// Authenticates user and returns new Snowflake session.
        /// </summary>
        /// <returns>New Snowflake session</returns>
        private async Task<SnowflakeSession> AuthenticateAsync(AuthInfo authInfo, SessionInfo sessionInfo, CancellationToken ct)
        {
            var loginRequest = _requestBuilder.BuildLoginRequest(authInfo, sessionInfo);

            if(authInfo is AzureAdAuthInfo azureAdAuthInfo)
            {
                var azureAdAccessToken = await _azureAdTokenProvider.GetAzureAdAccessTokenAsync(azureAdAuthInfo, ct).ConfigureAwait(false);
                loginRequest = _requestBuilder.BuildLoginRequest(authInfo, sessionInfo, azureAdAccessToken);
            } 
            else 
            {
                loginRequest = _requestBuilder.BuildLoginRequest(authInfo, sessionInfo);
            }

            var response = await _restClient.SendAsync<LoginResponse>(loginRequest, ct).ConfigureAwait(false);

            if (!response.Success)
                throw new SnowflakeException($"Authentication failed. Message: {response.Message}", response.Code);

            return new SnowflakeSession(response.Data);
        }

        /// <summary>
        /// Renew session
        /// </summary>
        /// <returns>True if session successfully renewed</returns>
        public async Task<bool> RenewSessionAsync(CancellationToken ct = default)
        {
            if (_snowflakeSession == null)
                throw new SnowflakeException("Session is not initialized yet.");

            var renewSessionRequest = _requestBuilder.BuildRenewSessionRequest();
            var response = await _restClient.SendAsync<RenewSessionResponse>(renewSessionRequest, ct).ConfigureAwait(false);
            
            if (response.Success)
                _snowflakeSession.Renew(response.Data);
            else if (response.Code == 390114)
                // Authentication token expired, re-authenticate
                await InitNewSessionAsync(ct).ConfigureAwait(false);
            else
                throw new SnowflakeException($"Renew session failed. Message: {response.Message}", response.Code);
                
            _requestBuilder.SetSessionTokens(_snowflakeSession.SessionToken, _snowflakeSession.MasterToken);

            return true;
        }

        /// <summary>
        /// Execute SQL that selects a single value.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <returns>The first cell value returned as string.</returns>
        public async Task<string> ExecuteScalarAsync(string sql, object sqlParams = null, CancellationToken ct = default)
        {
            var response = await QueryInternalAsync(sql, sqlParams, false, ct).ConfigureAwait(false);
            var rawResult = response.Data.RowSet.FirstOrDefault()?.FirstOrDefault();

            return rawResult;
        }

        /// <summary>
        /// Execute SQL that selects a single value.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <returns>The first cell value returned as of type T.</returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object sqlParams = null, CancellationToken ct = default)
        {
            var response = await QueryInternalAsync(sql, sqlParams, false, ct).ConfigureAwait(false);

            var firstColumn = response.Data.RowType.FirstOrDefault();
            var firstColumnValue = response.Data.RowSet.FirstOrDefault()?.FirstOrDefault();

            var result = SnowflakeDataMapper.MapTo<T>(firstColumn, firstColumnValue);
            return result;
        }

        /// <summary>
        /// Execute parameterized SQL.
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="sqlParams">The parameters to use for this query.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<long> ExecuteAsync(string sql, object sqlParams = null, CancellationToken ct = default)
        {
            var response = await QueryInternalAsync(sql, sqlParams, false, ct).ConfigureAwait(false);
            var affectedRows = SnowflakeUtils.GetAffectedRowsCount(response);

            return affectedRows;
        }

        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <returns>A sequence of data of the supplied type: one instance per row.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object sqlParams = null, CancellationToken ct = default)
        {
            var response = await QueryInternalAsync(sql, sqlParams, false, ct).ConfigureAwait(false);

            var rowSet = response.Data.RowSet;

            if (response.Data.Chunks != null && response.Data.Chunks.Count > 0)
            {
                var chunksDownloadInfo = new ChunksDownloadInfo()
                {
                    ChunkHeaders = response.Data.ChunkHeaders,
                    Chunks = response.Data.Chunks,
                    Qrmk = response.Data.Qrmk
                };
                var parsedRowSet = await ChunksDownloader.DownloadAndParseChunksAsync(chunksDownloadInfo, ct).ConfigureAwait(false);
                rowSet.AddRange(parsedRowSet);
            }

            var result = SnowflakeDataMapper.MapTo<T>(response.Data.RowType, rowSet);
            return result;
        }

        /// <summary>
        /// Executes a query, returning the raw data returned by REST API (rows, columns and query information).
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="sqlParams">The parameters to use for this command.</param>
        /// <param name="describeOnly">Return only columns information.</param>
        /// <returns>Rows and columns.</returns>
        public async Task<SnowflakeQueryRawResponse> QueryRawResponseAsync(string sql, object sqlParams = null, bool describeOnly = false, CancellationToken ct = default)
        {
            var response = await QueryInternalAsync(sql, sqlParams, describeOnly, ct).ConfigureAwait(false);

            if (_clientSettings.DownloadChunksForQueryRawResponses
                && response.Data.Chunks != null && response.Data.Chunks.Count > 0)
            {
                var rowSet = response.Data.RowSet;
                var chunksDownloadInfo = new ChunksDownloadInfo()
                {
                    ChunkHeaders = response.Data.ChunkHeaders,
                    Chunks = response.Data.Chunks,
                    Qrmk = response.Data.Qrmk
                };
                var parsedRowSet = await ChunksDownloader.DownloadAndParseChunksAsync(chunksDownloadInfo, ct).ConfigureAwait(false);
                rowSet.AddRange(parsedRowSet);
            }

            return new SnowflakeQueryRawResponse(response.Data);
        }

        /// <summary>
        /// Cancels running query
        /// </summary>
        /// <param name="requestId">Request ID to cancel.</param>
        public async Task<bool> CancelQueryAsync(string requestId, CancellationToken ct = default)
        {
            var cancelQueryRequest = _requestBuilder.BuildCancelQueryRequest(requestId);

            var response = await _restClient.SendAsync<NullDataResponse>(cancelQueryRequest, ct).ConfigureAwait(false);

            if (!response.Success)
                throw new SnowflakeException($"Cancelling query failed. Message: {response.Message}", response.Code);

            return true;
        }

        private async Task<QueryExecResponse> QueryInternalAsync(string sql, object sqlParams = null, bool describeOnly = false, CancellationToken ct = default)
        {
            if (_snowflakeSession == null)
            {
                await InitNewSessionAsync(ct).ConfigureAwait(false);
            }

            var queryRequest = _requestBuilder.BuildQueryRequest(sql, sqlParams, describeOnly);
            var response = await _restClient.SendAsync<QueryExecResponse>(queryRequest, ct).ConfigureAwait(false);

            // Auto renew session, if it's expired
            if (response.Code == 390112)
            {
                await RenewSessionAsync(ct).ConfigureAwait(false);

                // A new instance of HttpQueryRequest should be created for every request
                queryRequest = _requestBuilder.BuildQueryRequest(sql, sqlParams, describeOnly);
                response = await _restClient.SendAsync<QueryExecResponse>(queryRequest, ct).ConfigureAwait(false);
            }

            // If query execution takes more than 45 seconds we will get this
            if (response.Code == 333334 || response.Code == 333333)
            {
                response = await RepeatUntilQueryCompleted(response.Data.GetResultUrl, ct).ConfigureAwait(false);
            }

            if (!response.Success)
                throw new SnowflakeException($"Query execution failed. Message: {response.Message}", response.Code);

            return response;
        }

        private async Task<QueryExecResponse> RepeatUntilQueryCompleted(string getResultUrl, CancellationToken ct = default)
        {
            var lastResultUrl = getResultUrl;
            QueryExecResponse response;
            do
            {
                var getResultRequest = _requestBuilder.BuildGetResultRequest(lastResultUrl);
                response = await _restClient.SendAsync<QueryExecResponse>(getResultRequest, ct).ConfigureAwait(false);

                if (response.Code == 390112)
                {
                    await RenewSessionAsync(ct).ConfigureAwait(false);
                }
                else
                {
                    lastResultUrl = response.Data?.GetResultUrl;
                }
            } while (response.Code == 333334 || response.Code == 333333 || response.Code == 390112);

            return response;
        }

        /// <summary>
        /// Closes current Snowflake session. 
        /// </summary>
        /// <returns>True if session was successfully closed.</returns>
        public async Task<bool> CloseSessionAsync(CancellationToken ct = default)
        {
            var closeSessionRequest = _requestBuilder.BuildCloseSessionRequest();
            var response = await _restClient.SendAsync<CloseResponse>(closeSessionRequest, ct).ConfigureAwait(false);

            _snowflakeSession = null;
            _requestBuilder.ClearSessionTokens();

            if (!response.Success)
                throw new SnowflakeException($"Closing session failed. Message: {response.Message}", response.Code);

            return response.Success;
        }

        /// <summary>
        /// Overrides internal HttpClient
        /// </summary>
        public void SetHttpClient(HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentException("HttpClient cannot be null.");

            _restClient.SetHttpClient(httpClient);
        }
    }
}

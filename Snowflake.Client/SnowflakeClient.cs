using Snowflake.Client.Json;
using Snowflake.Client.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Snowflake.Client
{
    public class SnowflakeClient
    {
        public SnowflakeSession SnowflakeSession { get => session; }
        private SnowflakeSession session;
        private readonly RestClient restClient;
        private readonly RequestBuilder requestBuilder;
        private readonly SnowflakeClientSettings clientSettings;

        public SnowflakeClient(string user, string password, string account, string region = null)
         : this(new AuthInfo(user, password, account, region))
        {
        }

        public SnowflakeClient(AuthInfo authInfo, SessionInfo sessionInfo = null, UrlInfo urlInfo = null, JsonSerializerOptions jsonMapperOptions = null)
        : this(new SnowflakeClientSettings(authInfo))
        {
        }

        public SnowflakeClient(SnowflakeClientSettings settings)
        {
            ValidateClientSettings(settings);

            clientSettings = settings;
            restClient = new RestClient();
            requestBuilder = new RequestBuilder(settings.UrlInfo);

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

        public long ExecuteScalar(string sql, object sqlParams = null)
        {
            var response = QueryInternal(sql, sqlParams);
            var rawResult = response.Data.RowSet[0][0];

            return long.Parse(rawResult);
        }

        public string ExecuteNonQuery(string sql, object sqlParams = null)
        {
            var response = QueryInternal(sql, sqlParams);
            var rawResult = response.Data.RowSet[0][0];

            return rawResult;
        }

        public IEnumerable<T> Query<T>(string sql, object sqlParams = null)
        {
            var response = QueryInternal(sql, sqlParams);
            var result = SnowflakeDataMapper.Map<T>(response.Data);

            return result;
        }

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
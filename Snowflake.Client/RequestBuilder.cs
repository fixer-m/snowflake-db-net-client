using Snowflake.Client.Json;
using Snowflake.Client.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Snowflake.Client
{
    public class RequestBuilder
    {
        private readonly UrlInfo _urlInfo;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ClientAppInfo _clientInfo;
        
        private string _masterToken;
        private string _sessionToken;

        public RequestBuilder(UrlInfo urlInfo)
        {
            this._urlInfo = urlInfo;

            this._jsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };

            this._clientInfo = new ClientAppInfo();
        }

        public void SetSessionTokens(string sessionToken, string masterToken)
        {
            this._sessionToken = sessionToken;
            this._masterToken = masterToken;
        }

        public void ClearSessionTokens()
        {
            this._sessionToken = null;
            this._masterToken = null;
        }

        public HttpRequestMessage BuildLoginRequest(AuthInfo authInfo, SessionInfo sessionInfo)
        {
            var requestUri = BuildLoginUrl(sessionInfo);

            var data = new LoginRequestData()
            {
                LoginName = authInfo.User,
                Password = authInfo.Password,
                AccountName = authInfo.Account,
                ClientAppId = _clientInfo.DriverName,
                ClientAppVersion = _clientInfo.DriverVersion,
                ClientEnvironment = _clientInfo.Environment
            };

            var requestBody = new LoginRequest() { Data = data };
            var jsonBody = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
            var request = BuildJsonRequestMessage(requestUri, HttpMethod.Post, jsonBody);

            return request;
        }

        public HttpRequestMessage BuildRenewSessionRequest()
        {
            var requestUri = BuildRenewSessionUrl();
            var requestBody = new RenewSessionRequest()
            {
                OldSessionToken = _sessionToken,
                RequestType = "RENEW"
            };

            var jsonBody = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
            var request = BuildJsonRequestMessage(requestUri, HttpMethod.Post, jsonBody, true);

            return request;
        }

        public HttpRequestMessage BuildQueryRequest(string sql, object sqlParams, bool describeOnly)
        {
            var queryUri = BuildQueryUrl();

            var requestBody = new QueryRequest()
            {
                SqlText = sql,
                DescribeOnly = describeOnly,
                Bindings = ParameterBinder.BuildParameterBindings(sqlParams)
            };

            var jsonBody = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
            var request = BuildJsonRequestMessage(queryUri, HttpMethod.Post, jsonBody);

            return request;
        }

        public HttpRequestMessage BuildCloseSessionRequest()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams[SnowflakeConst.SF_QUERY_SESSION_DELETE] = "true";
            queryParams[SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString();

            var requestUri = BuildUri(SnowflakeConst.SF_SESSION_PATH, queryParams);
            var request = BuildJsonRequestMessage(requestUri, HttpMethod.Post);

            return request;
        }

        public Uri BuildLoginUrl(SessionInfo sessionInfo)
        {
            var queryParams = new Dictionary<string, string>();
            queryParams[SnowflakeConst.SF_QUERY_WAREHOUSE] = sessionInfo.Warehouse;
            queryParams[SnowflakeConst.SF_QUERY_DB] = sessionInfo.Database;
            queryParams[SnowflakeConst.SF_QUERY_SCHEMA] = sessionInfo.Schema;
            queryParams[SnowflakeConst.SF_QUERY_ROLE] = sessionInfo.Role;
            queryParams[SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString(); // extract to shared part ?

            var loginUrl = BuildUri(SnowflakeConst.SF_LOGIN_PATH, queryParams);
            return loginUrl;
        }

        public Uri BuildRenewSessionUrl()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams[SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString();
            queryParams[SnowflakeConst.SF_QUERY_REQUEST_GUID] = Guid.NewGuid().ToString();

            var url = BuildUri(SnowflakeConst.SF_TOKEN_REQUEST_PATH, queryParams);
            return url;
        }

        private Uri BuildQueryUrl()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams[SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString();

            var loginUrl = BuildUri(SnowflakeConst.SF_QUERY_PATH, queryParams);
            return loginUrl;
        }

        public Uri BuildUri(string basePath, Dictionary<string, string> queryParams = null)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = _urlInfo.Protocol;
            uriBuilder.Host = _urlInfo.Host;
            uriBuilder.Port = _urlInfo.Port;
            uriBuilder.Path = basePath;

            if (queryParams != null && queryParams.Count > 0)
            {
                var paramCollection = HttpUtility.ParseQueryString("");
                foreach (var kvp in queryParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                        paramCollection.Add(kvp.Key, kvp.Value);
                }
                uriBuilder.Query = paramCollection.ToString();
            }

            return uriBuilder.Uri;
        }

        private HttpRequestMessage BuildJsonRequestMessage(Uri uri, HttpMethod method, string jsonBody = null, bool useMasterToken = false)
        {
            var request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = uri;

            if (jsonBody != null && method != HttpMethod.Get)
            {
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            if (_sessionToken != null)
            {
                var authToken = useMasterToken ? _masterToken : _sessionToken;
                request.Headers.Add("Authorization", $"Snowflake Token=\"{authToken}\"");
            }

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/snowflake"));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(_clientInfo.DriverName, _clientInfo.DriverVersion));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(_clientInfo.Environment.OSVersion));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(_clientInfo.Environment.NETRuntime, _clientInfo.Environment.NETVersion));

            return request;
        }
    }
}
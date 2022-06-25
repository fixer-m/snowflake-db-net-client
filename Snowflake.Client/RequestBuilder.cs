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
    internal class RequestBuilder
    {
        private readonly UrlInfo _urlInfo;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ClientAppInfo _clientInfo;

        private string _masterToken;
        private string _sessionToken;

        internal RequestBuilder(UrlInfo urlInfo)
        {
            _urlInfo = urlInfo;

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };

            _clientInfo = new ClientAppInfo();
        }

        internal void SetSessionTokens(string sessionToken, string masterToken)
        {
            _sessionToken = sessionToken;
            _masterToken = masterToken;
        }

        internal void ClearSessionTokens()
        {
            _sessionToken = null;
            _masterToken = null;
        }

        internal HttpRequestMessage BuildLoginRequest(AuthInfo authInfo, SessionInfo sessionInfo)
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

        internal HttpRequestMessage BuildCancelQueryRequest(string requestId)
        {
            var requestUri = BuildCancelQueryUrl();
            var requestBody = new CancelQueryRequest()
            {
                RequestId = requestId
            };

            var jsonBody = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
            var request = BuildJsonRequestMessage(requestUri, HttpMethod.Post, jsonBody);

            return request;
        }

        internal HttpRequestMessage BuildRenewSessionRequest()
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

        internal HttpRequestMessage BuildQueryRequest(string sql, object sqlParams, bool describeOnly)
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

        internal HttpRequestMessage BuildCloseSessionRequest()
        {
            var queryParams = new Dictionary<string, string>();
            queryParams[SnowflakeConst.SF_QUERY_SESSION_DELETE] = "true";
            queryParams[SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString();

            var requestUri = BuildUri(SnowflakeConst.SF_SESSION_PATH, queryParams);
            var request = BuildJsonRequestMessage(requestUri, HttpMethod.Post);

            return request;
        }

        internal HttpRequestMessage BuildGetResultRequest(string getResultUrl)
        {
            var queryUri = BuildUri(getResultUrl);
            var request = BuildJsonRequestMessage(queryUri, HttpMethod.Get);

            return request;
        }

        internal Uri BuildLoginUrl(SessionInfo sessionInfo)
        {
            var queryParams = new Dictionary<string, string>
            {
                [SnowflakeConst.SF_QUERY_WAREHOUSE] = sessionInfo.Warehouse,
                [SnowflakeConst.SF_QUERY_DB] = sessionInfo.Database,
                [SnowflakeConst.SF_QUERY_SCHEMA] = sessionInfo.Schema,
                [SnowflakeConst.SF_QUERY_ROLE] = sessionInfo.Role,
                [SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString() // extract to shared part ?
            };

            var loginUrl = BuildUri(SnowflakeConst.SF_LOGIN_PATH, queryParams);
            return loginUrl;
        }

        internal Uri BuildCancelQueryUrl()
        {
            var queryParams = new Dictionary<string, string>
            {
                [SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString(),
                [SnowflakeConst.SF_QUERY_REQUEST_GUID] = Guid.NewGuid().ToString()
            };

            var url = BuildUri(SnowflakeConst.SF_QUERY_CANCEL_PATH, queryParams);
            return url;
        }

        internal Uri BuildRenewSessionUrl()
        {
            var queryParams = new Dictionary<string, string>
            {
                [SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString(),
                [SnowflakeConst.SF_QUERY_REQUEST_GUID] = Guid.NewGuid().ToString()
            };

            var url = BuildUri(SnowflakeConst.SF_TOKEN_REQUEST_PATH, queryParams);
            return url;
        }

        private Uri BuildQueryUrl()
        {
            var queryParams = new Dictionary<string, string>
            {
                [SnowflakeConst.SF_QUERY_REQUEST_ID] = Guid.NewGuid().ToString()
            };

            var loginUrl = BuildUri(SnowflakeConst.SF_QUERY_PATH, queryParams);
            return loginUrl;
        }

        internal Uri BuildUri(string basePath, Dictionary<string, string> queryParams = null)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = _urlInfo.Protocol,
                Host = _urlInfo.Host,
                Port = _urlInfo.Port,
                Path = basePath
            };

            if (queryParams != null && queryParams.Count > 0)
            {
                var paramCollection = HttpUtility.ParseQueryString("");
                foreach (var kvp in queryParams)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                        paramCollection.Add(kvp.Key, kvp.Value);
                }
                uriBuilder.Query = paramCollection.ToString() ?? "";
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
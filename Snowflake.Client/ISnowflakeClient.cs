using Snowflake.Client.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public interface ISnowflakeClient
    {
        Task<bool> CancelQueryAsync(string requestId);
        Task<bool> CloseSessionAsync();
        Task<long> ExecuteAsync(string sql, object sqlParams = null);
        Task<string> ExecuteScalarAsync(string sql, object sqlParams = null);
        Task<bool> InitNewSessionAsync();
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object sqlParams = null);
        Task<SnowflakeQueryRawResponse> QueryRawResponseAsync(string sql, object sqlParams = null, bool describeOnly = false);
        Task<bool> RenewSessionAsync();
        void SetHttpClient(HttpClient httpClient);
    }
}
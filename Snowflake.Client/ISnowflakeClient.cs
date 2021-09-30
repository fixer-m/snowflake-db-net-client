using Snowflake.Client.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public interface ISnowflakeClient
    {
        Task<bool> CancelQueryAsync(string requestId, CancellationToken ct = default);
        Task<bool> CloseSessionAsync(CancellationToken ct = default);
        Task<long> ExecuteAsync(string sql, object sqlParams = null, CancellationToken ct = default);
        Task<string> ExecuteScalarAsync(string sql, object sqlParams = null, CancellationToken ct = default);
        Task<bool> InitNewSessionAsync(CancellationToken ct = default);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object sqlParams = null, CancellationToken ct = default);
        Task<SnowflakeQueryRawResponse> QueryRawResponseAsync(string sql, object sqlParams = null, bool describeOnly = false, CancellationToken ct = default);
        Task<bool> RenewSessionAsync(CancellationToken ct = default);
        void SetHttpClient(HttpClient httpClient);
    }
}
using Snowflake.Client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public interface ISnowflakeClient
    {
        SnowflakeSession SnowflakeSession { get; }

        Task<bool> CloseSessionAsync();
        Task<long> ExecuteAsync(string sql, object sqlParams = null);
        Task<string> ExecuteScalarAsync(string sql, object sqlParams = null);
        Task<bool> InitNewSessionAsync();
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object sqlParams = null);
        Task<SnowflakeQueryRawResponse> QueryRawResponseAsync(string sql, object sqlParams = null, bool describeOnly = false);
    }
}
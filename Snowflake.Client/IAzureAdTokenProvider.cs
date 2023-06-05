using System.Threading;
using System.Threading.Tasks;
using Snowflake.Client.Model;

namespace Snowflake.Client
{
    public interface IAzureAdTokenProvider
        {
            Task<string> GetAzureAdAccessTokenAsync(AzureAdAuthInfo authInfo, CancellationToken ct = default);
    }
}
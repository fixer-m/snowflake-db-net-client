using Microsoft.Identity.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Snowflake.Client.Model;

namespace Snowflake.Client 
{
    public class AzureAdTokenProvider : IAzureAdTokenProvider
    {
        public async Task<string> GetAzureAdAccessTokenAsync(AzureAdAuthInfo authInfo, CancellationToken ct = default)
        {
            try
            {
                if (authInfo.ClientId == null || authInfo.ClientSecret == null || authInfo.ServicePrincipalObjectId == null || authInfo.TenantId == null || authInfo.Scope == null)
                {
                    throw new SnowflakeException("Error: One or more required environment variables are missing.", 400);
                }

                return await GetAccessTokenAsync(authInfo.ClientId, authInfo.ClientSecret, authInfo.ServicePrincipalObjectId, authInfo.TenantId, authInfo.Scope);
            }
            catch (Exception ex)
            {
                throw new SnowflakeException($"Failed getting the Azure Token. Message: {ex.Message}", ex);
            }
        }

        private async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string servicePrincipalObjectId, string tenantId, string scope)
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}/"))
                .Build();

            var scopes = new[] { scope };

            AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }
    }
}
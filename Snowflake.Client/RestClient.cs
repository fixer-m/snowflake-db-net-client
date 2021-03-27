using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public class RestClient
    {
        private HttpClient httpClient;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public void SetHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public RestClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12,
                CheckCertificateRevocationList = true,
            };

            httpClient = new HttpClient(httpClientHandler);

            jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        }

        [Obsolete]
        public T Send<T>(HttpRequestMessage request)
        {
            var response = httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        }
    }
}
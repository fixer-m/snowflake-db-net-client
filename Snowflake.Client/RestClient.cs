using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    internal class RestClient
    {
        private HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        internal RestClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                UseCookies = false,
                SslProtocols = SslProtocols.Tls12,
                CheckCertificateRevocationList = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromHours(1)
            };

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        internal void SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken ct)
        {
            SetServicePointOptions(request.RequestUri);

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        }

        private static void SetServicePointOptions(Uri requestUri)
        {
            var point = ServicePointManager.FindServicePoint(requestUri);

            point.Expect100Continue = false;
            point.UseNagleAlgorithm = false;
            point.ConnectionLimit = 20;
        }
    }
}
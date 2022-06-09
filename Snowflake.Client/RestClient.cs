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

        internal RestClient(bool sslBypass = false)
        {
            _httpClient = !sslBypass
                ? new HttpClient(new HttpClientHandler
                {
                    SslProtocols = SslProtocols.Tls12,
                    CheckCertificateRevocationList = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                })
                : new HttpClient(new HttpClientHandler
                {
                    SslProtocols = SslProtocols.Tls12,
                    CheckCertificateRevocationList = false,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                });

            _jsonSerializerOptions = new JsonSerializerOptions
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

        [Obsolete]
        internal T Send<T>(HttpRequestMessage request)
        {
            SetServicePointOptions(request.RequestUri);

            var response = _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        }

        private void SetServicePointOptions(Uri requestUri)
        {
            var point = ServicePointManager.FindServicePoint(requestUri);

            point.Expect100Continue = false;
            point.UseNagleAlgorithm = false;
            point.ConnectionLimit = 20;
        }
    }
}

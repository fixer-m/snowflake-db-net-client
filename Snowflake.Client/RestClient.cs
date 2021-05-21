using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public class RestClient
    {
        private HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public RestClient()
        {
            var httpClientHandler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12,
                CheckCertificateRevocationList = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(httpClientHandler);

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public void SetHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            SetServicePointOptions(request.RequestUri);

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
        }

        [Obsolete]
        public T Send<T>(HttpRequestMessage request)
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
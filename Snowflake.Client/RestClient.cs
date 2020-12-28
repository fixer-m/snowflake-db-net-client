using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Snowflake.Client
{
    public class RestClient
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public RestClient()
        {
            httpClient = new HttpClient();

            jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public T Send<T>(HttpRequestMessage request)
        {
            var response = httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;

            return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
        }
    }
}
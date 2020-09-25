using System.Net.Http;
using System.Text.Json;

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
    }
}
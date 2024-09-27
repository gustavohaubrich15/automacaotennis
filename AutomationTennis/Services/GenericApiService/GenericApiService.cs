using System.Text;
using System.Text.Json;

namespace AutomationTennis.Services.GenericApiService
{
    public class GenericApiService : IGenericApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GenericApiService> _logger;

        public GenericApiService(HttpClient httpClient,
            ILogger<GenericApiService> logger)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36");
            _logger = logger;
        }

        // Generic GET method
        public async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<TResponse>(content, options);
        }

        // Generic POST method
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest requestBody)
        {
            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)responseContent;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<TResponse>(responseContent, options);
        }
    }
}

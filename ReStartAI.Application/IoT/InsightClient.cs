using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ReStartAI.Application.IoT
{
    public class InsightClient
    {
        private readonly HttpClient _http;
        private readonly string _internalKey;
        private readonly JsonSerializerOptions _jsonOptions;

        public InsightClient(IConfiguration configuration, HttpClient http)
        {
            _http = http;
            _internalKey = configuration["IoT:InternalKey"] ?? string.Empty;
            _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            var baseUrl = configuration["IoT:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                _http.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<InsightResponseDto> GetInsightAsync(
            InsightRequestDto payload,
            CancellationToken cancellationToken = default)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, "/insight");
            message.Headers.Add("X-Internal-Key", _internalKey);

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _http.SendAsync(message, cancellationToken);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<InsightResponseDto>(body, _jsonOptions);

            if (result is null)
            {
                throw new InvalidOperationException("Resposta vazia do serviço de IoT.");
            }

            return result;
        }
    }
}
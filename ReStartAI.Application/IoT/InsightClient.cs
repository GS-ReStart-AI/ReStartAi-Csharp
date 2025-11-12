using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ReStartAI.Application.IoT;

public class InsightClient
{
    private readonly HttpClient _http;
    private readonly string _internalKey;

    public InsightClient(IConfiguration cfg, HttpClient http)
    {
        _http = http;
        _internalKey = cfg["IoT:InternalKey"] ?? string.Empty;

        var baseUrl = cfg["IoT:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
            _http.BaseAddress = new Uri(baseUrl);
    }

    public async Task<InsightResponseDto> GetInsightAsync(InsightRequestDto payload, CancellationToken ct = default)
    {
        using var msg = new HttpRequestMessage(HttpMethod.Post, "/insight");
        msg.Headers.Add("X-Internal-Key", _internalKey);
        msg.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(msg, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"IoT returned {(int)resp.StatusCode}: {body}");

        var dto = JsonSerializer.Deserialize<InsightResponseDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (dto is null || string.IsNullOrWhiteSpace(dto.insight) || string.IsNullOrWhiteSpace(dto.actionTag))
            throw new InvalidOperationException("Invalid IoT response");

        return dto;
    }
}
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ReStartAI.Application.IoT;

public record MetricsDto(int jobsViewedToday, int applyClicksToday, DateTime? lastEventAt);
public record ProfileDto(List<string> areas, List<string> roles, string? city, List<string>? gaps);
public record BestOpportunityDto(string? role, string? city, double? match, string? missingSkill);
public record InsightRequestDto(string userId, MetricsDto metrics, List<string> lastEvents, ProfileDto profile, BestOpportunityDto? bestOpportunity);
public record InsightResponseDto(string insight, string actionTag);

public class InsightClient
{
    private readonly IConfiguration _cfg;
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public InsightClient(IConfiguration cfg, HttpClient http)
    {
        _cfg = cfg;
        _http = http;
    }

    public async Task<InsightResponseDto> GetInsightAsync(InsightRequestDto payload, CancellationToken ct = default)
    {
        var baseUrl = _cfg["IoT:BaseUrl"] ?? "http://localhost:8000";
        var key = _cfg["IoT:InternalKey"] ?? "";
        var url = $"{baseUrl.TrimEnd('/')}/insight";

        using var msg = new HttpRequestMessage(HttpMethod.Post, url);
        msg.Headers.Add("X-Internal-Key", key);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        msg.Content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(msg, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            return new InsightResponseDto("Explore novas vagas e refine seu objetivo agora.", "explore");

        var parsed = JsonSerializer.Deserialize<InsightResponseDto>(body, _json);
        if (parsed is null || string.IsNullOrWhiteSpace(parsed.insight) || string.IsNullOrWhiteSpace(parsed.actionTag))
            return new InsightResponseDto("Explore novas vagas e refine seu objetivo agora.", "explore");

        if (parsed.insight.Length > 120)
            parsed = parsed with { insight = parsed.insight[..120] };

        return parsed;
    }
}

namespace ReStartAI.Application.IoT;

public record MetricsDto(int jobsViewedToday, int applyClicksToday, DateTime? lastEventAt);
public record ProfileDto(IEnumerable<string> areas, IEnumerable<string> roles, string? city, IEnumerable<string>? gaps);
public record BestOpportunityDto(string role, string? city, double match, string? missingSkill);

public record InsightRequestDto(
    string userId,
    MetricsDto metrics,
    IEnumerable<string> lastEvents,
    ProfileDto profile,
    BestOpportunityDto? bestOpportunity
);

public record InsightResponseDto(string insight, string actionTag);
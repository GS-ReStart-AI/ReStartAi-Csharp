namespace ReStartAI.Application.IoT
{
    public record MetricsDto(
        int JobsViewed,
        int ApplyClicks,
        DateTime? LastAt
    );

    public record ProfileDto(
        List<string> Areas,
        List<string> PapeisSugeridos,
        string? City,
        string? Gaps
    );

    public record BestOpportunityDto(
        string? Role,
        string? City,
        double Match,
        string? MissingSkill
    );

    public record InsightRequestDto(
        string UserId,
        MetricsDto Metrics,
        List<string> LastEvents,
        ProfileDto Profile,
        BestOpportunityDto? BestOpportunity
    );
}
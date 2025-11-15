using System;
using System.Collections.Generic;

namespace ReStartAI.Application.IoT
{
    public record MetricsDto(
        int JobsViewedToday,
        int ApplyClicksToday,
        DateTime? LastEventAt
    );

    public record EventDto(
        string Type,
        DateTime Ts
    );

    public record ProfileDto(
        List<string> Areas,
        List<string> Roles,
        string? City,
        List<string> Gaps
    );

    public record BestOpportunityDto(
        string? Role,
        string? City,
        int? Match,
        string? MissingSkill
    );

    public record InsightRequestDto(
        string UserId,
        MetricsDto Metrics,
        List<EventDto> LastEvents,
        ProfileDto Profile,
        BestOpportunityDto? BestOpportunity
    );
}
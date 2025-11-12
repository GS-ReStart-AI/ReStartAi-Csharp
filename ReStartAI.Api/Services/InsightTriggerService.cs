using ReStartAI.Application.IoT;
using ReStartAI.Application.Matching;
using ReStartAI.Application.Parsing;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Services;

public class InsightTriggerService
{
    private readonly IAppEventRepository _events;
    private readonly ICurriculoRepository _curriculos;
    private readonly IVagaRepository _vagas;

    public InsightTriggerService(IAppEventRepository events, ICurriculoRepository curriculos, IVagaRepository vagas)
    {
        _events = events;
        _curriculos = curriculos;
        _vagas = vagas;
    }

    public async Task<InsightRequestDto?> BuildAsync(string userId)
    {
        var (jobs, apply, lastAt) = await _events.GetTodayMetricsAsync(userId);
        var recent = await _events.GetLastAsync(userId, 10);
        var lastEvents = recent.Select(e => e.Type).ToList();

        var curr = await _curriculos.GetLastByUserAsync(userId);
        if (curr is null) return null;

        var parser = new ResumeParser();
        var parsed = parser.Parse(string.Join(' ', curr.Skills));

        var vagas = await _vagas.GetAtivasAsync();
        var matcher = new DeterministicMatcher();
        var best = matcher.BestMatch(vagas, curr.Skills);

        BestOpportunityDto? bestOp = null;
        if (best is not null)
        {
            bestOp = new BestOpportunityDto(
                role: best.Vaga.Titulo,
                city: null,
                match: best.Percentual,
                missingSkill: best.MustMissing.FirstOrDefault()
            );
        }

        var metrics = new MetricsDto(jobs, apply, lastAt);
        var profile = new ProfileDto(parsed.Areas, parsed.PapeisSugeridos, null, null);

        return new InsightRequestDto(userId, metrics, lastEvents, profile, bestOp);
    }
}
using ReStartAI.Application.IoT;
using ReStartAI.Application.Matching;
using ReStartAI.Application.Parsing;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Application.Services
{
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
            var recent = await _events.GetAllAsync(1, 10);
            var lastEvents = recent
                .Where(e => e.UsuarioId == userId)
                .OrderByDescending(e => e.TimestampUtc)
                .Take(10)
                .Select(e => e.Tipo)
                .ToList();

            var curriculosUsuario = await _curriculos.GetAllAsync(1, 10);
            var curr = curriculosUsuario.FirstOrDefault(c => c.UsuarioId == userId);
            if (curr is null) return null;

            var parser = new ResumeParser();
            var parsed = parser.Parse(curr.Texto);

            var vagas = await _vagas.GetAllAsync(1, 50);
            var matcher = new DeterministicMatcher();
            var best = matcher.BestMatch(vagas, parsed.Skills);

            BestOpportunityDto? bestOp = null;
            if (best is not null)
            {
                bestOp = new BestOpportunityDto(
                    Role: best.Vaga.Titulo,
                    City: best.Vaga.Cidade,
                    Match: best.Percentual,
                    MissingSkill: best.MustMissing.FirstOrDefault()
                );
            }

            var metrics = new MetricsDto(
                JobsViewed: recent.Count(e => e.Tipo == "view"),
                ApplyClicks: recent.Count(e => e.Tipo == "apply"),
                LastAt: recent.OrderByDescending(e => e.TimestampUtc).FirstOrDefault()?.TimestampUtc
            );

            var profile = new ProfileDto(
                parsed.Areas,
                parsed.PapeisSugeridos,
                null,
                null
            );

            return new InsightRequestDto(userId, metrics, lastEvents, profile, bestOp);
        }
    }
}

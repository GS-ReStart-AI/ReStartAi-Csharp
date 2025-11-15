using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;

namespace ReStartAI.Application.IoT
{
    public class InsightTriggerService
    {
        private readonly CurriculoService _curriculoService;
        private readonly InsightClient _client;

        public InsightTriggerService(
            CurriculoService curriculoService,
            InsightClient client)
        {
            _curriculoService = curriculoService;
            _client = client;
        }

        public async Task<InsightResponseDto?> GenerateInsightForUserAsync(
            string usuarioId,
            CancellationToken cancellationToken = default)
        {
            var page = 1;
            var pageSize = 100;

            var curriculos = await _curriculoService.GetAllAsync(page, pageSize);
            var curriculo = curriculos.FirstOrDefault(c => c.UsuarioId == usuarioId);

            if (curriculo is null)
                return null;

            var metrics = new MetricsDto(
                JobsViewedToday: 0,
                ApplyClicksToday: 0,
                LastEventAt: null
            );

            var lastEvents = new List<EventDto>();

            var areas = new List<string>();
            var roles = new List<string>();
            var gaps = new List<string>();

            if (curriculo.Skills is not null && curriculo.Skills.Count > 0)
            {
                areas.Add("Back-end .NET");
                roles.Add("Desenvolvedor .NET");
            }

            var profile = new ProfileDto(
                Areas: areas,
                Roles: roles,
                City: null,
                Gaps: gaps
            );

            BestOpportunityDto? bestOpportunity = null;

            var request = new InsightRequestDto(
                UserId: usuarioId,
                Metrics: metrics,
                LastEvents: lastEvents,
                Profile: profile,
                BestOpportunity: bestOpportunity
            );

            var response = await _client.GetInsightAsync(request, cancellationToken);
            return response;
        }
    }
}

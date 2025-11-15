using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Api.Swagger.Examples.Career;
using ReStartAI.Application.IoT;
using ReStartAI.Application.Services;
using ReStartAI.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [ApiKeyAuth]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CareerController : ControllerBase
    {
        public record TriggerRequest(
            string UsuarioId,
            MetricsDto Metrics,
            List<EventDto> LastEvents,
            ProfileDto Profile,
            BestOpportunityDto? BestOpportunity
        );

        public record TriggerResponse(
            string Insight,
            string ActionTag
        );

        private readonly InsightClient _insightsClient;
        private readonly NotificacaoService _notificacoes;

        public CareerController(
            InsightClient insightsClient,
            NotificacaoService notificacoes)
        {
            _insightsClient = insightsClient;
            _notificacoes = notificacoes;
        }

        [HttpPost("trigger")]
        [SwaggerOperation(
            Summary = "Gera um insight de carreira",
            Description = "Recebe métricas, perfil e melhor oportunidade e retorna um insight curto gerado pela IA."
        )]
        [SwaggerRequestExample(typeof(TriggerRequest), typeof(TriggerRequestExample))]
        [ProducesResponseType(typeof(TriggerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TriggerResponse>> Trigger(
            [FromBody] TriggerRequest req,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(req.UsuarioId))
                return BadRequest(new { message = "usuarioId é obrigatório" });

            var payload = new InsightRequestDto(
                UserId: req.UsuarioId,
                Metrics: req.Metrics,
                LastEvents: req.LastEvents,
                Profile: req.Profile,
                BestOpportunity: req.BestOpportunity
            );

            var result = await _insightsClient.GetInsightAsync(payload, cancellationToken);

            var titulo = result.actionTag switch
            {
                "apply" => "Foque em candidaturas com alto fit",
                "explore" => "Explore novas oportunidades sugeridas",
                "study" => "Invista nos seus gaps para evoluir",
                _ => "Novo insight sobre sua jornada"
            };

            var notificacao = new Notificacao
            {
                UsuarioId = req.UsuarioId,
                Titulo = titulo,
                Mensagem = result.insight
            };

            await _notificacoes.CreateAsync(notificacao);

            var response = new TriggerResponse(
                Insight: result.insight,
                ActionTag: result.actionTag
            );

            return Ok(response);
        }
    }
}

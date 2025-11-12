using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.IoT;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [Route("api/insights")]
    [Authorize]
    public class InsightsController : ControllerBase
    {
        private readonly InsightClient _client;
        private readonly INotificacaoRepository _notifications;

        public InsightsController(InsightClient client, INotificacaoRepository notifications)
        {
            _client = client;
            _notifications = notifications;
        }

        public record TriggerRequest(List<string> LastEvents, MetricsDto Metrics, ProfileDto Profile, BestOpportunityDto? BestOpportunity);
        public record TriggerResponse(string NotificationId, string Insight, string ActionTag);

        [HttpPost("trigger")]
        public async Task<ActionResult<TriggerResponse>> Trigger([FromBody] TriggerRequest req)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            var payload = new InsightRequestDto(uid, req.Metrics, req.LastEvents ?? new List<string>(), req.Profile, req.BestOpportunity);
            var result = await _client.GetInsightAsync(payload, HttpContext.RequestAborted);

            var titulo = result.actionTag switch
            {
                "apply" => "Hora de aplicar",
                "study" => "Foque no seu gap",
                _ => "Explore seu caminho"
            };

            var note = new Notificacao
            {
                UsuarioId = uid,
                Titulo = titulo,
                Mensagem = result.insight
            };

            await _notifications.CreateAsync(note);

            return Ok(new TriggerResponse(note.Id!, result.insight, result.actionTag));
        }
    }
}
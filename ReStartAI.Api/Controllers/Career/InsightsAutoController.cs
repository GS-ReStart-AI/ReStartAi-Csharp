using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Services;
using ReStartAI.Application.IoT;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [Route("api/insights")]
    [Authorize]
    public class InsightsAutoController : ControllerBase
    {
        private readonly InsightTriggerService _builder;
        private readonly InsightClient _client;
        private readonly INotificacaoRepository _notifications;

        public InsightsAutoController(InsightTriggerService builder, InsightClient client, INotificacaoRepository notifications)
        {
            _builder = builder;
            _client = client;
            _notifications = notifications;
        }

        public record AutoTriggerResponse(string NotificationId, string Insight, string ActionTag);

        [HttpPost("auto")]
        public async Task<ActionResult<AutoTriggerResponse>> Auto()
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            var payload = await _builder.BuildAsync(uid);
            if (payload is null) return NotFound();

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

            return Ok(new AutoTriggerResponse(note.Id!, result.insight, result.actionTag));
        }
    }
}
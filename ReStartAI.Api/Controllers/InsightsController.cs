using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.IoT;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/insights")]
[Authorize]
public class InsightsController : ControllerBase
{
    private readonly InsightClient _client;
    private readonly INotificationRepository _notifications;

    public InsightsController(InsightClient client, INotificationRepository notifications)
    {
        _client = client;
        _notifications = notifications;
    }

    public record TriggerRequest(List<string> lastEvents, MetricsDto metrics, ProfileDto profile, BestOpportunityDto? bestOpportunity);
    public record TriggerResponse(string notificationId, string insight, string actionTag);

    [HttpPost("trigger")]
    public async Task<ActionResult<TriggerResponse>> Trigger([FromBody] TriggerRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var payload = new InsightRequestDto(uid, req.metrics, req.lastEvents ?? new List<string>(), req.profile, req.bestOpportunity);
        var result = await _client.GetInsightAsync(payload, HttpContext.RequestAborted);

        var titulo = result.actionTag switch
        {
            "apply" => "Hora de aplicar",
            "study" => "Foque no seu gap",
            _ => "Explore seu caminho"
        };

        var note = new Notification
        {
            UserId = uid,
            Titulo = titulo,
            Mensagem = result.insight,
            Lido = false,
            CriadoEm = DateTime.UtcNow
        };

        await _notifications.InsertAsync(note);

        return Ok(new TriggerResponse(note.Id!, result.insight, result.actionTag));
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Services;
using ReStartAI.Application.IoT;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/insights")]
[Authorize]
public class InsightsAutoController : ControllerBase
{
    private readonly InsightTriggerService _builder;
    private readonly InsightClient _client;
    private readonly INotificationRepository _notifications;

    public InsightsAutoController(InsightTriggerService builder, InsightClient client, INotificationRepository notifications)
    {
        _builder = builder;
        _client = client;
        _notifications = notifications;
    }

    public record AutoTriggerResponse(string notificationId, string insight, string actionTag);

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

        var note = new Notification
        {
            UserId = uid,
            Titulo = titulo,
            Mensagem = result.insight,
            Lido = false,
            CriadoEm = DateTime.UtcNow
        };

        await _notifications.InsertAsync(note);

        return Ok(new AutoTriggerResponse(note.Id!, result.insight, result.actionTag));
    }
}
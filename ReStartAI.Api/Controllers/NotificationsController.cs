using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repo;

    public NotificationsController(INotificationRepository repo)
    {
        _repo = repo;
    }

    public record NotificationItem(string Id, string Titulo, string Mensagem, bool Lido, DateTime CriadoEm);

    [HttpGet]
    public async Task<ActionResult<List<NotificationItem>>> Get([FromQuery] bool? lido = null)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        var list = await _repo.GetByUserAsync(uid, lido);
        var result = list.Select(n => new NotificationItem(n.Id!, n.Titulo, n.Mensagem, n.Lido, n.CriadoEm)).ToList();
        return Ok(result);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(string id)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        await _repo.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        await _repo.MarkAllAsReadAsync(uid);
        return NoContent();
    }
}
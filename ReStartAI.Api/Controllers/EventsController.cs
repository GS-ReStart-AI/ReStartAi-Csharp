using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IAppEventRepository _repo;

    public EventsController(IAppEventRepository repo)
    {
        _repo = repo;
    }

    public record PostEventRequest(string Type, Dictionary<string, object>? Metadata);
    public record PostEventResponse(string Id, int JobsViewedToday, int ApplyClicksToday, DateTime? LastEventAt);
    public record EventItem(string Id, string Type, DateTime TimestampUtc);

    [HttpPost]
    public async Task<ActionResult<PostEventResponse>> Post([FromBody] PostEventRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(req.Type)) return BadRequest();

        var e = new AppEvent
        {
            UserId = uid,
            Type = req.Type.Trim().ToLowerInvariant(),
            TimestampUtc = DateTime.UtcNow,
            Metadata = req.Metadata
        };

        await _repo.InsertAsync(e);

        var (jobs, apply, lastAt) = await _repo.GetTodayMetricsAsync(uid);
        return Accepted(new PostEventResponse(e.Id!, jobs, apply, lastAt));
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<EventItem>>> GetMine([FromQuery] int limit = 10)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        limit = Math.Clamp(limit, 1, 50);
        var list = await _repo.GetLastAsync(uid, limit);
        var result = list.Select(x => new EventItem(x.Id!, x.Type, x.TimestampUtc)).ToList();
        return Ok(result);
    }
}
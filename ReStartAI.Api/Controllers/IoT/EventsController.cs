using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Swagger.Examples.Events;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers.IoT
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    [Produces("application/json")]
    public class EventsController : ControllerBase
    {
        private readonly IAppEventRepository _repo;

        public EventsController(IAppEventRepository repo)
        {
            _repo = repo;
        }

        public record PostEventRequest(string Tipo, Dictionary<string, object>? Metadata);
        public record PostEventResponse(string Id, int JobsViewedToday, int ApplyClicksToday, DateTime? LastEventAt);
        public record EventItem(string Id, string Tipo, DateTime TimestampUtc);

        [HttpPost]
        [SwaggerOperation(
            Summary = "Registra um evento IoT do app",
            Description = "Registra um evento de comportamento do usuário no app (ex.: job_view, apply_click) e retorna métricas agregadas do dia."
        )]
        [SwaggerRequestExample(typeof(PostEventRequest), typeof(PostEventRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status202Accepted, typeof(PostEventResponseExample))]
        public async Task<ActionResult<PostEventResponse>> Post([FromBody] PostEventRequest req)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized();
            if (string.IsNullOrWhiteSpace(req.Tipo)) return BadRequest();

            var e = new AppEvent
            {
                UsuarioId = uid,
                Tipo = req.Tipo.Trim().ToLowerInvariant(),
                TimestampUtc = DateTime.UtcNow,
                Metadata = req.Metadata
            };

            await _repo.CreateAsync(e);

            var eventos = await _repo.GetAllAsync(1, 50);
            var userEvents = eventos.Where(x => x.UsuarioId == uid).ToList();

            var jobs = userEvents.Count(x => x.Tipo.Contains("view"));
            var apply = userEvents.Count(x => x.Tipo.Contains("apply"));
            var lastAt = userEvents
                .OrderByDescending(x => x.TimestampUtc)
                .FirstOrDefault()
                ?.TimestampUtc;

            return Accepted(new PostEventResponse(e.Id!, jobs, apply, lastAt));
        }

        [HttpGet("me")]
        [SwaggerOperation(
            Summary = "Lista eventos recentes do usuário autenticado",
            Description = "Retorna os eventos IoT mais recentes do usuário autenticado, ordenados do mais recente para o mais antigo."
        )]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EventItemListResponseExample))]
        public async Task<ActionResult<List<EventItem>>> GetMine([FromQuery] int limit = 10)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            var eventos = await _repo.GetAllAsync(1, limit);
            var userEvents = eventos.Where(x => x.UsuarioId == uid)
                .OrderByDescending(x => x.TimestampUtc)
                .Take(limit)
                .ToList();

            var result = userEvents
                .Select(x => new EventItem(x.Id!, x.Tipo, x.TimestampUtc))
                .ToList();

            return Ok(result);
        }
    }
}

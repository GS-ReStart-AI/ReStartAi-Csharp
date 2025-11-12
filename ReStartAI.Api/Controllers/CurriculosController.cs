using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Parsing;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CurriculosController : ControllerBase
{
    private readonly ICurriculoRepository _curriculos;

    public CurriculosController(ICurriculoRepository curriculos)
    {
        _curriculos = curriculos;
    }

    public record PostTextoRequest(string Texto);
    public record PostTextoResponse(string CurriculoId, string? Nome, string? Email, string? Telefone, List<string> Skills, List<string> Areas, List<string> PapeisSugeridos);
    public record CurriculoItem(string Id, DateTime CriadoEm, List<string> Skills);

    [HttpPost("texto")]
    public async Task<ActionResult<PostTextoResponse>> PostTexto([FromBody] PostTextoRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(req.Texto)) return BadRequest();

        var parser = new ResumeParser();
        var parsed = parser.Parse(req.Texto);

        var c = new Curriculo
        {
            UserId = uid,
            TextoOriginal = req.Texto,
            TextoExtraido = req.Texto,
            Skills = parsed.Skills,
            CriadoEm = DateTime.UtcNow
        };

        await _curriculos.InsertAsync(c);

        return Created($"api/curriculos/{c.Id}", new PostTextoResponse(c.Id!, parsed.Nome, parsed.Email, parsed.Telefone, parsed.Skills, parsed.Areas, parsed.PapeisSugeridos));
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<CurriculoItem>>> GetMyCurriculos()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
        var list = await _curriculos.GetByUserAsync(uid);
        var result = list.OrderByDescending(x => x.CriadoEm).Select(x => new CurriculoItem(x.Id!, x.CriadoEm, x.Skills)).ToList();
        return Ok(result);
    }
}

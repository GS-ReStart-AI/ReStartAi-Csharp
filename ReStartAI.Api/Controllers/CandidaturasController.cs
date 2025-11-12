using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidaturasController : ControllerBase
{
    private readonly ICandidaturaRepository _repo;
    private readonly IVagaRepository _vagas;

    public CandidaturasController(ICandidaturaRepository repo, IVagaRepository vagas)
    {
        _repo = repo;
        _vagas = vagas;
    }

    public record CandidaturaResponse(string Id, string VagaId, string VagaTitulo, DateTime CriadoEm);

    [HttpGet("me")]
    public async Task<ActionResult<List<CandidaturaResponse>>> GetMine()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var list = await _repo.GetByUserAsync(uid);
        var result = new List<CandidaturaResponse>();
        foreach (var c in list.OrderByDescending(x => x.CriadoEm))
        {
            var v = await _vagas.GetByIdAsync(c.VagaId);
            result.Add(new CandidaturaResponse(c.Id!, c.VagaId, v?.Titulo ?? "Vaga", c.CriadoEm));
        }
        return Ok(result);
    }

    [HttpPost("{vagaId}")]
    public async Task<ActionResult<CandidaturaResponse>> Apply(string vagaId)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var vaga = await _vagas.GetByIdAsync(vagaId);
        if (vaga is null) return NotFound();

        var existing = await _repo.GetByUserAndVagaAsync(uid, vagaId);
        if (existing is not null) return Conflict("ja_candidatado");

        var c = new Candidatura
        {
            UserId = uid,
            VagaId = vagaId,
            CriadoEm = DateTime.UtcNow
        };
        await _repo.InsertAsync(c);

        return CreatedAtAction(nameof(GetMine), new { }, new CandidaturaResponse(c.Id!, c.VagaId, vaga.Titulo, c.CriadoEm));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var myItems = await _repo.GetByUserAsync(uid);
        if (myItems.All(x => x.Id != id)) return NotFound();

        await _repo.DeleteAsync(id);
        return NoContent();
    }
}

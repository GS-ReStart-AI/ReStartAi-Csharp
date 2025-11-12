using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VagasController : ControllerBase
{
    private readonly IVagaRepository _repo;

    public VagasController(IVagaRepository repo)
    {
        _repo = repo;
    }

    public record VagaRequest(string Titulo, List<string> MustSkills, List<string> NiceSkills, string Area, bool Ativo);
    public record VagaResponse(string Id, string Titulo, List<string> MustSkills, List<string> NiceSkills, string Area, bool Ativo, DateTime CriadoEm);

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<VagaResponse>>> Get([FromQuery] string? area = null, [FromQuery] string? must = null, [FromQuery] string? nice = null, [FromQuery] bool somenteAtivas = true)
    {
        var mustSkills = string.IsNullOrWhiteSpace(must) ? Array.Empty<string>() : must.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var niceSkills = string.IsNullOrWhiteSpace(nice) ? Array.Empty<string>() : nice.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (somenteAtivas && string.IsNullOrWhiteSpace(area) && !mustSkills.Any() && !niceSkills.Any())
        {
            var ativas = await _repo.GetAtivasAsync();
            return Ok(ativas.Select(v => new VagaResponse(v.Id!, v.Titulo, v.MustSkills, v.NiceSkills, v.Area, v.Ativo, v.CriadoEm)).ToList());
        }

        var list = await _repo.SearchAsync(area, mustSkills, niceSkills);
        return Ok(list.Select(v => new VagaResponse(v.Id!, v.Titulo, v.MustSkills, v.NiceSkills, v.Area, v.Ativo, v.CriadoEm)).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VagaResponse>> GetById(string id)
    {
        var v = await _repo.GetByIdAsync(id);
        if (v is null) return NotFound();
        return Ok(new VagaResponse(v.Id!, v.Titulo, v.MustSkills, v.NiceSkills, v.Area, v.Ativo, v.CriadoEm));
    }

    [HttpPost]
    public async Task<ActionResult<VagaResponse>> Create([FromBody] VagaRequest req)
    {
        var v = new Vaga
        {
            Titulo = req.Titulo,
            MustSkills = req.MustSkills?.Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList() ?? new(),
            NiceSkills = req.NiceSkills?.Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList() ?? new(),
            Area = req.Area,
            Ativo = req.Ativo,
            CriadoEm = DateTime.UtcNow
        };
        await _repo.InsertAsync(v);
        return CreatedAtAction(nameof(GetById), new { id = v.Id }, new VagaResponse(v.Id!, v.Titulo, v.MustSkills, v.NiceSkills, v.Area, v.Ativo, v.CriadoEm));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VagaResponse>> Update(string id, [FromBody] VagaRequest req)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Titulo = req.Titulo;
        existing.MustSkills = req.MustSkills?.Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList() ?? new();
        existing.NiceSkills = req.NiceSkills?.Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList() ?? new();
        existing.Area = req.Area;
        existing.Ativo = req.Ativo;

        await _repo.UpdateAsync(id, existing);
        return Ok(new VagaResponse(existing.Id!, existing.Titulo, existing.MustSkills, existing.NiceSkills, existing.Area, existing.Ativo, existing.CriadoEm));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return NotFound();
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}

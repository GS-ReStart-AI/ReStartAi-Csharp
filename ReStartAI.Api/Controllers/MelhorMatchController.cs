using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Matching;
using ReStartAI.Application.WhyMe;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/usuarios/me")]
[Authorize]
public class MelhorMatchController : ControllerBase
{
    private readonly ICurriculoRepository _curriculos;
    private readonly IVagaRepository _vagas;
    private readonly WhyMeGenerator _why;

    public MelhorMatchController(ICurriculoRepository curriculos, IVagaRepository vagas, WhyMeGenerator why)
    {
        _curriculos = curriculos;
        _vagas = vagas;
        _why = why;
    }

    public record MelhorMatchResponse(string role, int matchPercent, List<string> whyMe, List<string> missingSkills);

    [HttpGet("melhor-match")]
    public async Task<ActionResult<MelhorMatchResponse>> GetMelhorMatch()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var curr = await _curriculos.GetLastByUserAsync(uid);
        if (curr is null || curr.Skills.Count == 0) return NotFound();

        var vagas = await _vagas.GetAtivasAsync();
        if (vagas.Count == 0) return NotFound();

        var matcher = new DeterministicMatcher();
        var best = matcher.BestMatch(vagas, curr.Skills);
        if (best is null) return NotFound();

        var papel = best.Vaga.Titulo;
        var percent = (int)Math.Round(best.Percentual * 100);
        var why = await _why.GenerateAsync(
            papel,
            curr.Skills,
            best.MustMatched,
            best.NiceMatched,
            best.MustMissing,
            best.NiceMissing
        );

        var resp = new MelhorMatchResponse(papel, percent, why, best.MustMissing);
        return Ok(resp);
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Matching;
using ReStartAI.Application.WhyMe;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Api.Controllers.Career
{
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

        public record MelhorMatchResponse(string Role, int MatchPercent, List<string> WhyMe, List<string> MissingSkills);

        [HttpGet("melhor-match")]
        public async Task<ActionResult<MelhorMatchResponse>> GetMelhorMatch()
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            var curriculosUsuario = await _curriculos.GetAllAsync(1, 10);
            var curr = curriculosUsuario.FirstOrDefault(c => c.UsuarioId == uid);
            if (curr is null) return NotFound();

            var vagas = await _vagas.GetAllAsync(1, 50);
            var matcher = new DeterministicMatcher();
            var best = matcher.BestMatch(vagas, curr.Skills);
            if (best is null) return NotFound();

            var papel = best.Vaga.Titulo;
            var percent = (int)Math.Round(best.Percentual * 100);
            var why = await _why.GenerateAsync(papel, curr.Skills, best.MustMatched, best.NiceMatched, best.MustMissing, best.NiceMissing);

            var resp = new MelhorMatchResponse(papel, percent, why, best.MustMissing);
            return Ok(resp);
        }
    }
}
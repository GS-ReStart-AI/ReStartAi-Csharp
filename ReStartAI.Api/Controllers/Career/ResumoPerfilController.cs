﻿using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using ReStartAI.Api.Integration;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [Route("api/usuarios/me")]
    [ApiKeyAuth]
    [Produces("application/json")]
    public class ResumoPerfilController : ControllerBase
    {
        public record ResumoResponse(List<string> Areas, List<string> Roles, int Experiencias);

        private readonly ICurriculoRepository _curriculos;
        private readonly IResumeSummaryClient _resumeSummary;

        public ResumoPerfilController(ICurriculoRepository curriculos, IResumeSummaryClient resumeSummary)
        {
            _curriculos = curriculos;
            _resumeSummary = resumeSummary;
        }

        [HttpGet("resumo-perfil")]
        [SwaggerOperation(
            Summary = "Retorna um resumo do perfil do usuário",
            Description = "Gera um resumo do perfil com base no currículo mais recente usando o serviço IoT + GPT-4o mini."
        )]
        [ProducesResponseType(typeof(ResumoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ResumoResponse>> GetResumoPerfil([FromQuery] string usuarioId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(usuarioId))
                return BadRequest("usuarioId obrigatório.");

            var pagina = await _curriculos.GetAllAsync(1, 50);
            var curr = pagina
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.CriadoEm)
                .FirstOrDefault();

            if (curr is null)
                return NotFound();

            var texto = (curr.Texto ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(texto))
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Currículo ainda não processado.");

            try
            {
                var aiResult = await _resumeSummary.GenerateAsync(usuarioId, texto, ct);

                var areas = aiResult.Areas?.ToList() ?? new List<string>();
                if (!areas.Any())
                    areas.Add("Área em análise");

                var roles = new List<string>();
                if (!string.IsNullOrWhiteSpace(aiResult.BestRole))
                    roles.Add(aiResult.BestRole);
                if (aiResult.Roles != null && aiResult.Roles.Count > 0)
                {
                    foreach (var r in aiResult.Roles)
                    {
                        if (!string.IsNullOrWhiteSpace(r) && !roles.Contains(r))
                            roles.Add(r);
                    }
                }

                if (!roles.Any())
                    roles.Add("Papel em análise");

                var experiencias = aiResult.YearsOfExperience;
                if (experiencias < 0)
                    experiencias = 0;

                var resp = new ResumoResponse(areas, roles, experiencias);
                return Ok(resp);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Não foi possível gerar o resumo no momento.");
            }
        }
    }
}

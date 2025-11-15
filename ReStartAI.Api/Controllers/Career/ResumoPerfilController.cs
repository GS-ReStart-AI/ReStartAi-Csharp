using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Security;
using ReStartAI.Api.Swagger.Examples.ResumoPerfil;
using ReStartAI.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers.Career
{
    [ApiController]
    [Route("api/usuarios/me")]
    [ApiKeyAuth] 
    [Produces("application/json")]
    public class ResumoPerfilController : ControllerBase
    {
            private readonly ICurriculoRepository _curriculos;

            public ResumoPerfilController(ICurriculoRepository curriculos)
            {
                _curriculos = curriculos;
            }

            public record ResumoResponse(List<string> Areas, List<string> Roles, int Experiencias);

            [HttpGet("resumo-perfil")]
            [SwaggerOperation(
                Summary = "Retorna um resumo do perfil do usuário autenticado",
                Description = "Gera um resumo simples do perfil a partir do currículo do usuário, incluindo áreas de atuação, possíveis roles e anos de experiência."
            )]
            [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ResumoPerfilResponseExample))]
            [ProducesResponseType(typeof(ResumoResponse), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<ResumoResponse>> GetResumoPerfil()
            {
                var uid = User.FindFirstValue("uid");
                if (string.IsNullOrEmpty(uid)) return Unauthorized();

                var curriculosUsuario = await _curriculos.GetAllAsync(1, 10);
                var curr = curriculosUsuario.FirstOrDefault(c => c.UsuarioId == uid);
                if (curr is null) return NotFound();

                var skills = curr.Skills ?? new List<string>();

                var areas = new List<string>();
                if (skills.Any(s => s.Contains("backend", StringComparison.InvariantCultureIgnoreCase) ||
                                    s.Contains(".net", StringComparison.InvariantCultureIgnoreCase)))
                    areas.Add("Back-end .NET");
                if (skills.Any(s => s.Contains("api", StringComparison.InvariantCultureIgnoreCase)))
                    areas.Add("APIs e Integrações");
                if (!areas.Any())
                    areas.Add("Desenvolvimento de Software");

                var roles = new List<string>
                {
                    "Desenvolvedor .NET",
                    "Backend Developer"
                };

                var experiencias = curr.CriadoEm == default
                    ? 1
                    : Math.Max(1, DateTime.UtcNow.Year - curr.CriadoEm.Year);

                var resp = new ResumoResponse(areas, roles, experiencias);
                return Ok(resp);
            }
    }
}

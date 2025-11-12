using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Parsing;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/usuarios/me")]
[Authorize]
public class ResumoPerfilController : ControllerBase
{
    private readonly ICurriculoRepository _curriculos;

    public ResumoPerfilController(ICurriculoRepository curriculos)
    {
        _curriculos = curriculos;
    }

    public record ResumoResponse(List<string> Areas, List<string> PapeisSugeridos, int Experiencias);

    [HttpGet("resumo")]
    public async Task<ActionResult<ResumoResponse>> GetResumo()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var curr = await _curriculos.GetLastByUserAsync(uid);
        if (curr is null) return NotFound();

        var texto = string.Join(" ", curr.Skills);
        var parser = new ResumeParser();
        var parsed = parser.Parse(texto);

        var areas = parsed.Areas;
        var roles = parsed.PapeisSugeridos;
        var experiencias = curr.Skills?.Count ?? 0;

        return Ok(new ResumoResponse(areas, roles, experiencias));
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Security;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarios;

    public UsuariosController(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public record UsuarioResponse(string Id, string NomeCompleto, string Cpf, DateTime? DataNascimento, string Email, DateTime CriadoEm, DateTime? AtualizadoEm);
    public record UpdateUsuarioRequest(string NomeCompleto, string Cpf, DateTime? DataNascimento, string Email, string? SenhaNova);
    public record DeleteRequest(string Senha);

    [HttpGet("me")]
    public async Task<ActionResult<UsuarioResponse>> GetMe()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        var u = await _usuarios.GetByIdAsync(uid);
        if (u is null) return NotFound();
        return Ok(new UsuarioResponse(u.Id!, u.NomeCompleto, u.Cpf, u.DataNascimento, u.Email, u.CriadoEm, u.AtualizadoEm));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UsuarioResponse>> UpdateMe([FromBody] UpdateUsuarioRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        var u = await _usuarios.GetByIdAsync(uid);
        if (u is null) return NotFound();

        if (!string.Equals(u.Email, req.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _usuarios.EmailExistsAsync(req.Email)) return Conflict("email_ja_utilizado");
            u.Email = req.Email;
        }

        u.NomeCompleto = req.NomeCompleto;
        u.Cpf = req.Cpf;
        u.DataNascimento = req.DataNascimento;
        if (!string.IsNullOrWhiteSpace(req.SenhaNova)) u.SenhaHash = PasswordHasher.Hash(req.SenhaNova);
        u.AtualizadoEm = DateTime.UtcNow;

        await _usuarios.UpdateAsync(uid, u);

        return Ok(new UsuarioResponse(u.Id!, u.NomeCompleto, u.Cpf, u.DataNascimento, u.Email, u.CriadoEm, u.AtualizadoEm));
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe([FromBody] DeleteRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();
        var u = await _usuarios.GetByIdAsync(uid);
        if (u is null) return NotFound();

        if (!PasswordHasher.Verify(req.Senha, u.SenhaHash)) return Unauthorized();

        await _usuarios.DeleteAsync(uid);
        return NoContent();
    }
}

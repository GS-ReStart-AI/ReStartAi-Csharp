using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Security;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/usuarios/me")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly PasswordHasher _hasher;

    public UsuariosController(IUsuarioRepository usuarios, PasswordHasher hasher)
    {
        _usuarios = usuarios;
        _hasher = hasher;
    }

    public record ProfileResponse(string Id, string? NomeCompleto, string? Cpf, DateTime? DataNascimento, string Email);
    public record UpdateRequest(string? NomeCompleto, string? Cpf, DateTime? DataNascimento, string Email, string? NovaSenha);
    public record DeleteRequest(string Senha);

    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> Get()
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var u = await _usuarios.GetByIdAsync(uid);
        if (u is null) return NotFound();

        return Ok(new ProfileResponse(u.Id!, u.NomeCompleto, u.Cpf, u.DataNascimento, u.Email));
    }

    [HttpPut]
    public async Task<ActionResult<ProfileResponse>> Update([FromBody] UpdateRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var u = await _usuarios.GetByIdAsync(uid);
        if (u is null) return NotFound();

        if (!string.Equals(u.Email, req.Email.Trim().ToLowerInvariant(), StringComparison.Ordinal))
        {
            var exists = await _usuarios.GetByEmailAsync(req.Email.Trim().ToLowerInvariant());
            if (exists is not null) return Conflict("email_ja_cadastrado");
        }

        u.NomeCompleto = req.NomeCompleto ?? u.NomeCompleto;
        u.Cpf = req.Cpf ?? u.Cpf;
        u.DataNascimento = req.DataNascimento ?? u.DataNascimento;
        u.Email = req.Email.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(req.NovaSenha))
            u.SenhaHash = _hasher.Hash(req.NovaSenha);

        await _usuarios.UpdateAsync(u.Id!, u);

        return Ok(new ProfileResponse(u.Id!, u.NomeCompleto, u.Cpf, u.DataNascimento, u.Email));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRequest req)
    {
        var uid = User.FindFirstValue("uid");
        if (string.IsNullOrEmpty(uid)) return Unauthorized();

        var u = await _usuarios.GetByIdAsync(uid);
        if (u is null) return NotFound();

        if (!_hasher.Verify(req.Senha, u.SenhaHash))
            return Unauthorized();

        await _usuarios.DeleteAsync(uid);
        return NoContent();
    }
}

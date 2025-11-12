using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Security;
using ReStartAI.Domain.Entities;
using ReStartAI.Infrastructure.Repositories;

namespace ReStartAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly JwtTokenService _jwt;

    public AuthController(IUsuarioRepository usuarios, JwtTokenService jwt)
    {
        _usuarios = usuarios;
        _jwt = jwt;
    }

    public record SignupRequest(string NomeCompleto, string Cpf, DateTime? DataNascimento, string Email, string Senha);
    public record LoginRequest(string Email, string Senha);
    public record AuthResponse(string Token, DateTime ExpiresAtUtc, string UserId, string Email, string NomeCompleto);

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest req)
    {
        if (await _usuarios.EmailExistsAsync(req.Email)) return Conflict("email_ja_utilizado");

        var user = new Usuario
        {
            NomeCompleto = req.NomeCompleto,
            Cpf = req.Cpf,
            DataNascimento = req.DataNascimento,
            Email = req.Email,
            SenhaHash = PasswordHasher.Hash(req.Senha),
            CriadoEm = DateTime.UtcNow
        };

        await _usuarios.InsertAsync(user);

        var (token, exp) = _jwt.CreateToken(user.Email, new[]
        {
            new Claim("uid", user.Id ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email)
        });

        return Created($"api/usuarios/{user.Id}", new AuthResponse(token, exp, user.Id!, user.Email, user.NomeCompleto));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await _usuarios.GetByEmailAsync(req.Email);
        if (user is null) return Unauthorized();

        if (!PasswordHasher.Verify(req.Senha, user.SenhaHash)) return Unauthorized();

        var (token, exp) = _jwt.CreateToken(user.Email, new[]
        {
            new Claim("uid", user.Id ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email)
        });

        return Ok(new AuthResponse(token, exp, user.Id!, user.Email, user.NomeCompleto));
    }
}

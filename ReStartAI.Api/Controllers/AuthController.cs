using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReStartAI.Application.Security;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly PasswordHasher _hasher;
        private readonly JwtTokenService _jwt;

        public AuthController(IUsuarioRepository usuarios, PasswordHasher hasher, JwtTokenService jwt)
        {
            _usuarios = usuarios;
            _hasher = hasher;
            _jwt = jwt;
        }

        public record SignupRequest(string NomeCompleto, string Cpf, DateTime DataNascimento, string Email, string Senha);
        public record LoginRequest(string Email, string Senha);
        public record AuthResponse(string Token, DateTime ExpiresAt);

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha))
                return BadRequest();

            var exists = await _usuarios.GetByEmailAsync(req.Email.Trim().ToLowerInvariant());
            if (exists is not null) return Conflict("email_ja_cadastrado");

            var u = new Usuario
            {
                NomeCompleto = req.NomeCompleto,
                Cpf = req.Cpf,
                DataNascimento = req.DataNascimento,
                Email = req.Email.Trim().ToLowerInvariant(),
                SenhaHash = _hasher.Hash(req.Senha)
            };

            await _usuarios.CreateAsync(u);

            var claims = new[]
            {
                new Claim("uid", u.Id!),
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Name, u.NomeCompleto ?? u.Email),
                new Claim(ClaimTypes.Role, "user")
            };

            var expires = DateTime.UtcNow.AddHours(8);
            var token = _jwt.CreateToken(u.Email, expires, claims);
            return Created("", new AuthResponse(token, expires));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var user = await _usuarios.GetByEmailAsync(req.Email.Trim().ToLowerInvariant());
            if (user is null) return Unauthorized();

            if (!_hasher.Verify(req.Senha, user.SenhaHash)) return Unauthorized();

            var claims = new[]
            {
                new Claim("uid", user.Id!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.NomeCompleto ?? user.Email),
                new Claim(ClaimTypes.Role, "user")
            };

            var expires = DateTime.UtcNow.AddHours(8);
            var token = _jwt.CreateToken(user.Email, expires, claims);
            return Ok(new AuthResponse(token, expires));
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return NoContent();
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using ReStartAI.Api.Swagger.Examples.Auth;
using ReStartAI.Api.Security;
using ReStartAI.Application.Security;
using ReStartAI.Domain.Entities;
using ReStartAI.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly PasswordHasher _hasher;

        public AuthController(IUsuarioRepository usuarios, PasswordHasher hasher)
        {
            _usuarios = usuarios;
            _hasher = hasher;
        }

        public record SignupRequest(string NomeCompleto, string Cpf, DateTime DataNascimento, string Email, string Senha);
        public record LoginRequest(string Email, string Senha);
        public record AuthResponse(string Token, DateTime ExpiresAt, string UsuarioId);

        [HttpPost("signup")]
        [SwaggerOperation(
            Summary = "Cadastro de usuário",
            Description = "Cria um novo usuário."
        )]
        [SwaggerRequestExample(typeof(SignupRequest), typeof(SignupRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(AuthResponseExample))]
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

            var created = await _usuarios.CreateAsync(u);

            var expires = DateTime.UtcNow.AddHours(8);
            var token = Guid.NewGuid().ToString("N");

            return Created(string.Empty, new AuthResponse(token, expires, created.Id));
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login",
            Description = "Valida o usuário e retorna uma resposta de autenticação simples."
        )]
        [SwaggerRequestExample(typeof(LoginRequest), typeof(LoginRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AuthResponseExample))]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var user = await _usuarios.GetByEmailAsync(req.Email.Trim().ToLowerInvariant());
            if (user is null) return Unauthorized();

            if (!_hasher.Verify(req.Senha, user.SenhaHash)) return Unauthorized();

            var expires = DateTime.UtcNow.AddHours(8);
            var token = Guid.NewGuid().ToString("N");

            return Ok(new AuthResponse(token, expires, user.Id));
        }

        [HttpPost("logout")]
        [ApiKeyAuth]
        [SwaggerOperation(
            Summary = "Logout",
            Description = "Endpoint protegido por API Key. Não invalida estado no servidor."
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Logout()
        {
            return NoContent();
        }
    }
}

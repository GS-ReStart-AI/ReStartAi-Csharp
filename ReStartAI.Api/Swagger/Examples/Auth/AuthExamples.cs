using System;
using ReStartAI.Api.Controllers;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Auth
{
    public class SignupRequestExample : IExamplesProvider<AuthController.SignupRequest>
    {
        public AuthController.SignupRequest GetExamples()
        {
            return new AuthController.SignupRequest(
                NomeCompleto: "João Silva",
                Cpf: "12345678900",
                DataNascimento: new DateTime(1995, 5, 10),
                Email: "joao.silva@example.com",
                Senha: "Senha@123"
            );
        }
    }

    public class LoginRequestExample : IExamplesProvider<AuthController.LoginRequest>
    {
        public AuthController.LoginRequest GetExamples()
        {
            return new AuthController.LoginRequest(
                Email: "joao.silva@example.com",
                Senha: "Senha@123"
            );
        }
    }

    public class AuthResponseExample : IExamplesProvider<AuthController.AuthResponse>
    {
        public AuthController.AuthResponse GetExamples()
        {
            var expires = DateTime.UtcNow.AddHours(8);

            return new AuthController.AuthResponse(
                Token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.exemplo_payload.assinatura",
                ExpiresAt: expires
            );
        }
    }
}
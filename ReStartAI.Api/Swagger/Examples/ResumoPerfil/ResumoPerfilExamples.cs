using System.Collections.Generic;
using ReStartAI.Api.Controllers.Career;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.ResumoPerfil
{
    public class ResumoPerfilResponseExample : IExamplesProvider<ResumoPerfilController.ResumoResponse>
    {
        public ResumoPerfilController.ResumoResponse GetExamples()
        {
            return new ResumoPerfilController.ResumoResponse(
                new List<string>
                {
                    "Desenvolvimento de Software",
                    "Back-end .NET",
                    "APIs e Integrações"
                },
                new List<string>
                {
                    "Desenvolvedor .NET Pleno",
                    "Engenheiro de Software",
                    "Backend Developer"
                },
                8
            );
        }
    }
}
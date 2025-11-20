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
                8,
                "Desenvolvedor .NET Pleno",
                87.5,
                "Você possui sólida experiência em desenvolvimento back-end com .NET, trabalha há vários anos com APIs e integrações e domina as principais tecnologias exigidas para esse tipo de vaga, o que aumenta muito seu potencial de sucesso nesse papel."
            );
        }
    }
}
using ReStartAI.Api.Controllers.Career;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.MelhorMatch
{
    public class MelhorMatchResponseExample : IExamplesProvider<MelhorMatchController.MelhorMatchResponse>
    {
        public MelhorMatchController.MelhorMatchResponse GetExamples()
        {
            return new MelhorMatchController.MelhorMatchResponse(
                Role: "Desenvolvedor .NET Pleno",
                MatchPercent: 92,
                WhyMe: new List<string>
                {
                    "Você domina C# e .NET, que são requisitos obrigatórios da vaga.",
                    "Experiência com APIs REST e bancos NoSQL, alinhadas ao stack da empresa.",
                    "Histórico recente de projetos com microserviços e boas práticas de código."
                },
                MissingSkills: new List<string>
                {
                    "Inglês avançado",
                    "Experiência prévia com mensageria (RabbitMQ, Kafka)"
                }
            );
        }
    }
}
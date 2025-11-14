using ReStartAI.Api.Controllers.Career;
using ReStartAI.Application.IoT;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Insights
{
    public class TriggerRequestExample : IExamplesProvider<InsightsController.TriggerRequest>
    {
        public InsightsController.TriggerRequest GetExamples()
        {
            var metrics = new MetricsDto(
                JobsViewed: 5,
                ApplyClicks: 2,
                LastAt: DateTime.UtcNow.AddMinutes(-3)
            );

            var profile = new ProfileDto(
                Areas: new List<string> { "Back-end .NET", "APIs e Integrações" },
                PapeisSugeridos: new List<string> { "Desenvolvedor .NET Pleno", "Backend Developer" },
                City: "São Paulo - SP",
                Gaps: "Aprimorar inglês e mensageria"
            );

            var bestOp = new BestOpportunityDto(
                Role: "Desenvolvedor .NET Pleno",
                City: "São Paulo - SP",
                Match: 0.92,
                MissingSkill: "Inglês avançado"
            );

            return new InsightsController.TriggerRequest(
                Metrics: metrics,
                LastEvents: new List<string>
                {
                    "job_view: vaga 123",
                    "job_view: vaga 456",
                    "apply_click: vaga 456"
                },
                Profile: profile,
                BestOpportunity: bestOp
            );
        }
    }

    public class TriggerResponseExample : IExamplesProvider<InsightsController.TriggerResponse>
    {
        public InsightsController.TriggerResponse GetExamples()
        {
            return new InsightsController.TriggerResponse(
                NotificationId: "64f1234567890abcdefn001",
                Insight: "Você tem ótimo fit para vagas de Desenvolvedor .NET Pleno; que tal aplicar em mais 2 hoje?",
                ActionTag: "APPLY_FOCUS"
            );
        }
    }

    public class AutoTriggerResponseExample : IExamplesProvider<InsightsAutoController.AutoTriggerResponse>
    {
        public InsightsAutoController.AutoTriggerResponse GetExamples()
        {
            return new InsightsAutoController.AutoTriggerResponse(
                NotificationId: "64f1234567890abcdefn002",
                Insight: "Você está há alguns dias sem explorar novas vagas; experimente ver oportunidades de back-end.",
                ActionTag: "EXPLORE_JOBS"
            );
        }
    }
}

using ReStartAI.Api.Controllers.Career;
using ReStartAI.Application.IoT;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Career
{
    public class TriggerRequestExample : IExamplesProvider<CareerController.TriggerRequest>
    {
        public CareerController.TriggerRequest GetExamples()
        {
            var metrics = new MetricsDto(
                JobsViewedToday: 5,
                ApplyClicksToday: 2,
                LastEventAt: DateTime.UtcNow.AddMinutes(-3)
            );

            var profile = new ProfileDto(
                Areas: new List<string> { "Back-end .NET", "APIs e Integrações" },
                Roles: new List<string> { "Desenvolvedor .NET Pleno", "Backend Developer" },
                City: "São Paulo - SP",
                Gaps: new List<string> { "Aprimorar inglês e mensageria" }
            );

            var bestOp = new BestOpportunityDto(
                Role: "Desenvolvedor .NET Pleno",
                City: "São Paulo - SP",
                Match: 92,
                MissingSkill: "Inglês avançado"
            );

            var lastEvents = new List<EventDto>
            {
                new EventDto("job_view", DateTime.UtcNow.AddMinutes(-10)),
                new EventDto("apply_click", DateTime.UtcNow.AddMinutes(-5))
            };

            return new CareerController.TriggerRequest(
                UsuarioId: "507f191e810c19729de860ea",
                Metrics: metrics,
                LastEvents: lastEvents,
                Profile: profile,
                BestOpportunity: bestOp
            );
        }
    }
}
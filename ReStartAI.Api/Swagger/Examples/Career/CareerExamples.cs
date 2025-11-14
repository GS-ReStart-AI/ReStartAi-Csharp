using System;
using System.Collections.Generic;
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

            return new CareerController.TriggerRequest(
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

    public class TriggerResponseExample : IExamplesProvider<CareerController.TriggerResponse>
    {
        public CareerController.TriggerResponse GetExamples()
        {
            return new CareerController.TriggerResponse(
                NotificationId: "64f1234567890abcdefn001",
                Insight: "Você tem ótimo fit para vagas de Desenvolvedor .NET Pleno; que tal aplicar em mais 2 hoje?",
                ActionTag: "APPLY_FOCUS"
            );
        }
    }

    public class AutoTriggerResponseExample : IExamplesProvider<CareerController.AutoTriggerResponse>
    {
        public CareerController.AutoTriggerResponse GetExamples()
        {
            return new CareerController.AutoTriggerResponse(
                NotificationId: "64f1234567890abcdefn002",
                Insight: "Você está há alguns dias sem explorar novas vagas; experimente ver oportunidades de back-end.",
                ActionTag: "EXPLORE_JOBS"
            );
        }
    }

    public class MelhorMatchResponseExample : IExamplesProvider<CareerController.MelhorMatchResponse>
    {
        public CareerController.MelhorMatchResponse GetExamples()
        {
            return new CareerController.MelhorMatchResponse(
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

    public class UploadCurriculoResponseExample : IExamplesProvider<CareerController.UploadCurriculoResponse>
    {
        public CareerController.UploadCurriculoResponse GetExamples()
        {
            return new CareerController.UploadCurriculoResponse(
                CurriculoId: "64f1234567890abcdefc001",
                Nome: "Ana Silva",
                Email: "ana.silva@example.com",
                Telefone: "+55 11 99999-0000",
                Skills: new List<string>
                {
                    "C#",
                    ".NET",
                    "ASP.NET Core",
                    "MongoDB",
                    "APIs REST"
                },
                Areas: new List<string>
                {
                    "Back-end .NET",
                    "APIs e Integrações"
                },
                PapeisSugeridos: new List<string>
                {
                    "Desenvolvedor .NET Pleno",
                    "Backend Developer"
                }
            );
        }
    }
}

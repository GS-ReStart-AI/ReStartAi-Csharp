using ReStartAI.Application.Dto;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Candidatura
{
    public class CandidaturaRequestExample : IExamplesProvider<Domain.Entities.Candidatura>
    {
        public Domain.Entities.Candidatura GetExamples()
        {
            return new Domain.Entities.Candidatura
            {
                Id = "64f1234567890abcdefc001",
                UsuarioId = "64f1234567890abcdefu001",
                VagaId = "64f1234567890abcdefv001",
                Status = "Em análise",
                ScoreMatch = 87,
                ApplyUrl = "https://jobs.restart.ai/apply/64f1234567890abcdefv001"
            };
        }
    }

    public class CandidaturaUpdateRequestExample : IExamplesProvider<Domain.Entities.Candidatura>
    {
        public Domain.Entities.Candidatura GetExamples()
        {
            return new Domain.Entities.Candidatura
            {
                Id = "64f1234567890abcdefc001",
                UsuarioId = "64f1234567890abcdefu001",
                VagaId = "64f1234567890abcdefv001",
                Status = "Entrevista agendada",
                ScoreMatch = 90,
                ApplyUrl = "https://jobs.restart.ai/apply/64f1234567890abcdefv001"
            };
        }
    }

    public class CandidaturaResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                entity = new
                {
                    id = "64f1234567890abcdefc001",
                    usuarioId = "64f1234567890abcdefu001",
                    vagaId = "64f1234567890abcdefv001",
                    status = "Em análise",
                    scoreMatch = 87,
                    applyUrl = "https://jobs.restart.ai/apply/64f1234567890abcdefv001"
                },
                _links = new[]
                {
                    new { rel = "self", href = "/api/v1/candidatura/64f1234567890abcdefc001", method = "GET" },
                    new { rel = "update", href = "/api/v1/candidatura/64f1234567890abcdefc001", method = "PUT" },
                    new { rel = "delete", href = "/api/v1/candidatura/64f1234567890abcdefc001", method = "DELETE" }
                }
            };
        }
    }

    public class CandidaturaListResponseExample : IExamplesProvider<PagedResult<object>>
    {
        public PagedResult<object> GetExamples()
        {
            var items = new[]
            {
                new
                {
                    entity = new
                    {
                        id = "64f1234567890abcdefc001",
                        usuarioId = "64f1234567890abcdefu001",
                        vagaId = "64f1234567890abcdefv001",
                        status = "Em análise",
                        scoreMatch = 87,
                        applyUrl = "https://jobs.restart.ai/apply/64f1234567890abcdefv001"
                    },
                    _links = new[]
                    {
                        new { rel = "self", href = "/api/v1/candidatura/64f1234567890abcdefc001", method = "GET" },
                        new { rel = "update", href = "/api/v1/candidatura/64f1234567890abcdefc001", method = "PUT" },
                        new { rel = "delete", href = "/api/v1/candidatura/64f1234567890abcdefc001", method = "DELETE" }
                    }
                }
            };

            return new PagedResult<object>(items, totalItems: 1, page: 1, pageSize: 10);
        }
    }

    public class CandidaturaDeleteResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Candidatura removida com sucesso"
            };
        }
    }
}

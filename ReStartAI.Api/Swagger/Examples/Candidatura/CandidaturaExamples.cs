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
                UsuarioId = "000000000000000000000001",
                VagaId = "000000000000000000000002",
                Status = "Em análise",
                ScoreMatch = 87,
                ApplyUrl = "https://jobs.restart.ai/apply/000000000000000000000002"
            };
        }
    }

    public class CandidaturaUpdateRequestExample : IExamplesProvider<Domain.Entities.Candidatura>
    {
        public Domain.Entities.Candidatura GetExamples()
        {
            return new Domain.Entities.Candidatura
            {
                UsuarioId = "000000000000000000000001",
                VagaId = "000000000000000000000002",
                Status = "Entrevista agendada",
                ScoreMatch = 90,
                ApplyUrl = "https://jobs.restart.ai/apply/000000000000000000000002"
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
                    id = "000000000000000000000003",
                    usuarioId = "000000000000000000000001",
                    vagaId = "000000000000000000000002",
                    status = "Em análise",
                    scoreMatch = 87,
                    applyUrl = "https://jobs.restart.ai/apply/000000000000000000000002"
                },
                _links = new[]
                {
                    new { rel = "self",   href = "/api/v1/candidatura/000000000000000000000003", method = "GET" },
                    new { rel = "update", href = "/api/v1/candidatura/000000000000000000000003", method = "PUT" },
                    new { rel = "delete", href = "/api/v1/candidatura/000000000000000000000003", method = "DELETE" }
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
                        id = "000000000000000000000003",
                        usuarioId = "000000000000000000000001",
                        vagaId = "000000000000000000000002",
                        status = "Em análise",
                        scoreMatch = 87,
                        applyUrl = "https://jobs.restart.ai/apply/000000000000000000000002"
                    },
                    _links = new[]
                    {
                        new { rel = "self",   href = "/api/v1/candidatura/000000000000000000000003", method = "GET" },
                        new { rel = "update", href = "/api/v1/candidatura/000000000000000000000003", method = "PUT" },
                        new { rel = "delete", href = "/api/v1/candidatura/000000000000000000000003", method = "DELETE" }
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

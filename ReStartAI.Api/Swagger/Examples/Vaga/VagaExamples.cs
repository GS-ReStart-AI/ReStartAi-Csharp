using ReStartAI.Application.Dto;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Vaga
{
    public class VagaRequestExample : IExamplesProvider<Domain.Entities.Vaga>
    {
        public Domain.Entities.Vaga GetExamples()
        {
            return new Domain.Entities.Vaga
            {
                Titulo = "Desenvolvedor .NET Pleno",
                Empresa = "ReStart.AI",
                Cidade = "São Paulo - SP",
                Senioridade = "Pleno",
                Descricao = "Responsável por desenvolver e manter APIs em .NET.",
                ReqMust = "C#, .NET, MongoDB, REST, Git",
                ReqNice = "Docker, Kubernetes, Mensageria",
                Ativa = true
            };
        }
    }

    public class VagaUpdateRequestExample : IExamplesProvider<Domain.Entities.Vaga>
    {
        public Domain.Entities.Vaga GetExamples()
        {
            return new Domain.Entities.Vaga
            {
                Titulo = "Desenvolvedor .NET Pleno (Atualizado)",
                Empresa = "ReStart.AI",
                Cidade = "São Paulo - SP",
                Senioridade = "Pleno",
                Descricao = "Atuação em APIs .NET e integrações com serviços externos.",
                ReqMust = "C#, .NET 8, MongoDB, REST, Git",
                ReqNice = "Docker, Kubernetes, RabbitMQ",
                Ativa = true
            };
        }
    }

    public class VagaResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                entity = new
                {
                    id = "000000000000000000000002",
                    titulo = "Desenvolvedor .NET Pleno",
                    empresa = "ReStart.AI",
                    cidade = "São Paulo - SP",
                    senioridade = "Pleno",
                    descricao = "Responsável por desenvolver e manter APIs em .NET.",
                    reqMust = "C#, .NET, MongoDB, REST, Git",
                    reqNice = "Docker, Kubernetes, Mensageria",
                    ativa = true
                },
                _links = new[]
                {
                    new { rel = "self",   href = "/api/v1/vaga/000000000000000000000002", method = "GET" },
                    new { rel = "update", href = "/api/v1/vaga/000000000000000000000002", method = "PUT" },
                    new { rel = "delete", href = "/api/v1/vaga/000000000000000000000002", method = "DELETE" }
                }
            };
        }
    }

    public class VagaListResponseExample : IExamplesProvider<PagedResult<object>>
    {
        public PagedResult<object> GetExamples()
        {
            var items = new[]
            {
                new
                {
                    entity = new
                    {
                        id = "000000000000000000000002",
                        titulo = "Desenvolvedor .NET Pleno",
                        empresa = "ReStart.AI",
                        cidade = "São Paulo - SP",
                        senioridade = "Pleno",
                        descricao = "Responsável por desenvolver e manter APIs em .NET.",
                        reqMust = "C#, .NET, MongoDB, REST, Git",
                        reqNice = "Docker, Kubernetes, Mensageria",
                        ativa = true
                    },
                    _links = new[]
                    {
                        new { rel = "self",   href = "/api/v1/vaga/000000000000000000000002", method = "GET" },
                        new { rel = "update", href = "/api/v1/vaga/000000000000000000000002", method = "PUT" },
                        new { rel = "delete", href = "/api/v1/vaga/000000000000000000000002", method = "DELETE" }
                    }
                }
            };

            return new PagedResult<object>(items, totalItems: 1, page: 1, pageSize: 10);
        }
    }

    public class VagaDeleteResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Vaga removida com sucesso"
            };
        }
    }
}

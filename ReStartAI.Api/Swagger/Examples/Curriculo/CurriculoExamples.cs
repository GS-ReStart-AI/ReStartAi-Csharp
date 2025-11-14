using ReStartAI.Application.Dto;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Curriculo
{
    public class CurriculoRequestExample : IExamplesProvider<Domain.Entities.Curriculo>
    {
        public Domain.Entities.Curriculo GetExamples()
        {
            return new Domain.Entities.Curriculo
            {
                UsuarioId = "000000000000000000000001",
                NomeArquivo = "curriculo-joao-silva.pdf",
                Texto = "Profissional com experiência em desenvolvimento .NET, APIs REST e bancos NoSQL.",
                Skills = new List<string> { "C#", ".NET", "MongoDB", "REST", "Git" },
                CriadoEm = new DateTime(2024, 11, 1, 14, 30, 0, DateTimeKind.Utc)
            };
        }
    }

    public class CurriculoUpdateRequestExample : IExamplesProvider<Domain.Entities.Curriculo>
    {
        public Domain.Entities.Curriculo GetExamples()
        {
            return new Domain.Entities.Curriculo
            {
                UsuarioId = "000000000000000000000001",
                NomeArquivo = "curriculo-joao-silva-atualizado.pdf",
                Texto = "Profissional pleno em .NET, com experiência em microsserviços, mensageria e cloud.",
                Skills = new List<string> { "C#", ".NET 8", "MongoDB", "RabbitMQ", "Docker", "Kubernetes" },
                CriadoEm = new DateTime(2024, 11, 10, 9, 0, 0, DateTimeKind.Utc)
            };
        }
    }

    public class CurriculoResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                entity = new
                {
                    id = "000000000000000000000010",
                    usuarioId = "000000000000000000000001",
                    nomeArquivo = "curriculo-joao-silva.pdf",
                    texto = "Profissional com experiência em desenvolvimento .NET, APIs REST e bancos NoSQL.",
                    skills = new[] { "C#", ".NET", "MongoDB", "REST", "Git" },
                    criadoEm = "2024-11-01T14:30:00Z"
                },
                _links = new[]
                {
                    new { rel = "self",   href = "/api/v1/curriculo/000000000000000000000010", method = "GET" },
                    new { rel = "update", href = "/api/v1/curriculo/000000000000000000000010", method = "PUT" },
                    new { rel = "delete", href = "/api/v1/curriculo/000000000000000000000010", method = "DELETE" }
                }
            };
        }
    }

    public class CurriculoListResponseExample : IExamplesProvider<PagedResult<object>>
    {
        public PagedResult<object> GetExamples()
        {
            var items = new[]
            {
                new
                {
                    entity = new
                    {
                        id = "000000000000000000000010",
                        usuarioId = "000000000000000000000001",
                        nomeArquivo = "curriculo-joao-silva.pdf",
                        texto = "Profissional com experiência em desenvolvimento .NET, APIs REST e bancos NoSQL.",
                        skills = new[] { "C#", ".NET", "MongoDB", "REST", "Git" },
                        criadoEm = "2024-11-01T14:30:00Z"
                    },
                    _links = new[]
                    {
                        new { rel = "self",   href = "/api/v1/curriculo/000000000000000000000010", method = "GET" },
                        new { rel = "update", href = "/api/v1/curriculo/000000000000000000000010", method = "PUT" },
                        new { rel = "delete", href = "/api/v1/curriculo/000000000000000000000010", method = "DELETE" }
                    }
                }
            };

            return new PagedResult<object>(items, totalItems: 1, page: 1, pageSize: 10);
        }
    }

    public class CurriculoDeleteResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Currículo removido com sucesso"
            };
        }
    }
}

using ReStartAI.Application.Dto;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.Notificacao
{
    public class NotificacaoRequestExample : IExamplesProvider<Domain.Entities.Notificacao>
    {
        public Domain.Entities.Notificacao GetExamples()
        {
            return new Domain.Entities.Notificacao
            {
                UsuarioId = "000000000000000000000001",
                Titulo = "Nova vaga compatível com seu perfil",
                Mensagem = "Encontramos uma vaga de Desenvolvedor .NET Pleno que combina com o seu currículo."
            };
        }
    }

    public class NotificacaoUpdateRequestExample : IExamplesProvider<Domain.Entities.Notificacao>
    {
        public Domain.Entities.Notificacao GetExamples()
        {
            return new Domain.Entities.Notificacao
            {
                UsuarioId = "000000000000000000000001",
                Titulo = "Vaga atualizada",
                Mensagem = "A vaga de Desenvolvedor .NET Pleno teve a descrição atualizada. Confira os novos requisitos."
            };
        }
    }

    public class NotificacaoResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                entity = new
                {
                    id = "000000000000000000000030",
                    usuarioId = "000000000000000000000001",
                    titulo = "Nova vaga compatível com seu perfil",
                    mensagem = "Encontramos uma vaga de Desenvolvedor .NET Pleno que combina com o seu currículo."
                },
                _links = new[]
                {
                    new { rel = "self",   href = "/api/v1/notificacao/000000000000000000000030", method = "GET" },
                    new { rel = "update", href = "/api/v1/notificacao/000000000000000000000030", method = "PUT" },
                    new { rel = "delete", href = "/api/v1/notificacao/000000000000000000000030", method = "DELETE" }
                }
            };
        }
    }

    public class NotificacaoListResponseExample : IExamplesProvider<PagedResult<object>>
    {
        public PagedResult<object> GetExamples()
        {
            var items = new[]
            {
                new
                {
                    entity = new
                    {
                        id = "000000000000000000000030",
                        usuarioId = "000000000000000000000001",
                        titulo = "Nova vaga compatível com seu perfil",
                        mensagem = "Encontramos uma vaga de Desenvolvedor .NET Pleno que combina com o seu currículo."
                    },
                    _links = new[]
                    {
                        new { rel = "self",   href = "/api/v1/notificacao/000000000000000000000030", method = "GET" },
                        new { rel = "update", href = "/api/v1/notificacao/000000000000000000000030", method = "PUT" },
                        new { rel = "delete", href = "/api/v1/notificacao/000000000000000000000030", method = "DELETE" }
                    }
                }
            };

            return new PagedResult<object>(items, totalItems: 1, page: 1, pageSize: 10);
        }
    }

    public class NotificacaoDeleteResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Notificação removida com sucesso"
            };
        }
    }
}

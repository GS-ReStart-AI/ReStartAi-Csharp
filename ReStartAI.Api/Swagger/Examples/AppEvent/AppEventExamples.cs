using ReStartAI.Application.Dto;
using Swashbuckle.AspNetCore.Filters;

namespace ReStartAI.Api.Swagger.Examples.AppEvent
{
    public class AppEventRequestExample : IExamplesProvider<Domain.Entities.AppEvent>
    {
        public Domain.Entities.AppEvent GetExamples()
        {
            return new Domain.Entities.AppEvent
            {
                UsuarioId = "000000000000000000000001",
                Tipo = "job_view",
                TimestampUtc = new DateTime(2024, 11, 10, 14, 30, 0, DateTimeKind.Utc),
                Metadata = new Dictionary<string, object>
                {
                    { "vagaId", "000000000000000000000002" },
                    { "origem", "mobile" },
                    { "screen", "JobDetails" }
                }
            };
        }
    }

    public class AppEventUpdateRequestExample : IExamplesProvider<Domain.Entities.AppEvent>
    {
        public Domain.Entities.AppEvent GetExamples()
        {
            return new Domain.Entities.AppEvent
            {
                UsuarioId = "000000000000000000000001",
                Tipo = "apply_click",
                TimestampUtc = new DateTime(2024, 11, 10, 14, 35, 0, DateTimeKind.Utc),
                Metadata = new Dictionary<string, object>
                {
                    { "vagaId", "000000000000000000000002" },
                    { "origem", "mobile" },
                    { "screen", "JobDetails" },
                    { "cta", "ApplyNow" }
                }
            };
        }
    }

    public class AppEventResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                entity = new
                {
                    id = "000000000000000000000020",
                    usuarioId = "000000000000000000000001",
                    tipo = "job_view",
                    timestampUtc = "2024-11-10T14:30:00Z",
                    metadata = new
                    {
                        vagaId = "000000000000000000000002",
                        origem = "mobile",
                        screen = "JobDetails"
                    }
                },
                _links = new[]
                {
                    new { rel = "self",   href = "/api/v1/appevent/000000000000000000000020", method = "GET" },
                    new { rel = "update", href = "/api/v1/appevent/000000000000000000000020", method = "PUT" },
                    new { rel = "delete", href = "/api/v1/appevent/000000000000000000000020", method = "DELETE" }
                }
            };
        }
    }

    public class AppEventListResponseExample : IExamplesProvider<PagedResult<object>>
    {
        public PagedResult<object> GetExamples()
        {
            var items = new[]
            {
                new
                {
                    entity = new
                    {
                        id = "000000000000000000000020",
                        usuarioId = "000000000000000000000001",
                        tipo = "job_view",
                        timestampUtc = "2024-11-10T14:30:00Z",
                        metadata = new
                        {
                            vagaId = "000000000000000000000002",
                            origem = "mobile",
                            screen = "JobDetails"
                        }
                    },
                    _links = new[]
                    {
                        new { rel = "self",   href = "/api/v1/appevent/000000000000000000000020", method = "GET" },
                        new { rel = "update", href = "/api/v1/appevent/000000000000000000000020", method = "PUT" },
                        new { rel = "delete", href = "/api/v1/appevent/000000000000000000000020", method = "DELETE" }
                    }
                }
            };

            return new PagedResult<object>(items, totalItems: 1, page: 1, pageSize: 10);
        }
    }

    public class AppEventDeleteResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                message = "Evento de app removido com sucesso"
            };
        }
    }
}

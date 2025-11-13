using ReStartAI.Application.Dto;

namespace ReStartAI.Application.Helpers
{
    public static class HateoasHelper
    {
        public static object WithLinks<T>(T entity, string basePath, string id)
        {
            var links = new List<HateoasLink>
            {
                new HateoasLink("self", $"{basePath}/{id}", "GET"),
                new HateoasLink("update", $"{basePath}/{id}", "PUT"),
                new HateoasLink("delete", $"{basePath}/{id}", "DELETE")
            };

            return new { entity, _links = links };
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ReStartAI.Api.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string DefaultHeaderName = "x-api-key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var headerName = configuration["Swagger:ApiKeyHeaderName"] ?? DefaultHeaderName;
            var expectedApiKey = configuration["Swagger:ApiKey"];

            if (string.IsNullOrEmpty(expectedApiKey))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(headerName, out var providedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!string.Equals(providedApiKey, expectedApiKey, StringComparison.Ordinal))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
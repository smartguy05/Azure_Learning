using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace AzureLearningWeb.Security
{
    public class ApiKeyPolicy : AuthorizationHandler<ApiKeyRequirement>
    {
        private const string API_KEY_HEADER_NAME = "X-API-KEY";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            if (context.Resource is Endpoint filterContext)
            {
                filterContext.Metadata.
                var apiKey = filterContext.HttpContext.Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
                if (apiKey != null && requirement.ApiKeys.Any(requiredKey => apiKey == requiredKey))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace precioLuzApi.Handlers
{
    public class APIKeyAuthReq : IAuthorizationRequirement { }

    public class APIKeyAuthHandler : AuthorizationHandler<APIKeyAuthReq>
    {
        public IConfiguration Configuration { get; }

        public APIKeyAuthHandler(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, APIKeyAuthReq requirement)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (requirement == null)
                throw new ArgumentNullException(nameof(requirement));

            var httpContext = context.Resource as Microsoft.AspNetCore.Http.DefaultHttpContext;

            var headers = httpContext.HttpContext.Request.Headers;

            if (headers.TryGetValue("Authorization", out StringValues value))
            {
                var token = value.First().Split(" ")[1];

                if (token == Configuration["APIToken"])
                {
                    context.Succeed(requirement);

                    return Task.CompletedTask;
                }
            }

            context.Fail();

            httpContext.HttpContext.Response.StatusCode = 403;

            return Task.CompletedTask;
        }
    }
}

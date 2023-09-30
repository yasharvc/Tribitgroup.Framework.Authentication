using Microsoft.AspNetCore.Http;
using Tribitgroup.Framewok.Identity.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Middlewares
{
    internal class TestMiddleware
    {
        readonly RequestDelegate _next;
        public TestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if(context.User.Identity is not null)
            {
                context.AddClaims(new System.Security.Claims.Claim("test", "yashar"));
            }

            return _next(context);
        }

    }
}
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Tribitgroup.Framewok.Identity.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Middlewares
{
    internal class ClientAuthenticatorMiddleware
    {
        readonly RequestDelegate _next;
        public ClientAuthenticatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if(context.User.Identity is not null)
            {
                if (context.User.Identity?.IsAuthenticated ?? false)
                    return _next(context);
                var identity = new GenericIdentity("yashar", "AuthenticationTypes.Federation");

                var claims = new List<Claim>
                {
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "yashar"),
                    new Claim(ClaimTypes.Role, "Role2"),
                    new Claim("jti", "6b83ebaf-1827-4245-a2d7-b4fa17090655"),
                    new Claim("exp", "1696083359"),
                    new Claim("iss", "https://localhost:7000"),
                    new Claim("aud", "https://localhost:7000")
                };

                var cc = new ClaimsIdentity(identity, claims, "Identity.Application", ClaimTypes.Name, ClaimTypes.Role);

                context.User.AddIdentity(cc);
            }

            return _next(context);
        }

    }
}
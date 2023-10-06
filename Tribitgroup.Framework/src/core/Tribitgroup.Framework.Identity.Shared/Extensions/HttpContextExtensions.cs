using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tribitgroup.Framework.Identity.Shared.Extensions
{
    public static class HttpContextExtensions
    {
        public static HttpContext AddClaims(this HttpContext context, params Claim[] claims)
        {
            context.User.AddIdentity(new  ClaimsIdentity(claims));
            return context;
        }
    }
}
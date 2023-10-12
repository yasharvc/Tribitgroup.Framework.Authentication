using System.Security.Claims;

namespace Tribitgroup.Framework.Identity.Web
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
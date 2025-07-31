using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;

namespace Bookstore.Web.Helpers
{
    public static class OwinRequestExtensions
    {
        public static string GetReturnUrl(this IOwinContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}/signin-oidc";
        }
    }
}
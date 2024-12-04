using Microsoft.AspNetCore.Http;


namespace Bookstore.Web.Helpers
{
    public static class HttpContextExtensions
    {
        public static string GetReturnUrl(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}/signin-oidc";
        }
    }
}
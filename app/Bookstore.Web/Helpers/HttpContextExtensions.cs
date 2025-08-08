using System;
using System.Drawing;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Owin;
using System.Security.Claims;

namespace Bookstore.Web.Helpers
{
    public static class HttpContextCartExtensions
    {
        public static string GetShoppingCartCorrelationId(this HttpContext context)
        {
            var CookieKey = "ShoppingCartId";

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                Path = "/"
            };

            string shoppingCartClientId = null;
            if (context.Request.Cookies.TryGetValue(CookieKey, out string cookieValue))
            {
                shoppingCartClientId = cookieValue;
            }

            if (string.IsNullOrWhiteSpace(shoppingCartClientId))
            {
                shoppingCartClientId = context.User.Identity.IsAuthenticated ?
                    context.User.FindFirstValue("sub") ?? Guid.NewGuid().ToString() :
                    Guid.NewGuid().ToString();
            }

            context.Response.Cookies.Append(CookieKey, shoppingCartClientId, cookieOptions);

            return shoppingCartClientId;
        }
    }
}
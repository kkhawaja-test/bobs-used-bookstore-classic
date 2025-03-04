using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookstore.Domain.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Bookstore.Web.Helpers
{
    public class LocalAuthenticationMiddleware : IMiddleware
    {
        private const string UserId = "FB6135C7-1464-4A72-B74E-4B63D343DD09";

        private readonly ICustomerService _customerService;

        public LocalAuthenticationMiddleware(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.StartsWith("/Authentication/Login"))
            {
                await CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync(context);

                context.Response.Cookies.Append("LocalAuthentication", "true", new CookieOptions { Expires = DateTime.Now.AddDays(1) });

                context.Response.Redirect("/");
            }
            else if (context.Request.Cookies.ContainsKey("LocalAuthentication"))
            {
                await CreateClaimsPrincipal(context);

                await SaveCustomerDetailsAsync(context);

                await next(context);
            }
            else
            {
                await next(context);
            }
        }

        private async Task CreateClaimsPrincipal(HttpContext context)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.Name, "bookstoreuser"));
            identity.AddClaim(new Claim("nameidentifier", UserId));
            identity.AddClaim(new Claim("given_name", "Bookstore"));
            identity.AddClaim(new Claim("family_name", "User"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrators"));

            var principal = new ClaimsPrincipal(identity);
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private async Task SaveCustomerDetailsAsync(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var dto = new CreateOrUpdateCustomerDto(
                    identity.FindFirst("nameidentifier")?.Value,
                    identity.Name,
                    identity.FindFirst("given_name")?.Value,
                    identity.FindFirst("family_name")?.Value);

                await _customerService.CreateOrUpdateCustomerAsync(dto);
            }
        }
    }
}
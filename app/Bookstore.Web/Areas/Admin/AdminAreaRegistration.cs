using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Areas
{
    public static class AdminAreaRegistration
    {
        public static string AreaName => "Admin";

        public static void RegisterArea(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    "Admin_default",
                    "Admin",
                    "Admin/{controller=Home}/{action=Index}/{id?}",
                    new { area = "Admin" }
                );
            });
        }
    }
}
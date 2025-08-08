using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using System;
using NLog;


[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure services here
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure logging
            try
            {
                // Initialize NLog configuration if needed
                LogManager.Configuration = LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
                Console.WriteLine("Logging configured");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring logging: {ex.Message}");
            }

            // Configure application settings
            Console.WriteLine("Configuration initialized");

            // Configure dependency injection
            // (Previously handled by DependencyInjectionSetup.ConfigureDependencyInjection)

            // Configure authentication
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
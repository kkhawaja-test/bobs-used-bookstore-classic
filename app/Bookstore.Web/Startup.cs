using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            // Configure logging
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configure configuration
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IApplicationBuilder app)
        {
            // Configure dependency injection
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IApplicationBuilder app)
        {
            // Configure authentication
        }
    }

    public class Startup
    {
        // This method gets called by the OWIN middleware
        public void Configuration(IAppBuilder app)
        {
            // Forward to the ASP.NET Core Configure method
            // Any OWIN-specific configuration can still be done here
        }

        // This method gets called by the ASP.NET Core runtime
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure services
        }

        // This method gets called by the ASP.NET Core runtime
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}
using Microsoft.AspNetCore.Owin;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Configure authentication
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Configure dependency injection
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Initialize the logging configuration
            // LoggingSetup.ConfigureLogging();

            // ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}
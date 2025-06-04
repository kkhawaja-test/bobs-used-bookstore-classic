using Microsoft.AspNetCore.Owin;
using Microsoft.Owin;
using Owin;
using System;



[assembly: OwinStartup(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    internal static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Implement authentication configuration here
            Console.WriteLine("Authentication configured");
        }
    }

    internal static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            // Implement logging configuration here
            Console.WriteLine("Logging configured");
        }
    }

    internal static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Implement configuration setup here
            Console.WriteLine("Configuration configured");
        }
    }

    internal static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Implement dependency injection configuration here
            Console.WriteLine("Dependency injection configured");
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}
using Microsoft.AspNetCore.Owin;
using Owin;
using NLog;
using System;

[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure logging directly
            ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }

        private void ConfigureLogging()
        {
            try
            {
                // Basic NLog configuration
                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("Application starting up");
            }
            catch (Exception ex)
            {
                // Log to console if there's an issue setting up logging
                Console.WriteLine($"Error configuring logging: {ex}");
            }
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            // Configuration setup logic here
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            // Dependency injection setup logic here
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            // Authentication configuration logic here
        }
    }
}
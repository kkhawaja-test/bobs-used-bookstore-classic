using Microsoft.AspNetCore.Owin;
using NLog;
using Owin;
using System;

[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            try
            {
                // Configure authentication
                // Add authentication setup logic here
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring authentication: {ex.Message}");
            }
        }
    }
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            try
            {
                // Configure NLog
                LogManager.LoadConfiguration("NLog.config");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring logging: {ex.Message}");
            }
        }
    }

    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            try
            {
                // Configure application settings
                // Add configuration loading logic here
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring application settings: {ex.Message}");
            }
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            try
            {
                // Configure dependency injection
                // Add DI container setup logic here
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring dependency injection: {ex.Message}");
            }
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
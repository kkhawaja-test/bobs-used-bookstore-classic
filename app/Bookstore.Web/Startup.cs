using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using NLog.Targets;
using Owin;
using System;
using System.IO;
using Autofac;
using Autofac.Integration.Owin;




[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(Bookstore.Web.Startup))]

namespace Bookstore.Web
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging()
        {
            try
            {
                var config = new LoggingConfiguration();

                // Create targets
                var consoleTarget = new ConsoleTarget("console")
                {
                    Layout = "${longdate} ${level:uppercase=true} ${logger} - ${message} ${exception:format=ToString}"
                };
                config.AddTarget(consoleTarget);

                var fileTarget = new FileTarget("file")
                {
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "bookstore-${shortdate}.log"),
                    Layout = "${longdate} ${level:uppercase=true} ${logger} - ${message} ${exception:format=ToString}",
                    ArchiveFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "archive", "bookstore-{#}.log"),
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    ArchiveEvery = FileArchivePeriod.Day,
                    MaxArchiveFiles = 7
                };
                config.AddTarget(fileTarget);

                // Add rules
                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);

                // Apply config
                LogManager.Configuration = config;

                LogManager.GetCurrentClassLogger().Info("Logging initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring logging: {ex}");
            }
        }
    }

    public static class ConfigurationSetup
    {
        public static IConfiguration Configuration { get; private set; }

        public static void ConfigureConfiguration()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddEnvironmentVariables();

                Configuration = builder.Build();

                LogManager.GetCurrentClassLogger().Info("Configuration initialized");
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Error configuring application settings");
                Console.WriteLine($"Error configuring application settings: {ex}");
            }
        }
    }

    public static class DependencyInjectionSetup
    {
        public static void ConfigureDependencyInjection(IAppBuilder app)
        {
            try
            {
                var builder = new ContainerBuilder();

                // Register dependencies
                // builder.RegisterType<SomeService>().As<ISomeService>();
                // Add more registrations as needed

                var container = builder.Build();

                // Set the dependency resolver for the application
                app.UseAutofacMiddleware(container);

                LogManager.GetCurrentClassLogger().Info("Dependency injection configured");
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Error configuring dependency injection");
                Console.WriteLine($"Error configuring dependency injection: {ex}");
            }
        }
    }

    public static class AuthenticationConfig
    {
        public static void ConfigureAuthentication(IAppBuilder app)
        {
            try
            {
                // Configure authentication here
                // Example: app.UseOpenIdConnectAuthentication(...);

                LogManager.GetCurrentClassLogger().Info("Authentication configured");
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Error configuring authentication");
                Console.WriteLine($"Error configuring authentication: {ex}");
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
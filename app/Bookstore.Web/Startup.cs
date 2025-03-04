using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
        {
// Configure logging using the ILoggerFactory
            loggerFactory.AddConfiguration(app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>().GetSection("Logging"));

            // The rest of your Configure method remains the same
            ConfigurationSetup.ConfigureConfiguration();

            // Note: DependencyInjection setup should be moved to ConfigureServices method

            // Note: Authentication config should be updated to use ASP.NET Core Identity
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add logging services
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>().GetSection("Logging"))
                       .AddConsole()
                       .AddDebug();
            });

            // Move dependency injection setup here
            // DependencyInjectionSetup.ConfigureDependencyInjection(services);

            // Configure authentication here
            // services.AddAuthentication(...);
        }
    }
}
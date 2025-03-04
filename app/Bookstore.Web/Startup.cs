using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bookstore.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            // Note: DependencyInjection setup should be moved to ConfigureServices method

            // Note: Authentication config should be updated to use ASP.NET Core Identity
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Move dependency injection setup here
            // DependencyInjectionSetup.ConfigureDependencyInjection(services);

            // Configure authentication here
            // services.AddAuthentication(...);
        }
    }
}
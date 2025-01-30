using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bookstore.Common;

namespace Bookstore.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure services here
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            // Update these methods to work with IApplicationBuilder instead of IAppBuilder
            DependencyInjectionSetup.ConfigureDependencyInjection(app);

            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}
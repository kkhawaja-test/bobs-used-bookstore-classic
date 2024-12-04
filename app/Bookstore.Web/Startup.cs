using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Bookstore.Web.Setup; // Assuming LoggingSetup is in this namespace

namespace Bookstore.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            LoggingSetup.ConfigureLogging();

            ConfigurationSetup.ConfigureConfiguration();

            DependencyInjectionSetup.ConfigureDependencyInjection(services);

            services.AddAuthentication(); // Placeholder for authentication configuration
        }

        public void Configure(IApplicationBuilder app)
        {
            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}
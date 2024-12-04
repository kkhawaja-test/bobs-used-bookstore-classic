using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

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
            // TODO: Configure logging
            // TODO: Configure application settings
            // TODO: Configure dependency injection

            services.AddAuthentication(); // Placeholder for authentication configuration
        }

        public void Configure(IApplicationBuilder app)
        {
            AuthenticationConfig.ConfigureAuthentication(app);
        }
    }
}
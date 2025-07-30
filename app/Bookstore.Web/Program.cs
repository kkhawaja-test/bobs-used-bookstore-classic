
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bookstore.Web;
using System.Data.Entity;
using Microsoft.AspNetCore.Diagnostics;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration sources
                builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
                builder.Configuration.AddEnvironmentVariables();

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Configure connection string for Entity Framework 6
                var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:BookstoreDatabaseConnection") ??
                    "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

                // Entity Framework 6 doesn't use the AddDbContext method from EF Core
                // Instead, we can store the connection string for later use
                builder.Services.AddSingleton<string>(connectionString);

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Add client-side validation
                builder.Services.AddRazorPages().AddViewOptions(options =>
                {
                    options.HtmlHelperOptions.ClientValidationEnabled =
                        bool.Parse(builder.Configuration.GetValue<string>("ClientValidationEnabled") ?? "true");
                });

                // Register areas
                builder.Services.AddMvc()
                    .AddMvcOptions(options =>
                    {
                        // Add global filters here if needed
                        FilterConfig.RegisterGlobalFilters(options.Filters);
                    });

                // Add logging
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();

                // Add application settings as configuration
                builder.Services.Configure<AppSettings>(builder.Configuration);

                // Configure service implementations based on configuration
                var authService = builder.Configuration.GetValue<string>("Services/Authentication") ?? "local";
                var dbService = builder.Configuration.GetValue<string>("Services/Database") ?? "local";
                var fileService = builder.Configuration.GetValue<string>("Services/FileService") ?? "local";
                var imageValidationService = builder.Configuration.GetValue<string>("Services/ImageValidationService") ?? "local";
                var loggingService = builder.Configuration.GetValue<string>("Services/LoggingService") ?? "local";

                // Register services based on configuration
                if (authService == "aws")
                {
                    // AWS authentication services would be registered here
                }
                else
                {
                    // Local authentication services
                }

                if (fileService == "aws")
                {
                    // AWS file services would be registered here
                }
                else
                {
                    // Local file services
                }

                var app = builder.Build();

                // Configure the HTTP request pipeline (formerly Configure method)
                var environment = app.Configuration.GetValue<string>("Environment") ?? "Development";
                if (environment == "Development" || app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                //Added Middleware

                app.UseRouting();

                app.UseAuthorization();

                // Configure routes
                RouteConfig.RegisterRoutes(app);

                // Configure bundles
                BundleConfig.RegisterBundles(app);

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
        }

        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }

        public class AppSettings
        {
            public string Environment { get; set; }
            public ConnectionStrings ConnectionStrings { get; set; }
            public Services Services { get; set; }
            public Authentication Authentication { get; set; }
            public Files Files { get; set; }
            public bool ClientValidationEnabled { get; set; } = true;
        }

        public class ConnectionStrings
        {
            public string BookstoreDatabaseConnection { get; set; }
        }

        public class Services
        {
            public string Authentication { get; set; }
            public string Database { get; set; }
            public string FileService { get; set; }
            public string ImageValidationService { get; set; }
            public string LoggingService { get; set; }
        }

        public class Authentication
        {
            public Cognito Cognito { get; set; }
        }

        public class Cognito
        {
            public string LocalClientId { get; set; }
            public string AppRunnerClientId { get; set; }
            public string MetadataAddress { get; set; }
            public string CognitoDomain { get; set; }
        }

        public class Files
        {
            public string BucketName { get; set; }
            public string CloudFrontDomain { get; set; }
        }
    }

    namespace Bookstore.Web
    {
        public class CustomExceptionFilter : ExceptionFilterAttribute
        {
            public override void OnException(ExceptionContext context)
            {
                context.ExceptionHandled = true;
                context.Result = new ViewResult { ViewName = "Error" };
            }
        }

        public static class FilterConfig
        {
            public static void RegisterGlobalFilters(FilterCollection filters)
            {
                filters.Add(new CustomExceptionFilter());
            }
        }

        public static class RouteConfig
        {
            public static void RegisterRoutes(WebApplication app)
            {
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Add additional routes here if needed
            }
        }

        public static class BundleConfig
        {
            public static void RegisterBundles(WebApplication app)
            {
                // Bundle configuration is handled differently in ASP.NET Core
                // Use built-in bundling and minification or third-party libraries like WebOptimizer
            }
        }

        public static class AreaRegistration
        {
            public static void RegisterAllAreas()
            {
                // Areas are registered automatically in ASP.NET Core
                // This method is kept for compatibility
            }
        }
    }

    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using System.Data.Entity;
using WebOptimizer;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration sources
                builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
                builder.Configuration.AddEnvironmentVariables();

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Configure connection strings
                var connectionString = builder.Configuration["ConnectionStrings:BookstoreDatabaseConnection"]
                    ?? "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Configure MVC with client validation settings from Web.config
                builder.Services.AddMvc()
                    .AddMvcOptions(options =>
                    {
                        // Add global filters equivalent to FilterConfig.RegisterGlobalFilters
                    });

                // Configure client-side validation (from Web.config)
                builder.Services.Configure<MvcViewOptions>(options => {
                    options.HtmlHelperOptions.ClientValidationEnabled =
                        bool.TryParse(builder.Configuration["ClientValidationEnabled"], out bool clientValidationEnabled)
                        ? clientValidationEnabled : true;
                });

                // Add bundling services (alternative to BundleConfig.RegisterBundles)
                builder.Services.AddWebOptimizer(pipeline =>
                {
                    // Configure your bundles here
                    // Example: pipeline.AddCssBundle("/css/bundle.css", "wwwroot/css/**/*.css");
                    // Example: pipeline.AddJsBundle("/js/bundle.js", "wwwroot/js/**/*.js");
                });

                // Configure service modes from Web.config
                var authenticationMode = builder.Configuration["Services:Authentication"] ?? "local";
                var databaseMode = builder.Configuration["Services:Database"] ?? "local";
                var fileServiceMode = builder.Configuration["Services:FileService"] ?? "local";
                var imageValidationMode = builder.Configuration["Services:ImageValidationService"] ?? "local";
                var loggingMode = builder.Configuration["Services:LoggingService"] ?? "local";

                // Configure Entity Framework (not upgrading to EF Core as per requirements)
                // EntityFramework 6 can be used with .NET Core through the EntityFramework6.Npgsql package

                //Added Services
                
                var app = builder.Build();
                
                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment())
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

                // Use bundling middleware (WebOptimizer)
                app.UseWebOptimizer();

                // Configure error handling
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        logger.LogError(exception, "An unhandled exception occurred");

                        // Redirect or show error page as needed
                    });
                });

                //Added Middleware

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Configure additional routes if needed

                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }

            // Helper methods to access configuration values
            public static string GetConnectionString(string name)
            {
                return Configuration.GetConnectionString(name);
            }

            public static string GetAppSetting(string key)
            {
                return Configuration[key];
            }

            public static T GetAppSetting<T>(string key, T defaultValue)
            {
                if (Configuration[key] == null)
                    return defaultValue;

                try
                {
                    return (T)Convert.ChangeType(Configuration[key], typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
    }

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
using Microsoft.AspNetCore.Http;
using System.Data.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebOptimizer;

    namespace Bookstore.Web
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration from appsettings.json and environment variables
                builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();

                // Configure Entity Framework 6
                // No need to register DbContext with DI as EF6 doesn't use the same pattern as EF Core
                // Connection strings will be read from configuration by EF6 automatically
                System.Data.Entity.Database.SetInitializer(new System.Data.Entity.CreateDatabaseIfNotExists<System.Data.Entity.DbContext>());

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add application settings from Web.config
                builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Configure client-side validation
                builder.Services.AddMvc()
                    .AddViewOptions(options =>
                    {
                        options.HtmlHelperOptions.ClientValidationEnabled =
                            bool.Parse(builder.Configuration["AppSettings:ClientValidationEnabled"] ?? "true");
                    })
                    .AddMvcOptions(options =>
                    {
                        // Add global filters here if needed
                    });

                // Add authentication
                string authService = builder.Configuration["AppSettings:Services/Authentication"] ?? "local";
                if (authService == "aws")
                {
                    // AWS Cognito authentication would be configured here
                }
                else
                {
                    // Local authentication
                    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.LoginPath = "/Account/Login";
                            options.LogoutPath = "/Account/Logout";
                        });
                }

                // Add bundling services (alternative to ASP.NET Framework BundleConfig)
                builder.Services.AddWebOptimizer(pipeline =>
                {
                    // Configure bundles here
                    // Example: pipeline.AddJavaScriptBundle("/js/bundle.js", "js/**/*.js");
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline (formerly Configure method)
                string environment = app.Configuration["AppSettings:Environment"] ?? "Production";
                if (app.Environment.IsDevelopment() || environment == "Development")
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

                // Use WebOptimizer middleware (replacement for BundleConfig)
                app.UseWebOptimizer();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Map area routes
                app.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "{area}",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Configure global error handling
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

                        if (exceptionHandlerPathFeature?.Error != null)
                        {
                            logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception");
                        }
                    });
                });

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
            public bool ClientValidationEnabled { get; set; } = true;

            // Service configuration
            public string ServicesAuthentication { get; set; } = "local";
            public string ServicesDatabase { get; set; } = "local";
            public string ServicesFileService { get; set; } = "local";
            public string ServicesImageValidationService { get; set; } = "local";
            public string ServicesLoggingService { get; set; } = "local";

            // Authentication settings
            public string AuthenticationCognitoLocalClientId { get; set; }
            public string AuthenticationCognitoAppRunnerClientId { get; set; }
            public string AuthenticationCognitoMetadataAddress { get; set; }
            public string AuthenticationCognitoCognitoDomain { get; set; }

            // File service settings
            public string FilesBucketName { get; set; }
            public string FilesCloudFrontDomain { get; set; }
        }
    }
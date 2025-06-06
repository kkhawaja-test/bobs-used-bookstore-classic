
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

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add connection string from Web.config
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> {
                    { "ConnectionStrings:BookstoreDatabaseConnection", "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;" },
                    { "Environment", "Development" },
                    { "Services:Authentication", "local" },
                    { "Services:Database", "local" },
                    { "Services:FileService", "local" },
                    { "Services:ImageValidationService", "local" },
                    { "Services:LoggingService", "local" },
                    { "ClientValidationEnabled", "true" }
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Configure MVC settings
                builder.Services.AddMvc(options =>
                {
                    // Add global filters here as needed
                })
                .AddViewOptions(options =>
                {
                    options.HtmlHelperOptions.ClientValidationEnabled =
                        bool.Parse(builder.Configuration["ClientValidationEnabled"] ?? "true");
                });

                // Register areas
                builder.Services.AddMvc()
                    .AddControllersAsServices();

                // Configure logging based on environment
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();

                // Add Entity Framework if used
                if (builder.Configuration["Services:Database"] == "local")
                {
                    // Entity Framework 6 is still used with connection string
                    // The actual initialization will happen on first DB access through context
                }

                var app = builder.Build();

                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment() || builder.Configuration["Environment"] == "Development")
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

                // Register routes (formerly RouteConfig.RegisterRoutes)
                app.UseRouting();

                // Configure authentication based on settings
                if (builder.Configuration["Services:Authentication"] == "aws")
                {
                    // Would set up AWS Cognito authentication here
                }

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Register areas (formerly AreaRegistration.RegisterAllAreas)
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Handle application errors
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;

                        if (exception != null)
                        {
                            logger.LogError(exception, "Unhandled exception");
                        }

                        await Task.CompletedTask;
                    });
                });
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
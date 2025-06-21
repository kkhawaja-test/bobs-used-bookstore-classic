
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

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                
                // Add configuration
                builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                       .AddEnvironmentVariables();

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add connection string from Web.config
                var connectionString = builder.Configuration["ConnectionStrings:BookstoreDatabaseConnection"]
                    ?? "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews(options =>
                {
                    // Configure client validation from Web.config
                    if (builder.Configuration.GetValue<bool>("ClientValidationEnabled", true))
                    {
                        // Client validation is enabled by default
                    }
                });

                builder.Services.AddRazorPages();

                // Register area routes
// Exception handling is configured using app.UseExceptionHandler instead
                builder.Services.Configure<MvcOptions>(options =>
                {
                    // HandleErrorAttribute is replaced by UseExceptionHandler middleware
                });

                // Add antiforgery to views
                builder.Services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

                // Configure environment from Web.config
                var environment = builder.Configuration["Environment"] ?? "Development";

                // Configure services based on Web.config settings
                var authService = builder.Configuration["Services:Authentication"] ?? "local";
                var databaseService = builder.Configuration["Services:Database"] ?? "local";
                var fileService = builder.Configuration["Services:FileService"] ?? "local";
                var imageValidationService = builder.Configuration["Services:ImageValidationService"] ?? "local";
                var loggingService = builder.Configuration["Services:LoggingService"] ?? "local";

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
                
                //Added Middleware

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                // Configure error logging
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;
                        logger.LogError(exception, "An unhandled exception occurred");
                        await Task.CompletedTask;
                    });
                });
                
                // Register routes (from RouteConfig.RegisterRoutes in Global.asax.cs)
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();

                // Register areas (from AreaRegistration.RegisterAllAreas in Global.asax.cs)
                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
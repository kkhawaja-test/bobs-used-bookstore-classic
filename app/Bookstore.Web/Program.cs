
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

    namespace Bookstore.Web
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                
                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;
                
                // Add connection string for EntityFramework 6
                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";

// Register DbContext using standard service registration
                builder.Services.AddScoped<DbContext>(_ =>
                {
                    // Use the original EntityFramework (not Core)
                    var context = new DbContext(connectionString);
                    return context;
                });

                // Add application settings
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Environment", "Development"},
                    {"Services:Authentication", "local"},
                    {"Services:Database", "local"},
                    {"Services:FileService", "local"},
                    {"Services:ImageValidationService", "local"},
                    {"Services:LoggingService", "local"}
                });

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Register areas, filters, routes, and bundles
                builder.Services.AddMvc()
                    .AddMvcOptions(options =>
                    {
                        // Register global filters (equivalent to FilterConfig.RegisterGlobalFilters)
                    });

                // Configure client-side validation (from Web.config appSettings)
                builder.Services.AddRazorPages()
                    .AddViewOptions(options =>
                    {
                        options.HtmlHelperOptions.ClientValidationEnabled = true;
                    });

                // Add logging
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                
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
                
                // Configure routes (equivalent to RouteConfig.RegisterRoutes)

                app.UseRouting();

                app.UseAuthorization();

                // EntityFramework configuration from Web.config
                // Note: Most EF configuration is now done via DbContext options or Fluent API
                // We're keeping the original EntityFramework (not Core)

                // Add global error handling
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        var logger = app.Services.GetService<ILogger<Program>>();
                        logger?.LogError(ex, "An unhandled exception occurred.");
                        throw;
                    }
                });
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Map area routes (equivalent to AreaRegistration.RegisterAllAreas)
                app.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
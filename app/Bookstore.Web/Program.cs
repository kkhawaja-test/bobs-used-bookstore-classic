
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

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration values from Web.config appSettings
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "webpages:Version", "3.0.0.0" },
                    { "webpages:Enabled", "false" },
                    { "ClientValidationEnabled", "true" },
                    { "Environment", "Development" },
                    { "Services:Authentication", "local" },
                    { "Services:Database", "local" },
                    { "Services:FileService", "local" },
                    { "Services:ImageValidationService", "local" },
                    { "Services:LoggingService", "local" },
                    { "Authentication:Cognito:LocalClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Authentication:Cognito:AppRunnerClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Authentication:Cognito:MetadataAddress", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Authentication:Cognito:CognitoDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']" },
                    { "Files:BucketName", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']" },
                    { "Files:CloudFrontDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']" }
                });

                // Configure EntityFramework 6 (not EF Core)
                var connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";
                // Register your DbContext as a service
                builder.Services.AddScoped<System.Data.Entity.DbContext>(_ => {
                    // Create and return your EF6 DbContext instance
                    // This is a placeholder - you need to replace with your actual DbContext implementation
                    return null; // Replace with actual DbContext instance creation
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();
                builder.Services.AddRazorPages();

                // Register area services
                builder.Services.AddMvc()
                    .AddMvcOptions(options => {
                        // RegisterGlobalFilters equivalent
                        // Add any global filters here if needed
                    });

                // BundleConfig equivalent is no longer needed as it's handled by wwwroot and middleware
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

                app.UseAuthorization();

                // Configure error handling middleware to log exceptions
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        // Log the error (equivalent to Application_Error)
                        var logger = app.Services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An unhandled exception occurred.");
                        throw;
                    }
                });
                
                // Register routes (equivalent to RouteConfig.RegisterRoutes)
                // Configure client validation from Web.config settings
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments("/bundles"))
                    {
                        // Handle legacy bundles if needed
                    }
                    await next();
                });

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Register area routes
                app.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                app.MapRazorPages();
                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
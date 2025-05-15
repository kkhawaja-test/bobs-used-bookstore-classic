
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
using WebOptimizer;

    namespace Bookstore
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add configuration from appSettings in Web.config
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

                // Add connection string from Web.config
                // Configure database connection for Entity Framework 6
                var connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;";
                System.Data.Entity.Database.SetInitializer(new System.Data.Entity.CreateDatabaseIfNotExists<System.Data.Entity.DbContext>());

                // Register database context in DI container
                builder.Services.AddSingleton<string>(connectionString);

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews()
                    .AddMvcOptions(options =>
                    {
                        // Enable client validation
                        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
                            x => "The value is invalid");
                    });

                // Register Areas
                builder.Services.AddMvc()
                    .AddControllersAsServices();

                // Entity Framework 6 doesn't require explicit registration with ASP.NET Core DI

                // Bundle and Minification (equivalent to BundleConfig.RegisterBundles)
                builder.Services.AddWebOptimizer(pipeline =>
                {
                    // Add your bundle configurations here if needed
                });

                // Add logging
                builder.Logging.ClearProviders();
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

                // Enable bundling and minification middleware
                app.UseWebOptimizer();

                app.UseRouting();
                
                app.UseAuthorization();
                
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Configure area routing
                app.MapAreaControllerRoute(
                    name: "areas",
                    areaName: "{area}",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Global error handling
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next(context);
                    }
                    catch (Exception ex)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An unhandled exception occurred");
                        throw;
                    }
                });

                
                app.Run();
            }
        }
        
        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }
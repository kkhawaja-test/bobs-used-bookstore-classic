
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

                // Add connection string from web.config
                builder.Configuration.AddInMemoryCollection(new Dictionary<string, string> {
                    {"ConnectionStrings:BookstoreDatabaseConnection", "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=BookStoreClassic;MultipleActiveResultSets=true;Integrated Security=SSPI;"},
                    {"Environment", "Development"},
                    {"ClientValidationEnabled", "true"},
                    {"Services:Authentication", "local"},
                    {"Services:Database", "local"},
                    {"Services:FileService", "local"},
                    {"Services:ImageValidationService", "local"},
                    {"Services:LoggingService", "local"},
                    {"Authentication:Cognito:LocalClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Authentication:Cognito:AppRunnerClientId", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Authentication:Cognito:MetadataAddress", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Authentication:Cognito:CognitoDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/Authentication == 'aws']"},
                    {"Files:BucketName", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']"},
                    {"Files:CloudFrontDomain", "[Retrieved from AWS Systems Manager Parameter Store when Services/FileService == 'aws']"}
                });

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Entity Framework 6 doesn't need service registration in the same way as EF Core
                // DbContext instances will be created through their constructors

                // Register areas
                builder.Services.AddMvc()
                    .AddMvcOptions(options => {
                        // Configure MVC options here if needed
                    });

                // Configure bundling if needed
// In ASP.NET Core, bundling is done differently - consider using tools like WebOptimizer

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

                // Configure custom error handling middleware
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        var logger = app.Services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An unhandled exception occurred");
                        throw;
                    }
                });

                app.UseAuthorization();
                
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
    }
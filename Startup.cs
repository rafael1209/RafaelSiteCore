using MongoDB.Driver.Core.Configuration;
using System.Net;
using System;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace RafaelSiteCore
{
        public class Startup
        {
                private readonly IConfiguration configuration;

                public static string? clientId { get; set; }
                public static string? clientSecret { get; set; }
                public static string? redirectUrl { get; set; }
                public static string? connectionString { private get; set; }
                public static string? mongoDbName { get; set; }

                public Startup()
                {
                        configuration = new ConfigurationBuilder()
                                .AddEnvironmentVariables()
                                .AddJsonFile("appsettings.json", optional: true)
                                .Build();

                        clientId = configuration.GetValue<string>("ClientId");
                        clientSecret = configuration.GetValue<string>("ClientSecret");
                        redirectUrl = configuration.GetValue<string>("RedirectUrl");
                        connectionString = configuration.GetValue<string>("ConnectionStrings:MongoDbConnectionString");
                        mongoDbName = configuration.GetValue<string>("ConnectionStrings:MongoDbName");
                }

                public void ConfigureServices(IServiceCollection services)
                {
                        services.AddControllers();
                        services.AddEndpointsApiExplorer();
                        services.AddSwaggerGen();
                    
                        services.AddSingleton<DiscordApiClient>(sp =>
                              new DiscordApiClient(clientId ?? "", clientSecret ?? "", redirectUrl ?? ""));
                        services.AddSingleton<DiscordAuthLogic>();
                        services.AddSingleton<BlogLogic>();
                        services.AddSingleton<AuthorizeDbContext>(sp =>
                              new AuthorizeDbContext(connectionString ?? "", mongoDbName ?? ""));
                        services.AddSingleton<BlogDbContext>(sp =>
                              new BlogDbContext(connectionString ?? "", mongoDbName ?? ""));
                }

                public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
                {
                        if (env.IsDevelopment())
                        {
                                app.UseDeveloperExceptionPage();
                                app.UseHsts();
                        }


                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseSwagger();
                        app.UseSwaggerUI();
                        app.UseCors(k => { k.WithMethods("POST", "GET", "PATCH", "PUT"); k.AllowAnyOrigin(); k.AllowAnyHeader(); });
                        app.UseEndpoints(endpoints =>
                        {
                                endpoints.MapDefaultControllerRoute().AllowAnonymous();
                                endpoints.MapSwagger();
                                endpoints.MapControllers().AllowAnonymous();
                        });
                }
        }
}

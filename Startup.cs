﻿using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using Microsoft.OpenApi.Models;
using RafaelSiteCore.Interfaces;
using RafaelSiteCore.Services.GoogleDrive;
using RafaelSiteCore.Model.GoogleDriveCredentials;
using Microsoft.Extensions.Caching.Memory;
using RafaelSiteCore.Services.Logger;

namespace RafaelSiteCore
{
    public class Startup
    {
        private readonly IConfiguration configuration;

                public static ulong? clientId { get; set; }
                public static string? clientSecret { get; set; }
                public static string? redirectUrl { get; set; }
                public static string? connectionString { private get; set; }
                public static string? mongoDbName { get; set; }
                public static string? discordWebhook {  get; set; }
                public static string? googleDriveFolderId { get; set; }
                public static GoogleDriveCredentials? googleDriveCredentials { get; set; }

                public Startup()
                {
                        configuration = new ConfigurationBuilder()
                            .AddEnvironmentVariables()
                            .AddJsonFile("appsettings.json", optional: true)
                            .Build();

                        clientId = configuration.GetValue<ulong>("ClientId");
                        clientSecret = configuration.GetValue<string>("ClientSecret");
                        redirectUrl = configuration.GetValue<string>("RedirectUrl");
                        connectionString = configuration.GetValue<string>("ConnectionStrings:MongoDbConnectionString");
                        mongoDbName = configuration.GetValue<string>("ConnectionStrings:MongoDbName");
                        discordWebhook = configuration.GetValue<string>("DiscordWebhook");
                        googleDriveFolderId = configuration.GetValue<string>("GoogleDrive:FolderId");
                        googleDriveCredentials = new GoogleDriveCredentials
                        {
                                Type = configuration.GetValue<string>("GoogleDrive:Type")!,
                                ProjectId = configuration.GetValue<string>("GoogleDrive:ProjectId")!,
                                PrivateKeyId = configuration.GetValue<string>("GoogleDrive:PrivateKeyId")!,
                                PrivateKey = configuration.GetValue<string>("GoogleDrive:PrivateKey")!,
                                ClientEmail = configuration.GetValue<string>("GoogleDrive:ClientEmail")!,
                                ClientId = configuration.GetValue<string>("GoogleDrive:ClientId")!,
                                AuthUri = configuration.GetValue<string>("GoogleDrive:AuthUri")!,
                                TokenUri = configuration.GetValue<string>("GoogleDrive:TokenUri")!,
                                AuthProviderX509CertUrl = configuration.GetValue<string>("GoogleDrive:AuthProviderX509CertUrl")!,
                                ClientX509CertUrl = configuration.GetValue<string>("GoogleDrive:ClientX509CertUrl")!,
                                UniverseDomain = configuration.GetValue<string>("GoogleDrive:UniverseDomain")!
                        };
                }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                {
                                    {
                                        new OpenApiSecurityScheme
                                        {
                                            Name = "Bearer",
                                            In = ParameterLocation.Header,
                                            Reference = new OpenApiReference
                                            {
                                                Id = "Bearer",
                                                Type = ReferenceType.SecurityScheme
                                            }
                                        },
                                        new List<string>()
                                    }
                                });
                        });
                        services.AddHttpContextAccessor();
                        services.AddSingleton<DiscordApiClient>(sp =>
                            new DiscordApiClient(clientId ?? 0, clientSecret ?? "", redirectUrl ?? ""));
                        services.AddSingleton<DiscordAuthLogic>();
                        services.AddSingleton<BlogLogic>();
                        services.AddSingleton<AuthorizeDbContext>(sp =>
                            new AuthorizeDbContext(connectionString ?? "", mongoDbName ?? ""));
                        services.AddSingleton<IStorage, GoogleDriveService>(sp =>
                            new GoogleDriveService(googleDriveCredentials ?? new GoogleDriveCredentials { }, googleDriveFolderId ?? ""));

                        services.AddMemoryCache();

                        services.AddSingleton<BlogDbContext>(sp =>
                            new BlogDbContext(connectionString ?? "", mongoDbName ?? "", sp.GetRequiredService<IMemoryCache>()));

                        services.AddSingleton<DiscordAlert>(sp => new DiscordAlert(discordWebhook ?? ""));
                }


                public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
                {
                        //if (env.IsDevelopment())
                        {
                                app.UseDeveloperExceptionPage();
                                app.UseSwagger();
                                app.UseSwaggerUI();
                                app.UseHsts();
                        }

                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseCors(k => { k.WithMethods("POST", "GET", "PATCH", "PUT"); k.AllowAnyOrigin(); k.AllowAnyHeader(); });
                        app.UseEndpoints(endpoints =>
                        {
                                endpoints.MapDefaultControllerRoute().AllowAnonymous();
                                if (env.IsDevelopment())
                                {
                                        endpoints.MapSwagger();
                                }
                                endpoints.MapControllers().AllowAnonymous();
                        });
                }
        }
}

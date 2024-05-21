
using RafaelSiteCore.Controllers;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Services.Auth;

namespace RafaelSiteCore
{
        public class Program
        {
                public static void Main(string[] args)
                {
                        var builder = WebApplication.CreateBuilder(args);

                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();

                        builder.Services.AddSingleton<DiscordApiClient>();
                        builder.Services.AddSingleton<DiscordAuthLogic>();

                        _ = builder.Services.AddSingleton<AuthorizeDbContext>(sp =>
                              new AuthorizeDbContext(builder.Configuration.GetConnectionString("MongoDbConnectionString"), builder.Configuration.GetConnectionString("MongoDbName")));

                        var app = builder.Build();

                        if (app.Environment.IsDevelopment())
                        {
                                
                        }

                        app.UseSwagger();
                        app.UseSwaggerUI();

                        app.UseAuthorization();


                        app.MapControllers();

                        app.Run();
                }
        }
}

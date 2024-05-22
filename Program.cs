
using RafaelSiteCore.Controllers;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;

namespace RafaelSiteCore
{
        public class Program
        {
                public static void Main(string[] args)
                {
                        var builder = WebApplication.CreateBuilder(args);

                        string mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDbConnectionString") ?? throw new ArgumentNullException("MongoDbConnectionString", "MongoDB connection string is not configured.");
                        string mongoDbName = builder.Configuration.GetConnectionString("MongoDbName") ?? throw new ArgumentNullException("MongoDbName", "MongoDB database name is not configured.");                  

                        builder.Services.AddControllers();
                        builder.Services.AddEndpointsApiExplorer();
                        builder.Services.AddSwaggerGen();

                        builder.Services.AddSingleton<DiscordApiClient>();
                        builder.Services.AddSingleton<DiscordAuthLogic>();
                        builder.Services.AddSingleton<BlogLogic>();

                        _ = builder.Services.AddSingleton<AuthorizeDbContext>(sp =>
                              new AuthorizeDbContext(mongoDbConnectionString, mongoDbName));
                        _ = builder.Services.AddSingleton<BlogDbContext>(sp =>
                              new BlogDbContext(mongoDbConnectionString, mongoDbName));

                        var app = builder.Build();

                        if (app.Environment.IsDevelopment())
                        {

                        }

                        app.UseSwagger();
                        app.UseSwaggerUI();
                        //==============

                        app.UseAuthorization();


                        app.MapControllers();

                        app.Run();
                }
        }
}

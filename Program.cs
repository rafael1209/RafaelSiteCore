
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
                        CreateHostBuilder()
                                .Build()
                                .Run();
                }

                private static IHostBuilder CreateHostBuilder()
                {
                        return Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webHost => {
                                webHost.UseStartup<Startup>();
                        });
                }
        }
}
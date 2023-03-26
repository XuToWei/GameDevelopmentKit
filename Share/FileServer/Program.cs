using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FileServer
{
    public class Program
    {
        public static Config Config;
        
        public static void Main(string[] args)
        {
            Config = new Config();
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseUrls(Config.Url)
                .UseStartup<Startup>();
        }
    }
}
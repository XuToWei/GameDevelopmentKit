using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FileServer
{
    public class Program
    {
        public static Config Config;

        public static void Main(string[] args)
        {
            Config = new Config();
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenAnyIP(Config.Port);
                    })
                    .UseStartup<Startup>();
                });
        }
    }
}

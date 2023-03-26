using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDirectoryBrowser();  //开启目录浏览
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            List<string> fileDirs = new List<string>();
            foreach (var directoryPath in Program.Config.DirectoryPaths)
            {
                string fileDir = new DirectoryInfo(directoryPath).FullName;
                if (Directory.Exists(fileDir))
                {
                    UseStaticFiles(app, fileDir);
                    fileDirs.Add(fileDir);
                }
            }
            if (fileDirs.Count < 1)
            {
                throw new Exception("File directory is empty");
            }
            foreach (var dir in fileDirs)
            {
                Console.WriteLine($"File directory:{dir}");
            }
            app.Run(async (context) => { await context.Response.WriteAsync("Welcome to the ET file server!"); });
        }

        private void UseStaticFiles(IApplicationBuilder app, string filePath)
        {
            var staticfile = new StaticFileOptions
            {
                ServeUnknownFileTypes = true, 
                FileProvider = new PhysicalFileProvider(filePath),
                DefaultContentType = "application/x-msdownload"
            };
            // 设置MIME类型类型
            staticfile.ContentTypeProvider = new FileExtensionContentTypeProvider
            {    
                Mappings =
                {
                    ["*"] = "application/x-msdownload"
                }
            };
            app.UseDirectoryBrowser(new DirectoryBrowserOptions(){ FileProvider = staticfile.FileProvider });
            app.UseStaticFiles(staticfile);
        }
    }
}
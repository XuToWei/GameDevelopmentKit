using System.Text;
using ET.Server.Admin.Components;
using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;

namespace ET
{
    /// <summary>
    /// Admin Web 服务器组件，负责启动 ASP.NET Core Blazor 管理后台
    /// </summary>
    public class AdminComponent : Singleton<AdminComponent>, ISingletonAwake
    {
        public AdminActorService AdminActorService { get; private set; }
        public AgentActorService AgentActorService { get; private set; }

        public void Awake()
        {
        }

        public async UniTask StartAsync()
        {
            // 不传 ET 命令行参数给 ASP.NET Core，避免解析冲突
            var builder = WebApplication.CreateBuilder();

            builder.WebHost.UseStaticWebAssets();
            builder.WebHost.UseUrls("http://*:5200");

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
            });

            var jwtKey = builder.Configuration["Jwt:Key"] ?? "Admin_DefaultSecretKey_ChangeInProduction_32chars!";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Admin";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<AdminActorService>();
            builder.Services.AddSingleton<AgentActorService>();
            builder.Services.AddSingleton<AdminDatabase>();
            builder.Services.AddSingleton<LogService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ProcessManagerService>();
            builder.Services.AddSingleton<ServerMonitorService>();
            builder.Services.AddSingleton<ServerMonitorNotifier>();
            builder.Services.AddHostedService<ServerMonitorBackgroundService>();
            builder.Services.AddSingleton<VersionService>();
            builder.Services.AddSingleton<PlayerService>();
            builder.Services.AddSingleton<ConfigService>();
            builder.Services.AddSingleton<HotReloadService>();
            builder.Services.AddSingleton<LocalProcessService>();
            builder.Services.AddScoped<AuthenticationStateProvider, AdminAuthStateProvider>();
            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/healthz");
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            using (var scope = app.Services.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
                await authService.InitializeAsync();
            }

            this.AdminActorService = app.Services.GetRequiredService<AdminActorService>();
            this.AgentActorService = app.Services.GetRequiredService<AgentActorService>();

            // 非阻塞启动
            await app.StartAsync();

            Log.Info("Admin web server started");
        }
    }
}

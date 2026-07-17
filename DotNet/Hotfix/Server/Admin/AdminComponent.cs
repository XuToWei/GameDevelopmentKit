using ET.Server.Admin.Components;
using Cysharp.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
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

            // Windows EventLog may be unavailable for service accounts. Console logging is
            // deterministic and is already collected by the process supervisor.
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.WebHost.UseStaticWebAssets();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddCascadingAuthenticationState();

            var dataProtectionPath = Path.Combine(AppContext.BaseDirectory, "Data", "DataProtectionKeys");
            Directory.CreateDirectory(dataProtectionPath);
            var dataProtection = builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
                .SetApplicationName("GameDevelopmentKit.Admin");
            if (OperatingSystem.IsWindows())
            {
                dataProtection.ProtectKeysWithDpapi(protectToLocalMachine: true);
            }

            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
            });

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
                {
                    cookieOptions.Cookie.Name = "GDK.Admin.Session";
                    cookieOptions.Cookie.HttpOnly = true;
                    cookieOptions.Cookie.SameSite = SameSiteMode.Strict;
                    cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    cookieOptions.LoginPath = "/login";
                    cookieOptions.AccessDeniedPath = "/login";
                    cookieOptions.ExpireTimeSpan = TimeSpan.FromHours(8);
                    cookieOptions.SlidingExpiration = true;
                });

            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<AdminActorService>();
            builder.Services.AddSingleton<AgentActorService>();
            builder.Services.AddSingleton<AdminDatabase>();
            builder.Services.AddSingleton<LogService>();
            // Authentication state belongs to one Blazor circuit. A singleton would leak a
            // logged-in user to every connected browser session.
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddSingleton<ProcessManagerService>();
            builder.Services.AddSingleton<ServerMonitorService>();
            builder.Services.AddSingleton<ServerMonitorNotifier>();
            builder.Services.AddHostedService<ServerMonitorBackgroundService>();
            builder.Services.AddSingleton<VersionService>();
            builder.Services.AddSingleton<PlayerService>();
            builder.Services.AddSingleton<ConfigService>();
            builder.Services.AddSingleton<HotReloadService>();
            builder.Services.AddScoped<AuthenticationStateProvider, AdminAuthStateProvider>();
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(VersionController).Assembly);
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
                authService.Initialize();
            }

            this.AdminActorService = app.Services.GetRequiredService<AdminActorService>();
            this.AgentActorService = app.Services.GetRequiredService<AgentActorService>();

            // 非阻塞启动
            await app.StartAsync();

            Log.Info("Admin web server started");
        }
    }
}

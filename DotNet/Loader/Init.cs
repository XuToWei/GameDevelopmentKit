using System;
using CommandLine;
using Cysharp.Threading.Tasks;

namespace ET
{
    public class Init
    {
        public async UniTask StartAsync()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Log.Error(e.ExceptionObject.ToString());
                };
                
                // 命令行参数
                Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs())
                        .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                        .WithParsed((o) => World.Instance.AddSingleton(o));
                var log = new NLogger(Options.Instance.AppType.ToString(), Options.Instance.Process, 0);
                World.Instance.AddSingleton<Logger, ILog>(log);
                World.Instance.AddSingleton<TimeInfo, ITimeNow>(new TimeNow());
                World.Instance.AddSingleton<FiberManager>();
                World.Instance.AddSingleton<ConfigComponent, IConfigReader>(new ConfigReader());
                World.Instance.AddSingleton<CodeLoaderComponent, ICodeLoader>(new CodeLoader());
                
                await CodeLoaderComponent.Instance.StartAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Update()
        {
            TimeInfo.Instance.Update();
            FiberManager.Instance.Update();
        }

        public void LateUpdate()
        {
            FiberManager.Instance.LateUpdate();
        }
    }
}
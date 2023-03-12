using System;
using System.Threading;
using CommandLine;

namespace ET
{
    public static class Init
    {
        public static async void StartAsync()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };

                // 异步方法全部会回掉到主线程
                Game.AddSingleton<MainThreadSynchronizationContext>();

                // 命令行参数
                Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
                        .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                        .WithParsed(Game.AddSingleton);

                Game.AddSingleton<TimeInfo>().ITimeNow = new TimeNow();
                Game.AddSingleton<Logger>().ILog =
                        new NLogger(Options.Instance.AppType.ToString(), Options.Instance.Process, "../Config/NLog/NLog.config");
                Game.AddSingleton<ObjectPool>();
                Game.AddSingleton<IdGenerater>();
                Game.AddSingleton<EventSystem>();
                Game.AddSingleton<TimerComponent>();
                Game.AddSingleton<CoroutineLockComponent>();
                Game.AddSingleton<CodeLoaderComponent>().SetCodeLoader(new CodeLoader());
                
                Log.Console($"{Parser.Default.FormatCommandLine(Options.Instance)}");
                
                await CodeLoaderComponent.Instance.StartAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    Game.Update();
                    Game.LateUpdate();
                    Game.FrameFinishUpdate();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using CommandLine;
using Cysharp.Threading.Tasks;

namespace ET
{
    public static class Init
    {
        public static async void StartAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };

            try
            {
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
                Game.AddSingleton<Root>();

                Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Init).Assembly, typeof (Game).Assembly);
                EventSystem.Instance.Add(types);

                MongoHelper.Init();
                ProtobufHelper.Init();

                Log.Info($"server start........................ {Root.Instance.Scene.Id}");
                Options.Instance.AppType = AppType.RemoteBuilder;
                switch (Options.Instance.AppType)
                {
                    case AppType.ExcelExporter:
                    {
                        Options.Instance.Console = 1;
                        //Options: Customs
                        //Json-luban导出json
                        //GB2312:使用GB2312编码解决中文乱码
                        ExcelExporter.Export();
                        break;
                    }
                    case AppType.Proto2CS:
                    {
                        Options.Instance.Console = 1;
                        Proto2CS.Export();
                        break;
                    }
                    case AppType.RemoteBuilder:
                    {
                        Options.Instance.Console = 1;
                        await RemoteBuilder.RunAsync();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.ConsoleError(e.ToString());
            }
        }
    }
}
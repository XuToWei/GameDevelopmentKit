using System;
using System.Collections.Generic;
using CommandLine;

namespace ET.Server
{
    internal static class Init
    {
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };

            try
            {
                // 异步方法全部会回掉到主线程
                Game.AddSingleton<MainThreadSynchronizationContext>();

                // 命令行参数
                Parser.Default.ParseArguments<Options>(args)
                        .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                        .WithParsed(Game.AddSingleton);

                Game.AddSingleton<TimeInfo>().ITimeNow = new TimeNow();
                Game.AddSingleton<Logger>().ILog =
                        new NLogger(Options.Instance.AppType.ToString(), Options.Instance.Process, "../Config/NLog/NLog.config");
                Game.AddSingleton<ObjectPool>();
                Game.AddSingleton<IdGenerater>();
                Game.AddSingleton<EventSystem>();
                Game.AddSingleton<Root>();

                Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly);
                EventSystem.Instance.Add(types);

                MongoHelper.Init();
                ProtobufHelper.Init();

                Log.Info($"server start........................ {Root.Instance.Scene.Id}");
                
                switch (Options.Instance.AppType)
                {
                    case AppType.ExcelExporter:
                    {
                        Options.Instance.Console = 1;
                        //Options: Customs
                        //Json-luban导出json
                        //GB2312:使用GB2312编码解决中文乱码
                        ExcelExporter.Export();
                        return 0;
                    }
                    case AppType.Proto2CS:
                    {
                        Options.Instance.Console = 1;
                        Proto2CS.Export(Proto2CSCodeType.ET);
                        return 0;
                    }
                    case AppType.RemoteBuilderClient:
                    {
                        Options.Instance.Console = 1;
                        Root.Instance.Scene.AddComponent<ClientSceneManagerComponent>();
                        Scene scene = EntitySceneFactory.CreateScene(1, 1, 1, SceneType.RemoteBuilderClient, "RemoteBuilderClient", ClientSceneManagerComponent.Instance);
                        scene.AddComponent<RemoteBuilderClient>();
                        return 0;
                    }
                    case AppType.RemoteBuilderServer:
                    {
                        Options.Instance.Console = 1;
                        Root.Instance.Scene.AddComponent<ServerSceneManagerComponent>();
                        Scene scene = EntitySceneFactory.CreateScene(1, 1, 1, SceneType.RemoteBuilderClient, "RemoteBuilderClient", ServerSceneManagerComponent.Instance);
                        scene.AddComponent<RemoteBuilderServer>();
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                Log.ConsoleError(e.ToString());
            }

            return 1;
        }
    }
}
using System.Net;
using cfg;
using cfg.ET;

namespace ET.Server
{
    [Event(SceneType.Process)]
    public class EntryEvent2_InitServer: AEvent<ET.EventType.EntryEvent2>
    {
        protected override async ETTask Run(Scene scene, ET.EventType.EntryEvent2 args)
        {
            // 发送普通actor消息
            Root.Instance.Scene.AddComponent<ActorMessageSenderComponent>();
            // 发送location actor消息
            Root.Instance.Scene.AddComponent<ActorLocationSenderComponent>();
            // 访问location server的组件
            Root.Instance.Scene.AddComponent<LocationProxyComponent>();
            Root.Instance.Scene.AddComponent<ActorMessageDispatcherComponent>();
            Root.Instance.Scene.AddComponent<ServerSceneManagerComponent>();
            Root.Instance.Scene.AddComponent<RobotCaseComponent>();

            Root.Instance.Scene.AddComponent<NavmeshComponent>();

            DRStartProcessConfig processConfig = DataTables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, Options.Instance.Process);
            switch (Options.Instance.AppType)
            {
                case AppType.Server:
                {
                    Root.Instance.Scene.AddComponent<NetInnerComponent, IPEndPoint>(processConfig.InnerIPPort);

                    var processScenes = DataTables.Instance.DTStartSceneConfig.GetByProcess(Options.Instance.Process);
                    foreach (DRStartSceneConfig startConfig in processScenes)
                    {
                        await SceneFactory.CreateServerScene(ServerSceneManagerComponent.Instance, startConfig.Id, startConfig.InstanceId, startConfig.Zone, startConfig.Name,
                            startConfig.Type, startConfig);
                    }

                    break;
                }
                case AppType.Watcher:
                {
                    DRStartMachineConfig startMachineConfig = WatcherHelper.GetThisMachineConfig();
                    WatcherComponent watcherComponent = Root.Instance.Scene.AddComponent<WatcherComponent>();
                    watcherComponent.Start(Options.Instance.CreateScenes);
                    Root.Instance.Scene.AddComponent<NetInnerComponent, IPEndPoint>(NetworkHelper.ToIPEndPoint($"{startMachineConfig.InnerIP}:{startMachineConfig.WatcherPort}"));
                    break;
                }
                case AppType.GameTool:
                    break;
            }

            if (Options.Instance.Console == 1)
            {
                Root.Instance.Scene.AddComponent<ConsoleComponent>();
            }
        }
    }
}
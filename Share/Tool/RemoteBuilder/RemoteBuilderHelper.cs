using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using ET.Client;

namespace ET
{
    public static class RemoteBuilder
    {
        public static async UniTask RunAsync()
        {
            await InitShareAsync();
            if (Options.Instance.Customs.Equals("Client", StringComparison.OrdinalIgnoreCase))
            {
                await InitClientAsync();
            }
            else if (Options.Instance.Customs.Equals("Server", StringComparison.OrdinalIgnoreCase))
            {
                await InitServerAsync();
            }
            else //默认走ClientServer
            {
                await InitServerAsync();
                await InitClientAsync();
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

        private static async UniTask InitShareAsync()
        {
            Game.AddSingleton<NetServices>();
            Game.AddSingleton<ConfigComponent>().IConfigReader = new ConfigReader();
            await ConfigComponent.Instance.LoadAllAsync();
            
            Root.Instance.Scene.AddComponent<NetThreadComponent>();
            Root.Instance.Scene.AddComponent<OpcodeTypeComponent>();
            Root.Instance.Scene.AddComponent<MessageDispatcherComponent>();
            Root.Instance.Scene.AddComponent<ClientSceneManagerComponent>();
        }

        private static async UniTask InitClientAsync()
        {
            await UniTask.CompletedTask;
            Scene clientScene = EntitySceneFactory.CreateScene
                    (1, SceneType.RemoteBuilderClient, "RemoteBuilderClient", ClientSceneManagerComponent.Instance);
            clientScene.AddComponent<NetClientComponent, AddressFamily>(NetworkHelper
                    .GetHostAddress(Tables.Instance.RemoteBuilderConfig.ServerInnerIP).AddressFamily);
            
            Session routerSession = clientScene.GetComponent<NetClientComponent>().Create(NetworkHelper.ToIPEndPoint
                    ($"{Tables.Instance.RemoteBuilderConfig.ServerInnerIP}:{Tables.Instance.RemoteBuilderConfig.ServerPort}"));
            
            // Session gateSession = await RouterHelper
            //         .CreateRouterSession(clientScene, );
            // clientScene.AddComponent<SessionComponent>().Session = gateSession;
            clientScene.AddComponent<ConsoleComponent>();
        }

        private static async UniTask InitServerAsync()
        {
            await UniTask.CompletedTask;
            Root.Instance.Scene.AddComponent<ServerSceneManagerComponent>();
            Scene scene = EntitySceneFactory.CreateScene(1, 1, 1, SceneType.RemoteBuilderServer, "RemoteBuilderServer",
                ServerSceneManagerComponent.Instance);
        }
    }
}
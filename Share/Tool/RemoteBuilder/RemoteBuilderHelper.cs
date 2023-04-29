using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ET.Client;
using ET.Server;

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

            OpcodeHelper.IOpcodeIgnoreDebugLog = new OpcodeIgnoreDebugLog();
            Root.Instance.Scene.AddComponent<NetThreadComponent>();
            Root.Instance.Scene.AddComponent<OpcodeTypeComponent>();
            Root.Instance.Scene.AddComponent<MessageDispatcherComponent>();
            Root.Instance.Scene.AddComponent<ClientSceneManagerComponent>();
        }

        private static async UniTask InitClientAsync()
        {
            Scene clientScene = EntitySceneFactory.CreateScene
                    (1, SceneType.RemoteBuilderClient, "RemoteBuilderClient", ClientSceneManagerComponent.Instance);
            RemoteBuilderClient remoteBuilderClient = clientScene.AddComponent<RemoteBuilderClient>();
            await remoteBuilderClient.InitAsync();
        }
        
        private static async UniTask InitServerAsync()
        {
            Root.Instance.Scene.AddComponent<ServerSceneManagerComponent>();
            Scene scene = EntitySceneFactory.CreateScene(2, SceneType.RemoteBuilderServer, "RemoteBuilderServer",
                ServerSceneManagerComponent.Instance);
            RemoteBuilderServer remoteBuilderServer = scene.AddComponent<RemoteBuilderServer>();
            await remoteBuilderServer.InitAsync();
        }
    }
}
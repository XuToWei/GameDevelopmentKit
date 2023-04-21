using System;

namespace ET
{
    public static class RemoteBuilderHelper
    {
        public static void Run()
        {
            Options.Instance.Console = 1;
            if (Options.Instance.Customs.Equals("Client", StringComparison.OrdinalIgnoreCase))
            {
                Root.Instance.Scene.AddComponent<RemoteBuilderClient>();
            }
            else if (Options.Instance.Customs.Equals("Server", StringComparison.OrdinalIgnoreCase))
            {
                Root.Instance.Scene.AddComponent<RemoteBuilderServer>();
            }
            else//默认走ClientServer
            {
                Root.Instance.Scene.AddComponent<ClientSceneManagerComponent>();
                Scene scene = EntitySceneFactory.CreateScene(1, 1, 1, SceneType.RemoteBuilderClient, "RemoteBuilderClientServer", ClientSceneManagerComponent.Instance);
                scene.AddComponent<RemoteBuilderClient>();
                scene.AddComponent<RemoteBuilderServer>();
            }
        }
    }
}

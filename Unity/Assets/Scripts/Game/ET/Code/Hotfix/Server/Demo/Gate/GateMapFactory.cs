using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public static class GateMapFactory
    {
        public static async UniTask<Scene> Create(Entity parent, long id, long instanceId, string name)
        {
            await UniTask.CompletedTask;
            Scene scene = EntitySceneFactory.CreateScene(parent, id, instanceId, SceneType.Map, name);

            scene.AddComponent<UnitComponent>();
            scene.AddComponent<AOIManagerComponent>();
            scene.AddComponent<RoomManagerComponent>();
            
            scene.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            
            return scene;
        }
        
    }
}
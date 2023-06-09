using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class EntryEvent3_InitClient: AEvent<Scene, ET.EventType.EntryEvent3>
    {
        protected override async UniTask Run(Scene scene, ET.EventType.EntryEvent3 args)
        {
            Root.Instance.Scene.AddComponent<UGFEventComponent>();
            
            SceneType sceneType = EnumHelper.FromString<SceneType>(GlobalComponent.Instance.AppType.ToString());
            Scene clientScene = await SceneFactory.CreateClientScene(1, sceneType, sceneType.ToString());
            await EventSystem.Instance.PublishAsync(clientScene, new EventType.AppStartInitFinish());
        }
    }
}
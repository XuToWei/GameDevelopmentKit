using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class EntryEvent3_InitClient: AEvent<ET.EventType.EntryEvent3>
    {
        protected override async UniTask Run(Scene scene, ET.EventType.EntryEvent3 args)
        {
            // 加载配置
            Scene clientScene = await SceneFactory.CreateClientScene(1, "Game");
            clientScene.AddComponent<UGFEventComponent>();
            clientScene.AddComponent<UIComponent>();

            await EventSystem.Instance.PublishAsync(clientScene, new EventType.AppStartInitFinish());
        }
    }
}
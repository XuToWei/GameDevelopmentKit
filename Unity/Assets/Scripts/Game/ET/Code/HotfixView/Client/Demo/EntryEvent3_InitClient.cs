using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Main)]
    public class EntryEvent3_InitClient: AEvent<Scene, EntryEvent3>
    {
        protected override async UniTask Run(Scene root, EntryEvent3 args)
        {
            //Test
            root.AddComponent<TestComponent>();
            
            World.Instance.AddSingleton<UGFEventComponent>();
            
            GlobalComponent globalComponent = root.AddComponent<GlobalComponent>();
            root.AddComponent<UGFUIComponent>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<CurrentScenesComponent>();
            
            // 根据配置修改掉Main Fiber的SceneType
            SceneType sceneType = EnumHelper.FromString<SceneType>(globalComponent.AppType.ToString());
            root.SceneType = sceneType;
            
            await EventSystem.Instance.PublishAsync(root, new AppStartInitFinish());
        }
    }
}
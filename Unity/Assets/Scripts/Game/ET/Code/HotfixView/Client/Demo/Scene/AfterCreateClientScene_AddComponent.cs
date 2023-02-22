using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AfterCreateClientScene_AddComponent: AEvent<EventType.AfterCreateClientScene>
    {
        protected override async UniTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            
            await UniTask.CompletedTask;
        }
    }
}
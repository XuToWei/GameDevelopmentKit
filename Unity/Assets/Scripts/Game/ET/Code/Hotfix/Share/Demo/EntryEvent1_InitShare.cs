using Cysharp.Threading.Tasks;

namespace ET
{
    [Event(SceneType.Main)]
    public class EntryEvent1_InitShare: AEvent<Scene, EntryEvent1>
    {
        protected override async UniTask Run(Scene root, EntryEvent1 args)
        {
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ObjectWait>();
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<ProcessInnerSender>();
            
            await UniTask.CompletedTask;
        }
    }
}
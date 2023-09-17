using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.NetInner)]
    public class A2NetInner_MessageHandler: MessageHandler<Scene, A2NetInner_Message>
    {
        protected override async UniTask Run(Scene root, A2NetInner_Message innerMessage)
        {
            root.GetComponent<ProcessOuterSender>().Send(innerMessage.ActorId, innerMessage.MessageObject);
            await UniTask.CompletedTask;
        }
    }
}
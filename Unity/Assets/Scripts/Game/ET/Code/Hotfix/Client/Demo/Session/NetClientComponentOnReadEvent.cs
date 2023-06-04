using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Process)]
    public class NetClientComponentOnReadEvent: AEvent<Scene, NetClientComponentOnRead>
    {
        protected override async UniTask Run(Scene scene, NetClientComponentOnRead args)
        {
            Session session = args.Session;
            object message = args.Message;
            if (message is IResponse response)
            {
                session.OnResponse(response);
                return;
            }
            
            // 普通消息或者是Rpc请求消息
            MessageDispatcherComponent.Instance.Handle(session, message);
            await UniTask.CompletedTask;
        }
    }
}
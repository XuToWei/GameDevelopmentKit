using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [Event(SceneType.Process)]
    public class NetServerComponentOnReadEvent : AEvent<NetServerComponentOnRead>
    {
        protected override async UniTask Run(Scene scene, NetServerComponentOnRead args)
        {
            await UniTask.CompletedTask;
            Session session = args.Session;
            object message = args.Message;

            if (message is IResponse response)
            {
                session.OnResponse(response);
                return;
            }

            MessageDispatcherComponent.Instance.Handle(session, message);
        }
    }
}
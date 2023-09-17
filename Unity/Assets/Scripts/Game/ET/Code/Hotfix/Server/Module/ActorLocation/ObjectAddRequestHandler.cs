using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Location)]
    public class ObjectAddRequestHandler: MessageHandler<Scene, ObjectAddRequest, ObjectAddResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectAddRequest request, ObjectAddResponse response)
        {
            await scene.GetComponent<LocationManagerComoponent>().Get(request.Type).Add(request.Key, request.ActorId);
        }
    }
}
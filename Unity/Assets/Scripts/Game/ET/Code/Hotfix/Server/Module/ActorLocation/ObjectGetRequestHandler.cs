using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Location)]
    public class ObjectGetRequestHandler: MessageHandler<Scene, ObjectGetRequest, ObjectGetResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectGetRequest request, ObjectGetResponse response)
        {
            response.ActorId = await scene.GetComponent<LocationManagerComoponent>().Get(request.Type).Get(request.Key);
        }
    }
}
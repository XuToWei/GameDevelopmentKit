using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectRemoveRequestHandler: AMActorRpcHandler<Scene, ObjectRemoveRequest, ObjectRemoveResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectRemoveRequest request, ObjectRemoveResponse response)
        {
            await scene.GetComponent<LocationComponent>().Remove(request.Key);
        }
    }
}
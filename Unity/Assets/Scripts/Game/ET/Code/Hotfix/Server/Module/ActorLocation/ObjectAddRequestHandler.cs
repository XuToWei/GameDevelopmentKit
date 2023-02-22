using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectAddRequestHandler : AMActorRpcHandler<Scene, ObjectAddRequest, ObjectAddResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectAddRequest request, ObjectAddResponse response)
        {
            await scene.GetComponent<LocationComponent>().Add(request.Key, request.InstanceId);
        }
    }
}
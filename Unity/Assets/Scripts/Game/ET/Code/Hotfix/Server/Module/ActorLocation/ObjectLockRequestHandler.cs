using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectLockRequestHandler : AMActorRpcHandler<Scene, ObjectLockRequest, ObjectLockResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectLockRequest request, ObjectLockResponse response)
        {
            await scene.GetComponent<LocationComponent>().Lock(request.Key, request.InstanceId, request.Time);
        }
    }
}
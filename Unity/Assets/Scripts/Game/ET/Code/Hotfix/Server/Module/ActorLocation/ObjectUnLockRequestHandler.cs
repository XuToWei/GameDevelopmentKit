using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectUnLockRequestHandler : AMActorRpcHandler<Scene, ObjectUnLockRequest, ObjectUnLockResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectUnLockRequest request, ObjectUnLockResponse response)
        {
            scene.GetComponent<LocationComponent>().UnLock(request.Key, request.OldInstanceId, request.InstanceId);

            await UniTask.CompletedTask;
        }
    }
}
using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectUnLockRequestHandler: ActorMessageHandler<Scene, ObjectUnLockRequest, ObjectUnLockResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectUnLockRequest request, ObjectUnLockResponse response)
        {
            scene.GetComponent<LocationManagerComponent>().Get(request.Type).UnLock(request.Key, request.OldInstanceId, request.InstanceId);

            await UniTask.CompletedTask;
        }
    }
}
using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectLockRequestHandler: ActorMessageHandler<Scene, ObjectLockRequest, ObjectLockResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectLockRequest request, ObjectLockResponse response)
        {
            await scene.GetComponent<LocationManagerComponent>().Get(request.Type).Lock(request.Key, request.InstanceId, request.Time);
        }
    }
}
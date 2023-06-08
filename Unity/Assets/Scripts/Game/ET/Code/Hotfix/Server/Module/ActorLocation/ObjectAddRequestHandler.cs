using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectAddRequestHandler: ActorMessageHandler<Scene, ObjectAddRequest, ObjectAddResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectAddRequest request, ObjectAddResponse response)
        {
            await scene.GetComponent<LocationManagerComponent>().Get(request.Type).Add(request.Key, request.InstanceId);
        }
    }
}
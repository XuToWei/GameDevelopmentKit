using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectGetRequestHandler: ActorMessageHandler<Scene, ObjectGetRequest, ObjectGetResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectGetRequest request, ObjectGetResponse response)
        {
            long instanceId = await scene.GetComponent<LocationManagerComponent>().Get(request.Type).Get(request.Key);
            response.InstanceId = instanceId;
        }
    }
}
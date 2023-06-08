using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [ActorMessageHandler(SceneType.Location)]
    public class ObjectRemoveRequestHandler: ActorMessageHandler<Scene, ObjectRemoveRequest, ObjectRemoveResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectRemoveRequest request, ObjectRemoveResponse response)
        {
            await scene.GetComponent<LocationManagerComponent>().Get(request.Type).Remove(request.Key);
        }
    }
}
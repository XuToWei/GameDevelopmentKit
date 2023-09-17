using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Location)]
    public class ObjectRemoveRequestHandler: MessageHandler<Scene, ObjectRemoveRequest, ObjectRemoveResponse>
    {
        protected override async UniTask Run(Scene scene, ObjectRemoveRequest request, ObjectRemoveResponse response)
        {
            await scene.GetComponent<LocationManagerComoponent>().Get(request.Type).Remove(request.Key);
        }
    }
}
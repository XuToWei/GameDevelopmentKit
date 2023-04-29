using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.RemoteBuilderClient)]
    public class OnClientInitFinish_RequestCanBuild : AEvent<EventType.OnClientInitFinish>
    {
        protected override async UniTask Run(Scene scene, EventType.OnClientInitFinish arg)
        {
            RemoteBuilderClient remoteBuilderClient = scene.GetComponent<RemoteBuilderClient>();
            S2C_CanBuild response = (S2C_CanBuild)await remoteBuilderClient.Session.Call(new C2S_CanBuild());
            if (ErrorCore.IsRpcNeedThrowException(response.Error))
            {
                throw new RpcException(response.Error, $"Message: {response.Message}");
            }

            if (response.CanBuild)
            {
                Log.Console(response.Message);
            }
            else
            {
                Log.Console(response.Message);
            }
        }
    }
}
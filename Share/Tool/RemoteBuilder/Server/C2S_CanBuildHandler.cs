using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class C2S_CanBuildHandler: AMRpcHandler<C2S_CanBuild, S2C_CanBuild>
    {
        protected override async UniTask Run(Session session, C2S_CanBuild request, S2C_CanBuild response)
        {
            await UniTask.CompletedTask;
            RemoteBuilderServer remoteBuilderServer = session.ClientScene().GetComponent<RemoteBuilderServer>();
            if (remoteBuilderServer.IsBuilding)
            {
                response.CanBuild = false;
                response.Message = $"version:{remoteBuilderServer.BuildingVersion} 正在打包中... ";
            }
            else
            {
                response.CanBuild = true;
                response.Message = $"当前 version:{remoteBuilderServer.BuildingVersion}, 确定打包吗（Y/N）？";
            }
        }
    }
}

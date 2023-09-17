using Cysharp.Threading.Tasks;

namespace ET.Server
{
	[MessageLocationHandler(SceneType.Map)]
	public class C2M_TransferMapHandler : MessageLocationHandler<Unit, C2M_TransferMap, M2C_TransferMap>
	{
		protected override async UniTask Run(Unit unit, C2M_TransferMap request, M2C_TransferMap response)
		{
			await UniTask.CompletedTask;

			string currentMap = unit.Scene().Name;
			string toMap = null;
			if (currentMap == "Map1")
			{
				toMap = "Map2";
			}
			else
			{
				toMap = "Map1";
			}

			var startSceneConfig = Tables.Instance.DTStartSceneConfig.GetBySceneName(unit.Fiber().Zone, toMap);
			
			TransferHelper.TransferAtFrameFinish(unit, startSceneConfig.ActorId, toMap).Forget();
		}
	}
}
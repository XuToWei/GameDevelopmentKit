using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
	[ActorMessageLocationHandler(SceneType.Map)]
	public class C2M_TransferMapHandler : ActorMessageLocationHandler<Unit, C2M_TransferMap, M2C_TransferMap>
	{
		protected override async UniTask Run(Unit unit, C2M_TransferMap request, M2C_TransferMap response)
		{
			await UniTask.CompletedTask;

			string currentMap = unit.DomainScene().Name;
			string toMap = null;
			if (currentMap == "Map1")
			{
				toMap = "Map2";
			}
			else
			{
				toMap = "Map1";
			}

			DRStartSceneConfig startSceneConfig = Tables.Instance.DTStartSceneConfig.GetBySceneName(unit.DomainScene().Zone, toMap);
			
			TransferHelper.TransferAtFrameFinish(unit, startSceneConfig.InstanceId, toMap).Forget();
		}
	}
}
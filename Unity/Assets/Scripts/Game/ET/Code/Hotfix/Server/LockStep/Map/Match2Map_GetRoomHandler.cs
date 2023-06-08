using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
	[ActorMessageHandler(SceneType.Map)]
	public class Match2Map_GetRoomHandler : ActorMessageHandler<Scene, Match2Map_GetRoom, Map2Match_GetRoom>
	{
		protected override async UniTask Run(Scene scene, Match2Map_GetRoom request, Map2Match_GetRoom response)
		{
			RoomManagerComponent roomManagerComponent = scene.GetComponent<RoomManagerComponent>();
			Room room = await roomManagerComponent.CreateServerRoom(request);
			response.InstanceId = room.InstanceId;
			await UniTask.CompletedTask;
		}
	}
}
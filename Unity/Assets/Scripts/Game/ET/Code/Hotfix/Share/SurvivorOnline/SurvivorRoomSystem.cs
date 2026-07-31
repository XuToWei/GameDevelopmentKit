namespace ET
{
    [EntitySystemOf(typeof(SurvivorRoom))]
    public static partial class SurvivorRoomSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorRoom self, SceneType sceneType, string roomCode)
        {
            self.Fiber = self.Parent.Fiber();
            self.SceneType = sceneType;
            self.Name = sceneType.ToString();
            self.RoomCode = roomCode;
        }
    }
}

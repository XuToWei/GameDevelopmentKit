namespace ET
{
    [ComponentOf]
    public sealed class SurvivorRoom: Entity, IScene, IAwake<SceneType, string>
    {
        public Fiber Fiber { get; set; }

        public SceneType SceneType { get; set; }

        public string Name { get; set; }

        public string RoomCode { get; set; }
    }
}

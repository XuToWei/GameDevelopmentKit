namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorRoomDirectoryComponent))]
    public static partial class SurvivorRoomDirectoryComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorRoomDirectoryComponent self)
        {
            self.Runtime = new SurvivorRoomDirectoryRuntime();
        }
    }
}

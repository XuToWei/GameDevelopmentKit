namespace ET.Server
{
    [EntitySystemOf(typeof(SurvivorPlayerRoomComponent))]
    public static partial class SurvivorPlayerRoomComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorPlayerRoomComponent self)
        {
        }
    }
}

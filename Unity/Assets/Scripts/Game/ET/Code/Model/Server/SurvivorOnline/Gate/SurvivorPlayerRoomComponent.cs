namespace ET.Server
{
    [ComponentOf(typeof(Player))]
    public sealed class SurvivorPlayerRoomComponent: Entity, IAwake
    {
        public ActorId RoomActorId { get; set; }

        public string RoomCode { get; set; }
    }
}

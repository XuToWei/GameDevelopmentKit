namespace ET.Server
{
    [ChildOf(typeof(MatchMgrComponent))]
    public class MatchRoomComponent: Entity, IAwake, IDestroy
    {
        public long PlayerId1 { get; set; }
        public long PlayerId2 { get; set; }
        public MatchRoomPlayerData PlayerData1 { get; set; }
        public MatchRoomPlayerData PlayerData2 { get; set; }
        public MatchRoomState State { get; set; }
        public MatchRoomType Type { get; set; }
        public string Code { get; set; }
    }
}

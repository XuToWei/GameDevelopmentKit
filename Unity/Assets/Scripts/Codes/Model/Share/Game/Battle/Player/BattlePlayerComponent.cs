namespace ET
{
    [ComponentOf(typeof(BattleComponent))]
    public class BattlePlayerComponent: Entity, IAwake<long, long>
    {
        public long PlayerId1 { get; set; }
        public long PlayerId2 { get; set; }
        public BattlePlayerData PlayerData1 { get; set; }
        public BattlePlayerData PlayerData2 { get; set; }
    }
}

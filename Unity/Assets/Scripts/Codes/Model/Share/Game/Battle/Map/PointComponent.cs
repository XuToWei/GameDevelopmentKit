namespace ET
{
    [ChildOf(typeof(BattleMapComponent))]
    public class PointComponent: Entity, IAwake<int, int>
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}

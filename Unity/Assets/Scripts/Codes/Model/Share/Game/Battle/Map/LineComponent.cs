namespace ET
{
    [ChildOf(typeof (BattleMapComponent))]
    public class LineComponent: Entity, IAwake<int, (int, int, int, int)>
    {
        public int LineNo { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public bool IsConnected { get; set; }
        public LineType LineType { get; set; }
    }
}

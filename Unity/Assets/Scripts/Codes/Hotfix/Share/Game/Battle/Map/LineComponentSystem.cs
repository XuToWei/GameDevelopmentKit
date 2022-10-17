namespace ET
{
    [FriendOf(typeof(LineComponent))]
    [ChildOf(typeof(BattleMapComponent))]
    public static class LineComponentSystem
    {
        public class AwakeSystem : AwakeSystem<LineComponent, int, (int, int, int, int)>
        {
            protected override void Awake(LineComponent self, int lineNo, (int, int, int, int) points)
            {
                self.LineNo = lineNo;
                self.X1 = points.Item1;
                self.Y1 = points.Item2;
                self.X2 = points.Item3;
                self.Y2 = points.Item4;
                if (self.X1 == self.X2)
                {
                    self.LineType = LineType.Vertical;
                }
                else if (self.Y1 == self.Y2)
                {
                    self.LineType = LineType.Horizontal;
                }
                else
                {
                    self.LineType = LineType.Undefine;
                }
            }
        }
    }
}

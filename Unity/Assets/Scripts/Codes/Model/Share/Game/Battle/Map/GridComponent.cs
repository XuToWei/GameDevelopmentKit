using System.Collections.Generic;

namespace ET
{
    [ChildOf(typeof (BattleMapComponent))]
    public class GridComponent: Entity, IAwake<int, int>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Dictionary<DirectionType, int> AroundLineNoDict { get; } = new();
    }
}
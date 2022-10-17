using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof (BattleComponent))]
    public class BattleMapComponent: Entity, IAwake<int, int>
    {
        public int XNum { get; set; }
        public int YNum { get; set; }
        public Dictionary<int, LineComponent> LineComponentDict { get;} = new ();
        public Dictionary<(int, int), PointComponent> PointComponentDict { get;} = new ();
        public Dictionary<(int, int), GridComponent> GridComponentDict { get;} = new ();
    }
}

using System.Collections.Generic;

namespace ET
{
    public class BattlePlayerData
    {
        public long PlayerId { get; }
        public HashSet<(int, int)> ConnectedGridKeys { get; set; } = new ();

        public BattlePlayerData(long playerId)
        {
            this.PlayerId = playerId;
        }
    }
}

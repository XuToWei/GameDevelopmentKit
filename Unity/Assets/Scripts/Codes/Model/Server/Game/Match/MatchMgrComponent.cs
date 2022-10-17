using System.Collections.Generic;

namespace ET.Server
{
    public class MatchMgrComponent: Entity, IAwake
    {
        public Dictionary<string, long> MatchRoomCodeDict { get; } = new();
        public Dictionary<long, long> MatchRoomPlayerIdDict { get; } = new();
        public Dictionary<long, MatchingPlayerData> MatchingPlayerDataDict { get; } = new();
        public Queue<long> MatchingPlayerIds = new ();
        public long UpdateTimerId { get; set; }
        public Queue<int> MatchRoomCodePool { get; } = new();
    }
}
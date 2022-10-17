namespace ET.Server
{
    public class MatchingPlayerData
    {
        public long PlayerId { get; private set; }
        public long MatchingStartTime { get; private set; }


        public static MatchingPlayerData Create(long playerId, long startTime)
        {
            MatchingPlayerData data = ObjectPool.Instance.Fetch<MatchingPlayerData>();
            data.PlayerId = playerId;
            data.MatchingStartTime = startTime;
            return data;
        }
    }
}

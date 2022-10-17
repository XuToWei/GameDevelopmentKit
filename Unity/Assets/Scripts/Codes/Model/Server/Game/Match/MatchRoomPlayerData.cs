namespace ET.Server
{
    public class MatchRoomPlayerData
    {
        public long PlayerId { get; set; }
        public bool IsReady { get; set; }
        public bool IsRobot { get; set; }

        public void InitPlayer(long playerId)
        {
            this.PlayerId = playerId;
            this.IsReady = false;
            this.IsRobot = false;
        }

        public void InitRobot()
        {
            this.PlayerId = 0;
            this.IsReady = false;
            this.IsRobot = true;
        }

        public void Clear()
        {
            this.PlayerId = 0;
            this.IsReady = false;
            this.IsRobot = false;
        }
    }
}
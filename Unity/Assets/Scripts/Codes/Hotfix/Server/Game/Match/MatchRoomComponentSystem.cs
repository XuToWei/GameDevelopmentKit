namespace ET.Server
{
    [FriendOf(typeof (MatchRoomComponent))]
    public static class MatchRoomComponentSystem
    {
        public class AwakeSystem: AwakeSystem<MatchRoomComponent>
        {
            protected override void Awake(MatchRoomComponent self)
            {
                self.State = MatchRoomState.Prepare;
                self.PlayerId1 = 0;
                self.PlayerId2 = 0;
                self.PlayerData1 = ObjectPool.Instance.Fetch<MatchRoomPlayerData>();
                self.PlayerData2 = ObjectPool.Instance.Fetch<MatchRoomPlayerData>();
            }
        }
        
        public class DestroySystem : DestroySystem<MatchRoomComponent>
        {
            protected override void Destroy(MatchRoomComponent self)
            {
                self.PlayerId1 = 0;
                self.PlayerId2 = 0;
                self.PlayerData1.Clear();
                ObjectPool.Instance.Recycle(self.PlayerData1);
                self.PlayerData1 = null;
                self.PlayerData2.Clear();
                ObjectPool.Instance.Recycle(self.PlayerData2);
                self.PlayerData2 = null;
            }
        }

        public static bool IsAllReady(this MatchRoomComponent self)
        {
            bool IsReady(MatchRoomPlayerData playerData)
            {
                return playerData != null && playerData.PlayerId != 0 && playerData.IsReady;
            }

            return IsReady(self.PlayerData1) && IsReady(self.PlayerData2);
        }

        public static void SetPlayerReady(this MatchRoomComponent self, long readyPlayerId)
        {
            if (self.PlayerId1 == readyPlayerId)
            {
                self.PlayerData1.IsReady = true;
            }
            else if (self.PlayerId2 == readyPlayerId)
            {
                self.PlayerData2.IsReady = true;
            }
        }

        public static void AddPlayer(this MatchRoomComponent self, long playerId)
        {
            if (self.PlayerId1 == 0)
            {
                self.PlayerId1 = playerId;
                self.PlayerData1.InitPlayer(playerId);
            }
            else if (self.PlayerId2 == 0)
            {
                self.PlayerId2 = playerId;
                self.PlayerData2.InitPlayer(playerId);
            }
        }
        
        public static void AddRobot(this MatchRoomComponent self)
        {
            if (self.PlayerId1 == 0)
            {
                self.PlayerData1.InitRobot();
            }
            else if (self.PlayerId2 == 0)
            {
                self.PlayerData2.InitRobot();
            }
        }

        public static void KickOutPlayer(this MatchRoomComponent self, long playerId)
        {
            if (self.PlayerId1 == playerId)
            {
                self.PlayerId1 = 0;
                self.PlayerData1.Clear();
            }
            if (self.PlayerId2 == playerId)
            {
                self.PlayerId2 = 0;
                self.PlayerData2.Clear();
            }
        }
    }
}

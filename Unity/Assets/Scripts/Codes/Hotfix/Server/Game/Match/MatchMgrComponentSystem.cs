using System;
using cfg;

namespace ET.Server
{
    [FriendOf(typeof(MatchMgrComponent))]
    public static class MatchMgrComponentSystem
    {
        public class AwakeSystem: AwakeSystem<MatchMgrComponent>
        {
            protected override void Awake(MatchMgrComponent self)
            {
                self.UpdateTimerId = TimerComponent.Instance.NewRepeatedTimer(1000, TimerInvokeType.MatchUpdate, self);
            }
        }

        [FriendOf(typeof(MatchMgrComponent))]
        [Invoke(TimerInvokeType.MatchUpdate)]
        public class MatchUpdateTimer: ATimer<MatchMgrComponent>
        {
            protected override void Run(MatchMgrComponent self)
            {
                if (self.MatchingPlayerDataDict.Count < 1)
                    return;
                int maxCount = Math.Max(100, self.MatchingPlayerDataDict.Count);
                Scene scene = self.DomainScene();
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
                while (self.MatchingPlayerIds.Count >= 2 && maxCount > 0)
                {
                    maxCount--;
                    long playerId1 = self.MatchingPlayerIds.Dequeue();
                    RemoveMathedPlayer(self, playerId1);
                    long playerId2 = self.MatchingPlayerIds.Dequeue();
                    RemoveMathedPlayer(self, playerId2);
                    if (playerComponent.Get(playerId1) == default || playerComponent.Get(playerId2) == default)
                    {
                        continue;
                    }
                    MatchRoomComponent roomComponent = self.CreateMatchRoom(playerId1);
                    roomComponent.AddPlayer(playerId2);
                }

                //超时匹配机器人
                long curTime = TimeHelper.ServerNow();
                foreach (var pair in self.MatchingPlayerDataDict)
                {
                    if (curTime - pair.Value.MatchingStartTime >= DataTables.Instance.DTDefaultConfig.MaxMatchTime)
                    {
                        MatchRoomComponent roomComponent = self.CreateMatchRoom(pair.Key);
                        roomComponent.AddPlayer(pair.Key);
                    }
                }
            }

            private void RemoveMathedPlayer(MatchMgrComponent self, long playerId)
            {
                MatchingPlayerData playerData = self.MatchingPlayerDataDict[playerId];
                self.MatchingPlayerDataDict.Remove(playerId);
                ObjectPool.Instance.Recycle(playerData);
            }
        }

        public static void AddMatchingPlayer(this MatchMgrComponent self, long playerId)
        {
            if (self.MatchingPlayerDataDict.ContainsKey(playerId))
                return;
            if (self.MatchRoomPlayerIdDict.ContainsKey(playerId))
                return;
            self.MatchingPlayerIds.Enqueue(playerId);
            self.MatchingPlayerDataDict.Add(playerId, MatchingPlayerData.Create(playerId, TimeHelper.ServerNow()));
        }

        public static MatchRoomComponent GetMatchRoomComponent(this MatchMgrComponent self, long roomId)
        {
            return self.GetChild<MatchRoomComponent>(roomId);
        }
        
        public static MatchRoomComponent GetMatchRoomComponent(this MatchMgrComponent self, string roomCode)
        {
            if (self.MatchRoomCodeDict.TryGetValue(roomCode, out long roomId))
            {
                return self.GetMatchRoomComponent(roomId);
            }

            return default;
        }

        public static MatchRoomComponent GetMatchRoomComponentByPlayerId(this MatchMgrComponent self, long playerId)
        {
            if (self.MatchRoomPlayerIdDict.TryGetValue(playerId, out long roomId))
            {
                return self.GetMatchRoomComponent(roomId);
            }

            return default;
        }

        public static MatchRoomComponent CreateMatchRoom(this MatchMgrComponent self, long playerId)
        {
            if (self.MatchRoomPlayerIdDict.ContainsKey(playerId))
            {
                return default;
            }
            MatchRoomComponent roomComponent = self.AddChild<MatchRoomComponent>();
            roomComponent.AddPlayer(playerId);
            return roomComponent;
        }

        public static void AddPlayerFromMatchRoom(this MatchMgrComponent self, long roomId, long playerId)
        {
            MatchRoomComponent roomComponent = self.GetMatchRoomComponent(roomId);
            if (roomComponent != null)
            {
                roomComponent.AddPlayer(playerId);
                self.MatchRoomPlayerIdDict[playerId] = roomComponent.Id;
            }
        }
        
        public static void AddRobotFromMatchRoom(this MatchMgrComponent self, long roomId)
        {
            MatchRoomComponent roomComponent = self.GetMatchRoomComponent(roomId);
            if (roomComponent != null)
            {
                roomComponent.AddRobot();
            }
        }

        public static void KickOutPlayerFromMatchRoom(this MatchMgrComponent self, long playerId)
        {
            MatchRoomComponent roomComponent = self.GetMatchRoomComponentByPlayerId(playerId);
            if (roomComponent != null)
            {
                roomComponent.KickOutPlayer(playerId);
                if (self.MatchRoomPlayerIdDict.ContainsKey(playerId))
                {
                    self.MatchRoomPlayerIdDict.Remove(playerId);
                }
                if (roomComponent.PlayerId1 == 0 && roomComponent.PlayerId2 == 0)
                {
                    if (self.MatchRoomCodeDict.ContainsKey(roomComponent.Code))
                    {
                        self.MatchRoomCodeDict.Remove(roomComponent.Code);
                    }
                    roomComponent.Dispose();
                }
            }
        }
    }
}

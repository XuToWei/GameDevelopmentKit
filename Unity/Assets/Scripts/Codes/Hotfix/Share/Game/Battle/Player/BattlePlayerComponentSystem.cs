namespace ET
{
    [FriendOf(typeof(BattlePlayerComponent))]
    public static class BattlePlayerComponentSystem
    {
        public class AwakeSystem : AwakeSystem<BattlePlayerComponent, long, long>
        {
            protected override void Awake(BattlePlayerComponent self, long playerId1, long playerId2)
            {
                self.PlayerId1 = playerId1;
                self.PlayerId2 = playerId2;
                self.PlayerData1 = new BattlePlayerData(playerId1);
                self.PlayerData2 = new BattlePlayerData(playerId2);
            }
        }

        public static BattlePlayerData GetPlayerData(this BattlePlayerComponent component, long playerId)
        {
            if (playerId == component.PlayerId1)
            {
                return component.PlayerData1;
            }

            if (playerId == component.PlayerId2)
            {
                return component.PlayerData2;
            }

            return default;
        }

        public static void AddSurroundedGrid(this BattlePlayerComponent component, long playerId, (int, int) gridKey)
        {
            BattlePlayerData playerData = component.GetPlayerData(playerId);
            if(playerData == null)
                return;
            if (playerData.ConnectedGridKeys.Contains(gridKey))
                return;
            playerData.ConnectedGridKeys.Add(gridKey);
        }

        public static bool IsSurroundedGrid(this BattlePlayerComponent component, (int, int) gridKey)
        {
            if (component.PlayerData1.ConnectedGridKeys.Contains(gridKey))
                return true;
            if (component.PlayerData2.ConnectedGridKeys.Contains(gridKey))
                return true;
            return false;
        }
    }
}

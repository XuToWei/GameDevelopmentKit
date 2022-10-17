namespace ET
{
    [FriendOf(typeof(BattleComponent))]
    public static class BattleComponentSystem
    {
        public class AwakeSystem : AwakeSystem<BattleComponent, (long, long), (int, int)>
        {
            protected override void Awake(BattleComponent self, (long, long) playerIds, (int, int) mapXY)
            {
                self.AddComponent<BattleMapComponent, int, int>(mapXY.Item1, mapXY.Item2);
                self.AddComponent<BattlePlayerComponent, long, long>(playerIds.Item1, playerIds.Item2);
            }
        }

        public static void Connection(this BattleComponent component, int lineNo, long playerId)
        {
            BattleMapComponent battleMapComponent = component.GetComponent<BattleMapComponent>();
            LineComponent lineComponent = battleMapComponent.GetLineComponent(lineNo);
            if(lineComponent == default || lineComponent.IsConnected)
                return;
            lineComponent.IsConnected = true;
            (GridComponent, GridComponent) gridComponents = battleMapComponent.GetAroundGridComponents(lineComponent);
            if (gridComponents == default)
                return;
            BattlePlayerComponent battlePlayerComponent = component.GetComponent<BattlePlayerComponent>();

            bool CheckCanSurroundedGrid(GridComponent gridComponent)
            {
                if (gridComponent != null && !battlePlayerComponent.IsSurroundedGrid((gridComponent.X, gridComponent.Y)) && battleMapComponent.CheckCanSurroundedGrid(gridComponent))
                {
                    return true;
                }
                return false;
            }

            if (CheckCanSurroundedGrid(gridComponents.Item1))
            {
                battlePlayerComponent.AddSurroundedGrid(playerId, (gridComponents.Item1.X, gridComponents.Item1.Y));
            }
            if (CheckCanSurroundedGrid(gridComponents.Item2))
            {
                battlePlayerComponent.AddSurroundedGrid(playerId, (gridComponents.Item2.X, gridComponents.Item2.Y));
            }
            
            int player1Count = battlePlayerComponent.PlayerData1.ConnectedGridKeys.Count;
            int player2Count = battlePlayerComponent.PlayerData2.ConnectedGridKeys.Count;
            if (player1Count + player2Count >= battleMapComponent.XNum * battleMapComponent.YNum)//所有格子都被填满
            {
                if (player1Count > player2Count)
                {
                    //1号玩家胜利
                }
                else if (player1Count < player2Count)
                {
                    //2号玩家胜利
                }
                else
                {
                    //两人都失败
                }
            }
        }
    }
}
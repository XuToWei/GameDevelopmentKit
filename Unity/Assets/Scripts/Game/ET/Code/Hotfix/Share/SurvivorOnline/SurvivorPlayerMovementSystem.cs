namespace ET
{
    public static class SurvivorPlayerMovementSystem
    {
        public static void TickPlayerMovement(this SurvivorWorldComponent self)
        {
            using var playerEnumerator = self.Data.Players.GetEnumerator();
            while (playerEnumerator.MoveNext())
            {
                SurvivorPlayerState player = playerEnumerator.Current.Value;
                if (!player.Alive)
                {
                    continue;
                }

                player.PositionX += player.MoveX * player.MovePerTick() / SurvivorDefaults.InputScale;
                player.PositionY += player.MoveY * player.MovePerTick() / SurvivorDefaults.InputScale;
                player.PositionX = SurvivorDefaults.ClampPlayerPosition(player.PositionX);
                player.PositionY = SurvivorDefaults.ClampPlayerPosition(player.PositionY);
            }
        }
    }
}

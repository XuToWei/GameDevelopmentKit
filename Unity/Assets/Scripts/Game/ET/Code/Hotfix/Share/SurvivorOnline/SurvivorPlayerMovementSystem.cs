namespace ET
{
    public static class SurvivorPlayerMovementSystem
    {
        public static void TickPlayerMovement(this SurvivorWorldComponent self)
        {
            using var playerEnumerator = self.Data.Players.GetEnumerator();
            while (playerEnumerator.MoveNext())
            {
                self.Runtime.Player = playerEnumerator.Current.Value;
                if (!self.Runtime.Player.Alive)
                {
                    continue;
                }

                self.Runtime.Player.PositionX +=
                        self.Runtime.Player.MoveX * self.Runtime.Player.MovePerTick() /
                        SurvivorDefaults.InputScale;
                self.Runtime.Player.PositionY +=
                        self.Runtime.Player.MoveY * self.Runtime.Player.MovePerTick() /
                        SurvivorDefaults.InputScale;
                self.Runtime.Player.PositionX = SurvivorDefaults.ClampPlayerPosition(
                    self.Runtime.Player.PositionX);
                self.Runtime.Player.PositionY = SurvivorDefaults.ClampPlayerPosition(
                    self.Runtime.Player.PositionY);
            }

            self.Runtime.Player = null;
        }
    }
}

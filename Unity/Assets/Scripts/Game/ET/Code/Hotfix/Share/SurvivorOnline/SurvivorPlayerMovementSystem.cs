namespace ET
{
    public static class SurvivorPlayerMovementSystem
    {
        public static void TickPlayerMovement(this SurvivorWorldComponent self)
        {
            self.Runtime.PlayerEnumerator = self.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                self.Runtime.Player = self.Runtime.PlayerEnumerator.Current.Value;
                if (!self.Runtime.Player.Alive)
                {
                    continue;
                }

                self.Runtime.Player.PositionX +=
                        self.Runtime.Player.MoveX * SurvivorDefaults.PlayerMovePerTick / SurvivorDefaults.InputScale;
                self.Runtime.Player.PositionY +=
                        self.Runtime.Player.MoveY * SurvivorDefaults.PlayerMovePerTick / SurvivorDefaults.InputScale;
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;
            self.Runtime.Player = null;
        }
    }
}

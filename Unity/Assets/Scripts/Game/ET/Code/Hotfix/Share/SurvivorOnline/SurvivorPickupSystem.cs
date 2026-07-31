namespace ET
{
    public static class SurvivorPickupSystem
    {
        public static void TickPickups(this SurvivorWorldComponent self)
        {
            self.Runtime.PickupRemovalStateIds.Clear();
            self.Runtime.PickupEnumerator = self.Data.Pickups.GetEnumerator();
            while (self.Runtime.PickupEnumerator.MoveNext())
            {
                self.Runtime.Pickup = self.Runtime.PickupEnumerator.Current.Value;
                self.Runtime.Collected = false;
                self.Runtime.PlayerEnumerator = self.Data.Players.GetEnumerator();
                while (self.Runtime.PlayerEnumerator.MoveNext())
                {
                    self.Runtime.Player = self.Runtime.PlayerEnumerator.Current.Value;
                    if (!self.Runtime.Player.Alive)
                    {
                        continue;
                    }

                    if (SurvivorMath.Abs(self.Runtime.Player.PositionX - self.Runtime.Pickup.PositionX) >
                        SurvivorDefaults.PlayerCollisionRadius + SurvivorDefaults.PickupCollisionRadius)
                    {
                        continue;
                    }

                    if (SurvivorMath.Abs(self.Runtime.Player.PositionY - self.Runtime.Pickup.PositionY) >
                        SurvivorDefaults.PlayerCollisionRadius + SurvivorDefaults.PickupCollisionRadius)
                    {
                        continue;
                    }

                    self.Runtime.Player.Experience += self.Runtime.Pickup.Experience;
                    self.Runtime.Collected = true;
                    break;
                }

                self.Runtime.PlayerEnumerator.Dispose();
                self.Runtime.PlayerEnumerator = null;
                if (self.Runtime.Collected)
                {
                    self.Runtime.Player.LogicObserver.ObserveChanges();
                }

                if (self.Runtime.Collected)
                {
                    self.Runtime.PickupRemovalStateIds.Add(self.Runtime.Pickup.StateId);
                }
            }

            self.Runtime.PickupEnumerator.Dispose();
            self.Runtime.PickupEnumerator = null;
            self.Runtime.Index = 0;
            while (self.Runtime.Index < self.Runtime.PickupRemovalStateIds.Count)
            {
                self.Data.Pickups.Remove(self.Runtime.PickupRemovalStateIds[self.Runtime.Index]);
                self.Data.PickupSetRevision++;
                self.Runtime.Index++;
            }

            self.Runtime.Player = null;
            self.Runtime.Pickup = null;
        }
    }
}

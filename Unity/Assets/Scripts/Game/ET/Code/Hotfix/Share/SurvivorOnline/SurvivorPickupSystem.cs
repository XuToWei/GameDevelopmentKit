namespace ET
{
    public static class SurvivorPickupSystem
    {
        public static void TickPickups(this SurvivorWorldComponent self)
        {
            self.Runtime.PickupRemovalStateIds.Clear();
            using var pickupEnumerator = self.Data.Pickups.GetEnumerator();
            while (pickupEnumerator.MoveNext())
            {
                self.Runtime.Pickup = pickupEnumerator.Current.Value;
                self.Runtime.Collected = false;
                using var playerEnumerator = self.Data.Players.GetEnumerator();
                while (playerEnumerator.MoveNext())
                {
                    self.Runtime.Player = playerEnumerator.Current.Value;
                    if (!self.Runtime.Player.Alive)
                    {
                        continue;
                    }

                    self.Runtime.DeltaX =
                            self.Runtime.Player.PositionX - self.Runtime.Pickup.PositionX;
                    self.Runtime.DeltaY =
                            self.Runtime.Player.PositionY - self.Runtime.Pickup.PositionY;
                    self.Runtime.DistanceSquared =
                            (long)self.Runtime.DeltaX * self.Runtime.DeltaX +
                            (long)self.Runtime.DeltaY * self.Runtime.DeltaY;
                    if (self.Runtime.DistanceSquared >
                        (long)SurvivorDefaults.ExperiencePickupRange *
                        SurvivorDefaults.ExperiencePickupRange)
                    {
                        continue;
                    }

                    self.Runtime.Player.Experience += self.Runtime.Pickup.Experience;
                    self.Runtime.Collected = true;
                    break;
                }

                if (self.Runtime.Collected)
                {
                    self.Runtime.PickupRemovalStateIds.Add(self.Runtime.Pickup.StateId);
                }
            }

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

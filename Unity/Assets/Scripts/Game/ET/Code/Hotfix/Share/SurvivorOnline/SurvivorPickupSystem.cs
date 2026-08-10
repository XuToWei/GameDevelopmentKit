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
                SurvivorPickupState pickup = pickupEnumerator.Current.Value;
                bool collected = false;
                using var playerEnumerator = self.Data.Players.GetEnumerator();
                while (playerEnumerator.MoveNext())
                {
                    SurvivorPlayerState player = playerEnumerator.Current.Value;
                    if (!player.Alive)
                    {
                        continue;
                    }

                    int deltaX = player.PositionX - pickup.PositionX;
                    int deltaY = player.PositionY - pickup.PositionY;
                    long distanceSquared = (long)deltaX * deltaX + (long)deltaY * deltaY;
                    if (distanceSquared >
                        (long)SurvivorDefaults.ExperiencePickupRange * SurvivorDefaults.ExperiencePickupRange)
                    {
                        continue;
                    }

                    player.Experience += pickup.Experience;
                    collected = true;
                    break;
                }

                if (collected)
                {
                    self.Runtime.PickupRemovalStateIds.Add(pickup.StateId);
                }
            }

            for (int index = 0; index < self.Runtime.PickupRemovalStateIds.Count; index++)
            {
                self.Data.Pickups.Remove(self.Runtime.PickupRemovalStateIds[index]);
                self.Data.PickupSetRevision++;
            }
        }
    }
}

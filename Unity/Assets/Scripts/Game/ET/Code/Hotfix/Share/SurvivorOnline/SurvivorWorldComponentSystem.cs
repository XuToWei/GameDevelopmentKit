namespace ET
{
    [EntitySystemOf(typeof(SurvivorWorldComponent))]
    public static partial class SurvivorWorldComponentSystem
    {
        [EntitySystem]
        private static void Awake(this SurvivorWorldComponent self, SurvivorWorldRole role, string roomCode)
        {
            self.Role = role;
            self.Runtime = new SurvivorWorldRuntime();
            self.Data = role == SurvivorWorldRole.ServerAuthority ? SurvivorWorldFactory.CreateWorld(roomCode) : null;
            self.AttachTo(self.Runtime.SyncContext);
        }

        [EntitySystem]
        private static void Destroy(this SurvivorWorldComponent self)
        {
            self.Runtime.Dispose();
            self.Runtime = null;
        }

        public static byte[] CaptureFull(this SurvivorWorldComponent self)
        {
            self.Runtime.CaptureStream.Position = 0;
            self.Runtime.CaptureStream.SetLength(0);
            self.Runtime.SyncContext.CaptureFull(self.Runtime.CaptureWriter);
            self.Runtime.CaptureWriter.Flush();
            return self.Runtime.CaptureStream.ToArray();
        }

        public static byte[] CaptureDelta(this SurvivorWorldComponent self)
        {
            self.Runtime.CaptureStream.Position = 0;
            self.Runtime.CaptureStream.SetLength(0);
            self.Runtime.SyncContext.CaptureDelta(self.Runtime.CaptureWriter);
            self.Runtime.CaptureWriter.Flush();
            return self.Runtime.CaptureStream.ToArray();
        }

        public static void ApplySnapshot(this SurvivorWorldComponent self, byte[] payload)
        {
            self.Runtime.ApplyStream.Position = 0;
            self.Runtime.ApplyStream.SetLength(0);
            self.Runtime.ApplyStream.Write(payload, 0, payload.Length);
            self.Runtime.ApplyStream.Position = 0;
            self.Runtime.SyncContext.Apply(self.Runtime.ApplyReader);
        }

        public static long AddPlayer(this SurvivorWorldComponent self, long playerId, string displayName)
        {
            long stateId = self.AllocateStateId();
            SurvivorPlayerState player = SurvivorWorldFactory.CreatePlayer(stateId, playerId, displayName);
            self.Data.Players.Add(playerId, player);
            self.Data.PlayerSetRevision++;
            self.AttachPlayerReaction(player);
            if (self.Data.HostPlayerId == 0)
            {
                self.Data.HostPlayerId = playerId;
            }

            return stateId;
        }

        public static void ResetForLobby(this SurvivorWorldComponent self)
        {
            self.DetachStateReactions();
            SurvivorWorldFactory.ResetForLobby(self.Data);
            using var playerEnumerator = self.Data.Players.GetEnumerator();
            while (playerEnumerator.MoveNext())
            {
                self.AttachPlayerReaction(playerEnumerator.Current.Value);
            }
        }

        public static void SetPlayerInput(
            this SurvivorWorldComponent self,
            long playerId,
            long inputSequence,
            int moveX,
            int moveY)
        {
            if (!self.Data.Players.TryGetValue(playerId, out SurvivorPlayerState player) ||
                inputSequence <= player.LastInputSequence)
            {
                return;
            }

            player.LastInputSequence = inputSequence;
            player.MoveX = SurvivorMath.Clamp(moveX, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
            player.MoveY = SurvivorMath.Clamp(moveY, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
        }
    }
}

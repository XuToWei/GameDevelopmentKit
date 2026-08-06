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
            self.Runtime.FrameBytes = self.Runtime.CaptureStream.ToArray();
            return self.Runtime.FrameBytes;
        }

        public static byte[] CaptureDelta(this SurvivorWorldComponent self)
        {
            self.Runtime.CaptureStream.Position = 0;
            self.Runtime.CaptureStream.SetLength(0);
            self.Runtime.SyncContext.CaptureDelta(self.Runtime.CaptureWriter);
            self.Runtime.CaptureWriter.Flush();
            self.Runtime.FrameBytes = self.Runtime.CaptureStream.ToArray();
            return self.Runtime.FrameBytes;
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
            self.Runtime.StateId = self.Data.NextStateId;
            self.Data.NextStateId++;
            self.Data.Players.Add(
                playerId,
                SurvivorWorldFactory.CreatePlayer(self.Runtime.StateId, playerId, displayName));
            self.Data.PlayerSetRevision++;
            self.AttachPlayerReaction(self.Data.Players[playerId]);
            if (self.Data.HostPlayerId == 0)
            {
                self.Data.HostPlayerId = playerId;
            }

            return self.Runtime.StateId;
        }

        public static void ResetForLobby(this SurvivorWorldComponent self)
        {
            self.DetachStateReactions();
            SurvivorWorldFactory.ResetForLobby(self.Data);
            self.Runtime.PlayerEnumerator = self.Data.Players.GetEnumerator();
            while (self.Runtime.PlayerEnumerator.MoveNext())
            {
                self.AttachPlayerReaction(self.Runtime.PlayerEnumerator.Current.Value);
            }

            self.Runtime.PlayerEnumerator.Dispose();
            self.Runtime.PlayerEnumerator = null;
        }

        public static void SetPlayerInput(
            this SurvivorWorldComponent self,
            long playerId,
            long inputSequence,
            int moveX,
            int moveY)
        {
            if (!self.Data.Players.ContainsKey(playerId))
            {
                return;
            }

            if (inputSequence <= self.Data.Players[playerId].LastInputSequence)
            {
                return;
            }

            self.Data.Players[playerId].LastInputSequence = inputSequence;
            self.Data.Players[playerId].MoveX = SurvivorMath.Clamp(moveX, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
            self.Data.Players[playerId].MoveY = SurvivorMath.Clamp(moveY, -SurvivorDefaults.InputScale, SurvivorDefaults.InputScale);
        }

    }
}

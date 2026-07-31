using System;
using System.Collections.Generic;
using System.IO;
using ReactiveBinding;

namespace ET
{
    [EnableClass]
    public sealed class SurvivorWorldRuntime: IDisposable
    {
        public SurvivorWorldRuntime()
        {
            this.SyncContext = new SyncContext();
            this.CaptureStream = new MemoryStream();
            this.CaptureWriter = new BinaryWriter(this.CaptureStream);
            this.ApplyStream = new MemoryStream();
            this.ApplyReader = new BinaryReader(this.ApplyStream);
        }

        public SyncContext SyncContext { get; }

        public ISurvivorPlayerReactionSink PlayerReactionSink { get; set; }

        public ISurvivorMonsterReactionSink MonsterReactionSink { get; set; }

        public MemoryStream CaptureStream { get; }

        public BinaryWriter CaptureWriter { get; }

        public MemoryStream ApplyStream { get; }

        public BinaryReader ApplyReader { get; }

        public IEnumerator<KeyValuePair<long, SurvivorPlayerState>> PlayerEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorMonsterState>> MonsterEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorProjectileState>> ProjectileEnumerator { get; set; }

        public IEnumerator<KeyValuePair<long, SurvivorPickupState>> PickupEnumerator { get; set; }

        public List<long> MonsterRemovalStateIds { get; } = new();

        public List<long> ProjectileRemovalStateIds { get; } = new();

        public List<long> PickupRemovalStateIds { get; } = new();

        public SurvivorPlayerState Player { get; set; }

        public SurvivorMonsterState Monster { get; set; }

        public SurvivorProjectileState Projectile { get; set; }

        public SurvivorPickupState Pickup { get; set; }

        public SurvivorPlayerState TargetPlayer { get; set; }

        public SurvivorMonsterState TargetMonster { get; set; }

        public long StateId { get; set; }

        public long TargetPlayerId { get; set; }

        public int Index { get; set; }

        public int DeltaX { get; set; }

        public int DeltaY { get; set; }

        public int Distance { get; set; }

        public int SpawnPositionX { get; set; }

        public int SpawnPositionY { get; set; }

        public int VelocityX { get; set; }

        public int VelocityY { get; set; }

        public int AlivePlayerCount { get; set; }

        public bool Hit { get; set; }

        public bool Collected { get; set; }

        public byte[] FrameBytes { get; set; }

        public void Dispose()
        {
            this.PlayerEnumerator?.Dispose();
            this.MonsterEnumerator?.Dispose();
            this.ProjectileEnumerator?.Dispose();
            this.PickupEnumerator?.Dispose();
            this.ApplyReader.Dispose();
            this.ApplyStream.Dispose();
            this.CaptureWriter.Dispose();
            this.CaptureStream.Dispose();
        }
    }
}

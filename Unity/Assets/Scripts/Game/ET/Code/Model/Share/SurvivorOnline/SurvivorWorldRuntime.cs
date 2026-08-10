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

        public MemoryStream CaptureStream { get; }

        public BinaryWriter CaptureWriter { get; }

        public MemoryStream ApplyStream { get; }

        public BinaryReader ApplyReader { get; }

        public List<long> ObserverIds { get; } = new();

        public List<long> ProjectileRemovalStateIds { get; } = new();

        public List<long> SwordWaveHitStateIds { get; } = new();

        public List<long> PickupRemovalStateIds { get; } = new();

        public void Dispose()
        {
            this.ApplyReader.Dispose();
            this.ApplyStream.Dispose();
            this.CaptureWriter.Dispose();
            this.CaptureStream.Dispose();
        }
    }
}

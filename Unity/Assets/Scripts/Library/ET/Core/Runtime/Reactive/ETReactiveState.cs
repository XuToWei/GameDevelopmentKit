using System;
using System.Collections.Generic;
using System.Threading;

namespace ET
{
    /// <summary>
    /// Stable storage for reactive values used by fieldless Hotfix systems.
    /// </summary>
    [EnableClass]
    public sealed class ETReactiveState
    {
        private readonly Dictionary<long, ETReactiveGroupState> groups = new();

        public ETReactiveGroupState GetOrCreateGroup(long groupId, long schemaId, int slotCount)
        {
            if (!this.groups.TryGetValue(groupId, out ETReactiveGroupState group))
            {
                group = new ETReactiveGroupState(schemaId, ETReactiveRuntime.ReloadVersion, slotCount);
                this.groups.Add(groupId, group);
                return group;
            }

            group.EnsureSchema(schemaId, ETReactiveRuntime.ReloadVersion, slotCount);
            return group;
        }

        public void Reset(long groupId)
        {
            if (this.groups.TryGetValue(groupId, out ETReactiveGroupState group))
            {
                group.Reset();
            }
        }

        public void ResetAll()
        {
            foreach (ETReactiveGroupState group in this.groups.Values)
            {
                group.Reset();
            }
        }

        public void Clear()
        {
            this.groups.Clear();
        }
    }

    [EnableClass]
    public sealed class ETReactiveGroupState
    {
        private long schemaId;
        private long reloadVersion;
        private object[] slots;

        public bool Initialized { get; set; }

        internal ETReactiveGroupState(long schemaId, long reloadVersion, int slotCount)
        {
            this.schemaId = schemaId;
            this.reloadVersion = reloadVersion;
            this.slots = new object[slotCount];
        }

        internal void EnsureSchema(long schemaId, long reloadVersion, int slotCount)
        {
            if (this.schemaId == schemaId &&
                this.reloadVersion == reloadVersion &&
                this.slots.Length == slotCount)
            {
                return;
            }

            this.schemaId = schemaId;
            this.reloadVersion = reloadVersion;
            this.slots = new object[slotCount];
            this.Initialized = false;
        }

        public ETReactiveSlot<T> GetSlot<T>(int index)
        {
            if ((uint)index >= (uint)this.slots.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            object value = this.slots[index];
            if (value == null)
            {
                ETReactiveSlot<T> slot = new();
                this.slots[index] = slot;
                return slot;
            }

            if (value is ETReactiveSlot<T> typedSlot)
            {
                return typedSlot;
            }

            throw new InvalidOperationException(
                $"Reactive slot type mismatch at index {index}. Reset the binding schema before changing source types.");
        }

        internal void Reset()
        {
            this.Initialized = false;
        }
    }

    [EnableClass]
    public sealed class ETReactiveSlot<T>
    {
        public T Value { get; set; }
    }

    public static class ETReactiveRuntime
    {
        [StaticField]
        private static long reloadVersion;

        public static long ReloadVersion => Interlocked.Read(ref reloadVersion);

        public static void NotifyCodeReload()
        {
            Interlocked.Increment(ref reloadVersion);
        }
    }
}

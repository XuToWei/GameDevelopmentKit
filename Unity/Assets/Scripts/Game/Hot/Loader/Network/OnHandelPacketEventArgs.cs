using GameFramework;
using GameFramework.Event;

namespace Game
{
    public sealed class OnHandelPacketEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnHandelPacketEventArgs).GetHashCode();

        public override int Id => EventId;

        public SCPacketBase Packet { get; private set; }

        public static OnHandelPacketEventArgs Create(SCPacketBase scPacket)
        {
            OnHandelPacketEventArgs eventArgs = ReferencePool.Acquire<OnHandelPacketEventArgs>();
            eventArgs.Packet = scPacket;
            return eventArgs;
        }

        public override void Clear()
        {
            Packet = null;
        }
    }
}

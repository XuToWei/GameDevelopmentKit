//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ProtoBuf;
using Sirenix.OdinInspector;

namespace Game
{
    public abstract class CSPacketBase : PacketBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ClientToServer;
            }
        }

        [ProtoMember(-1), ShowInInspector]
        public uint CorrelationID { get; private set; }

        private static uint s_GlobalCorrelationID = 0;

        public void IncrementCorrelationID()
        {
            if (s_GlobalCorrelationID >= uint.MaxValue)
            {
                s_GlobalCorrelationID = 0;
            }
            CorrelationID = ++s_GlobalCorrelationID;
        }

        public override void Clear()
        {
            CorrelationID = 0;
        }
    }
}

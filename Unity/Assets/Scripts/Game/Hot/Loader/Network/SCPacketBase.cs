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
    public abstract class SCPacketBase : PacketBase
    {
        public override PacketType PacketType
        {
            get
            {
                return PacketType.ServerToClient;
            }
        }

        [ProtoMember(-1), ShowInInspector]
        public uint CorrelationID { get; private set; }

        public void SetCorrelationID(uint correlationID)
        {
            CorrelationID = correlationID;
        }

        public override void Clear()
        {
            CorrelationID = 0;
        }
    }
}

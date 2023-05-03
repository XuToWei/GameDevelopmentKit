using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityGameFramework.Extension;
namespace Game
{
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest")]
    public partial class CSHeartBeatTest : SCPacketBase
    {
        public override int Id => 30002;
        [ProtoMember(1)]
        public List<int> A { get; set; }

        public override void Clear()
        {
            A?.Clear();
        }
    }

    [Serializable, ProtoContract(Name = @"SCHeartBeatTest")]
    public partial class SCHeartBeatTest : SCPacketBase
    {
        public override int Id => 30003;
        [ProtoMember(1)]
        public List<int> A { get; set; }

        public override void Clear()
        {
            A?.Clear();
        }
    }
}

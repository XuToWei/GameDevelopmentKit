// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;
namespace Game.Hot
{
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest")]
    public partial class CSHeartBeatTest : CSPacketBase
    {
        public override int Id => 30002;
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();

        public override void Clear()
        {
            A.Clear();
        }
    }

    [Serializable, ProtoContract(Name = @"SCHeartBeatTest")]
    public partial class SCHeartBeatTest : SCPacketBase
    {
        public override int Id => 30003;
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();

        public override void Clear()
        {
            A.Clear();
        }
    }

}

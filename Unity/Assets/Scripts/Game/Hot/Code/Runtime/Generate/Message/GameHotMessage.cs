// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Game.Hot
{
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest")]
    //protofile : UGF/GameHot.proto
    public partial class CSHeartBeatTest : CSPacketBase
    {
        public override int Id => 30001;
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(2)]
        public string B { get; set; }
        [ProtoMember(3)]
        public Dictionary<int, long> C { get; set; } = new Dictionary<int, long>();
        public override void Clear()
        {
            this.A.Clear();
            this.B = default;
            this.C.Clear();
        }
    }

    [Serializable, ProtoContract(Name = @"SCHeartBeatTest")]
    //protofile : UGF/GameHot.proto
    public partial class SCHeartBeatTest : SCPacketBase
    {
        public override int Id => 30002;
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        public override void Clear()
        {
            this.A.Clear();
        }
    }

    [Serializable, ProtoContract(Name = @"CSHeartBeatTest22")]
    //protofile : UGF/GameHot2.proto
    public partial class CSHeartBeatTest22 : CSPacketBase
    {
        public override int Id => 30003;
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        public override void Clear()
        {
            this.A.Clear();
        }
    }

    [Serializable, ProtoContract(Name = @"SCHeartBeatTest22")]
    //protofile : UGF/GameHot2.proto
    public partial class SCHeartBeatTest22 : SCPacketBase
    {
        public override int Id => 30004;
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        public override void Clear()
        {
            this.A.Clear();
        }
    }

}

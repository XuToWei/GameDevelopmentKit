// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Game.Hot
{
    /// <summary>
    /// 心跳测试
    /// </summary>
    // protofile : GameHot/GameHot.proto
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest")]
    public partial class CSHeartBeatTest : CSPacketBase
    {
        public override int Id => 30001;
        /// <summary>
        /// 测试A
        /// </summary>
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        /// <summary>
        /// 测试B
        /// </summary>
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

    // protofile : GameHot/GameHot.proto
    [Serializable, ProtoContract(Name = @"SCHeartBeatTest")]
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

    // protofile : GameHot/GameHot2.proto
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest22")]
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

    // protofile : GameHot/GameHot2.proto
    [Serializable, ProtoContract(Name = @"SCHeartBeatTest22")]
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

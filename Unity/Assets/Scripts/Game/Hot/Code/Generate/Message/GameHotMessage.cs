// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Game.Hot
{
    /// <summary>
    /// 心跳测试
    /// </summary>
    // proto file : GameHot/GameHot.proto (line:4)
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

    // proto file : GameHot/GameHot.proto (line:11)
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

    // proto file : GameHot/GameHot2.proto (line:3)
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest22")]
    public partial class CSHeartBeatTest22 : CSPacketBase
    {
        public override int Id => 30003;
        /// <summary>
        /// 测试A
        /// </summary>
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(1)]
        public List<string> B { get; set; } = new List<string>();
        public override void Clear()
        {
            this.A.Clear();
            this.B.Clear();
        }
    }

    // proto file : GameHot/GameHot2.proto (line:10)
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

    // 测试枚举
    // proto file : GameHot/GameHot2.proto (line:16)
    public enum TestEnum    {
        /// <summary>
        /// 测试A
        /// </summary>
        A = 1,
        B = 2,
    }

    public static partial class GameHotMessageId
    {
         public const ushort CSHeartBeatTest = 30001;
         public const ushort SCHeartBeatTest = 30002;
         public const ushort CSHeartBeatTest22 = 30003;
         public const ushort SCHeartBeatTest22 = 30004;
    }
}

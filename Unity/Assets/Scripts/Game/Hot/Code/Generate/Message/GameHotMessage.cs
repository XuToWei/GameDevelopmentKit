// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;
using GameFramework;

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

    // proto file : GameHot/GameHot.proto (line:16)
    [Serializable, ProtoContract(Name = @"SCTest")]
    public partial class SCTest : SCPacketBase
    {
        public override int Id => 30003;
        [ProtoMember(1)]
        public List<TestClass> A { get; set; } = new List<TestClass>();
        [ProtoMember(2)]
        public List<string> B { get; set; } = new List<string>();
        public override void Clear()
        {
            foreach(var item in A)
            {
                ReferencePool.Release(item);
            }
            this.A.Clear();
            this.B.Clear();
        }
    }

    // proto file : GameHot/GameHot.proto (line:22)
    [Serializable, ProtoContract(Name = @"TestClass")]
    public partial class TestClass : IReference
    {
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(2)]
        public List<string> B { get; set; } = new List<string>();
        [ProtoMember(3)]
        public List<TestClass2> C { get; set; } = new List<TestClass2>();
        [ProtoMember(4)]
        public TestClass2 D { get; set; }
        [ProtoMember(5)]
        public Dictionary<int, TestClass2> E { get; set; } = new Dictionary<int, TestClass2>();
        public void Clear()
        {
            this.A.Clear();
            this.B.Clear();
            foreach(var item in C)
            {
                ReferencePool.Release(item);
            }
            this.C.Clear();
            if(this.D != null)
            {
                ReferencePool.Release(this.D);
            }
            this.D = default;
            foreach(var item in E.Values)
            {
                ReferencePool.Release(item);
            }
            this.E.Clear();
        }
    }

    // proto file : GameHot/GameHot.proto (line:31)
    [Serializable, ProtoContract(Name = @"TestClass2")]
    public partial class TestClass2 : IReference
    {
        [ProtoMember(1)]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(2)]
        public List<string> B { get; set; } = new List<string>();
        [ProtoMember(3)]
        public Dictionary<int, long> C { get; set; } = new Dictionary<int, long>();
        [ProtoMember(4)]
        public string D { get; set; }
        public void Clear()
        {
            this.A.Clear();
            this.B.Clear();
            this.C.Clear();
            this.D = default;
        }
    }

    // proto file : GameHot/GameHot2.proto (line:3)
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest22")]
    public partial class CSHeartBeatTest22 : CSPacketBase
    {
        public override int Id => 30006;
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
        public override int Id => 30007;
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
         public const ushort SCTest = 30003;
         public const ushort TestClass = 30004;
         public const ushort TestClass2 = 30005;
         public const ushort CSHeartBeatTest22 = 30006;
         public const ushort SCHeartBeatTest22 = 30007;
    }
}

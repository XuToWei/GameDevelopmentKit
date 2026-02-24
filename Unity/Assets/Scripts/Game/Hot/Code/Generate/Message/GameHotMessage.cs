// This is an automatically generated class by Share.Tool. Please do not modify it.

using ProtoBuf;
using System;
using System.Collections.Generic;
using GameFramework;
using Sirenix.OdinInspector;

namespace Game.Hot
{
    /// <summary>
    /// 心跳测试
    /// </summary>
    // proto file : GameHot/GameHot.proto (line:4)
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest"), ShowInInspector]
    public partial class CSHeartBeatTest : CSPacketBase
    {
        [ShowInInspector]
        public override int Id => GameHotMessageId.CSHeartBeatTest;
        /// <summary>
        /// 测试A
        /// </summary>
        [ProtoMember(1), ShowInInspector]
        public List<int> A { get; set; } = new List<int>();
        /// <summary>
        /// 测试B
        /// </summary>
        [ProtoMember(2), ShowInInspector]
        public string B { get; set; }
        [ProtoMember(3), ShowInInspector]
        public Dictionary<int, long> C { get; set; } = new Dictionary<int, long>();
        public override void Clear()
        {
            base.Clear();
            this.A.Clear();
            this.B = default;
            this.C.Clear();
        }
    }

    // proto file : GameHot/GameHot.proto (line:11)
    [Serializable, ProtoContract(Name = @"SCHeartBeatTest"), ShowInInspector]
    public partial class SCHeartBeatTest : SCPacketBase
    {
        [ShowInInspector]
        public override int Id => GameHotMessageId.SCHeartBeatTest;
        [ProtoMember(1), ShowInInspector]
        public List<int> A { get; set; } = new List<int>();
        public override void Clear()
        {
            base.Clear();
            this.A.Clear();
        }
    }

    // proto file : GameHot/GameHot.proto (line:16)
    [Serializable, ProtoContract(Name = @"SCTest"), ShowInInspector]
    public partial class SCTest : SCPacketBase
    {
        [ShowInInspector]
        public override int Id => GameHotMessageId.SCTest;
        [ProtoMember(1), ShowInInspector]
        public List<TestClass> A { get; set; } = new List<TestClass>();
        [ProtoMember(2), ShowInInspector]
        public List<string> B { get; set; } = new List<string>();
        public override void Clear()
        {
            base.Clear();
            foreach(var item in A)
            {
                ReferencePool.Release(item);
            }
            this.A.Clear();
            this.B.Clear();
        }
    }

    // proto file : GameHot/GameHot.proto (line:22)
    [Serializable, ProtoContract(Name = @"TestClass"), ShowInInspector]
    public partial class TestClass : IReference
    {
        [ProtoMember(1), ShowInInspector]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(2), ShowInInspector]
        public List<string> B { get; set; } = new List<string>();
        [ProtoMember(3), ShowInInspector]
        public List<TestClass2> C { get; set; } = new List<TestClass2>();
        [ProtoMember(4), ShowInInspector]
        public TestClass2 D { get; set; }
        [ProtoMember(5), ShowInInspector]
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
    [Serializable, ProtoContract(Name = @"TestClass2"), ShowInInspector]
    public partial class TestClass2 : IReference
    {
        [ProtoMember(1), ShowInInspector]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(2), ShowInInspector]
        public List<string> B { get; set; } = new List<string>();
        [ProtoMember(3), ShowInInspector]
        public Dictionary<int, long> C { get; set; } = new Dictionary<int, long>();
        [ProtoMember(4), ShowInInspector]
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
    [Serializable, ProtoContract(Name = @"CSHeartBeatTest22"), ShowInInspector]
    public partial class CSHeartBeatTest22 : CSPacketBase
    {
        [ShowInInspector]
        public override int Id => GameHotMessageId.CSHeartBeatTest22;
        /// <summary>
        /// 测试A
        /// </summary>
        [ProtoMember(1), ShowInInspector]
        public List<int> A { get; set; } = new List<int>();
        [ProtoMember(1), ShowInInspector]
        public List<string> B { get; set; } = new List<string>();
        public override void Clear()
        {
            base.Clear();
            this.A.Clear();
            this.B.Clear();
        }
    }

    // proto file : GameHot/GameHot2.proto (line:10)
    [Serializable, ProtoContract(Name = @"SCHeartBeatTest22"), ShowInInspector]
    public partial class SCHeartBeatTest22 : SCPacketBase
    {
        [ShowInInspector]
        public override int Id => GameHotMessageId.SCHeartBeatTest22;
        [ProtoMember(1), ShowInInspector]
        public List<int> A { get; set; } = new List<int>();
        public override void Clear()
        {
            base.Clear();
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

}

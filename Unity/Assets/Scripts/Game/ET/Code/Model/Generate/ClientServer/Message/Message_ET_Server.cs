// This is an automatically generated class by Share.Tool. Please do not modify it.

using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectQueryRequest)]
    [ResponseType(nameof(ObjectQueryResponse))]
    public partial class ObjectQueryRequest: MessageObject, IRequest
    {
        public static ObjectQueryRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectQueryRequest), isFromPool) as ObjectQueryRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long Key { get; set; }
        [MemoryPackOrder(2)]
        public long InstanceId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Key = default;
            this.InstanceId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.M2A_Reload)]
    [ResponseType(nameof(A2M_Reload))]
    public partial class M2A_Reload: MessageObject, IRequest
    {
        public static M2A_Reload Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2A_Reload), isFromPool) as M2A_Reload; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.A2M_Reload)]
    public partial class A2M_Reload: MessageObject, IResponse
    {
        public static A2M_Reload Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(A2M_Reload), isFromPool) as A2M_Reload; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2G_LockRequest)]
    [ResponseType(nameof(G2G_LockResponse))]
    public partial class G2G_LockRequest: MessageObject, IRequest
    {
        public static G2G_LockRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2G_LockRequest), isFromPool) as G2G_LockRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long Id { get; set; }
        [MemoryPackOrder(2)]
        public string Address { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Id = default;
            this.Address = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2G_LockResponse)]
    public partial class G2G_LockResponse: MessageObject, IResponse
    {
        public static G2G_LockResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2G_LockResponse), isFromPool) as G2G_LockResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2G_LockReleaseRequest)]
    [ResponseType(nameof(G2G_LockReleaseResponse))]
    public partial class G2G_LockReleaseRequest: MessageObject, IRequest
    {
        public static G2G_LockReleaseRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2G_LockReleaseRequest), isFromPool) as G2G_LockReleaseRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long Id { get; set; }
        [MemoryPackOrder(2)]
        public string Address { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Id = default;
            this.Address = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2G_LockReleaseResponse)]
    public partial class G2G_LockReleaseResponse: MessageObject, IResponse
    {
        public static G2G_LockReleaseResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2G_LockReleaseResponse), isFromPool) as G2G_LockReleaseResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectAddRequest)]
    [ResponseType(nameof(ObjectAddResponse))]
    public partial class ObjectAddRequest: MessageObject, IRequest
    {
        public static ObjectAddRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectAddRequest), isFromPool) as ObjectAddRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Type { get; set; }
        [MemoryPackOrder(2)]
        public long Key { get; set; }
        [MemoryPackOrder(3)]
        public ActorId ActorId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            this.Key = default;
            this.ActorId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectAddResponse)]
    public partial class ObjectAddResponse: MessageObject, IResponse
    {
        public static ObjectAddResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectAddResponse), isFromPool) as ObjectAddResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectLockRequest)]
    [ResponseType(nameof(ObjectLockResponse))]
    public partial class ObjectLockRequest: MessageObject, IRequest
    {
        public static ObjectLockRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectLockRequest), isFromPool) as ObjectLockRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Type { get; set; }
        [MemoryPackOrder(2)]
        public long Key { get; set; }
        [MemoryPackOrder(3)]
        public ActorId ActorId { get; set; }
        [MemoryPackOrder(4)]
        public int Time { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            this.Key = default;
            this.ActorId = default;
            this.Time = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectLockResponse)]
    public partial class ObjectLockResponse: MessageObject, IResponse
    {
        public static ObjectLockResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectLockResponse), isFromPool) as ObjectLockResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectUnLockRequest)]
    [ResponseType(nameof(ObjectUnLockResponse))]
    public partial class ObjectUnLockRequest: MessageObject, IRequest
    {
        public static ObjectUnLockRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectUnLockRequest), isFromPool) as ObjectUnLockRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Type { get; set; }
        [MemoryPackOrder(2)]
        public long Key { get; set; }
        [MemoryPackOrder(3)]
        public ActorId OldActorId { get; set; }
        [MemoryPackOrder(4)]
        public ActorId NewActorId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            this.Key = default;
            this.OldActorId = default;
            this.NewActorId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectUnLockResponse)]
    public partial class ObjectUnLockResponse: MessageObject, IResponse
    {
        public static ObjectUnLockResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectUnLockResponse), isFromPool) as ObjectUnLockResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectRemoveRequest)]
    [ResponseType(nameof(ObjectRemoveResponse))]
    public partial class ObjectRemoveRequest: MessageObject, IRequest
    {
        public static ObjectRemoveRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectRemoveRequest), isFromPool) as ObjectRemoveRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Type { get; set; }
        [MemoryPackOrder(2)]
        public long Key { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            this.Key = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectRemoveResponse)]
    public partial class ObjectRemoveResponse: MessageObject, IResponse
    {
        public static ObjectRemoveResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectRemoveResponse), isFromPool) as ObjectRemoveResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectGetRequest)]
    [ResponseType(nameof(ObjectGetResponse))]
    public partial class ObjectGetRequest: MessageObject, IRequest
    {
        public static ObjectGetRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectGetRequest), isFromPool) as ObjectGetRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Type { get; set; }
        [MemoryPackOrder(2)]
        public long Key { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            this.Key = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectGetResponse)]
    public partial class ObjectGetResponse: MessageObject, IResponse
    {
        public static ObjectGetResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectGetResponse), isFromPool) as ObjectGetResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int Type { get; set; }
        [MemoryPackOrder(4)]
        public ActorId ActorId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Type = default;
            this.ActorId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.R2G_GetLoginKey)]
    [ResponseType(nameof(G2R_GetLoginKey))]
    public partial class R2G_GetLoginKey: MessageObject, IRequest
    {
        public static R2G_GetLoginKey Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(R2G_GetLoginKey), isFromPool) as R2G_GetLoginKey; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Account { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Account = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2R_GetLoginKey)]
    public partial class G2R_GetLoginKey: MessageObject, IResponse
    {
        public static G2R_GetLoginKey Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2R_GetLoginKey), isFromPool) as G2R_GetLoginKey; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long Key { get; set; }
        [MemoryPackOrder(4)]
        public long GateId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Key = default;
            this.GateId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2M_SessionDisconnect)]
    public partial class G2M_SessionDisconnect: MessageObject, ILocationMessage
    {
        public static G2M_SessionDisconnect Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2M_SessionDisconnect), isFromPool) as G2M_SessionDisconnect; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.ObjectQueryResponse)]
    public partial class ObjectQueryResponse: MessageObject, IResponse
    {
        public static ObjectQueryResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(ObjectQueryResponse), isFromPool) as ObjectQueryResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public byte[] Entity { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Entity = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.M2M_UnitTransferRequest)]
    [ResponseType(nameof(M2M_UnitTransferResponse))]
    public partial class M2M_UnitTransferRequest: MessageObject, IRequest
    {
        public static M2M_UnitTransferRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2M_UnitTransferRequest), isFromPool) as M2M_UnitTransferRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public ActorId OldActorId { get; set; }
        [MemoryPackOrder(2)]
        public byte[] Unit { get; set; }
        [MemoryPackOrder(3)]
        public List<byte[]> Entitys { get; set; } = new List<byte[]>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.OldActorId = default;
            this.Unit = default;
            this.Entitys.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/InnerMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Server.M2M_UnitTransferResponse)]
    public partial class M2M_UnitTransferResponse: MessageObject, IResponse
    {
        public static M2M_UnitTransferResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2M_UnitTransferResponse), isFromPool) as M2M_UnitTransferResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    /// <summary>
    /// 请求匹配
    /// </summary>
    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2Match_Match)]
    [ResponseType(nameof(Match2G_Match))]
    public partial class G2Match_Match: MessageObject, IRequest
    {
        public static G2Match_Match Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2Match_Match), isFromPool) as G2Match_Match; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long Id { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Id = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.Match2G_Match)]
    public partial class Match2G_Match: MessageObject, IResponse
    {
        public static Match2G_Match Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Match2G_Match), isFromPool) as Match2G_Match; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.Match2Map_GetRoom)]
    [ResponseType(nameof(Map2Match_GetRoom))]
    public partial class Match2Map_GetRoom: MessageObject, IRequest
    {
        public static Match2Map_GetRoom Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Match2Map_GetRoom), isFromPool) as Match2Map_GetRoom; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public List<long> PlayerIds { get; set; } = new List<long>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.PlayerIds.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.Map2Match_GetRoom)]
    public partial class Map2Match_GetRoom: MessageObject, IResponse
    {
        public static Map2Match_GetRoom Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Map2Match_GetRoom), isFromPool) as Map2Match_GetRoom; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        /// <summary>
        /// 房间的ActorId
        /// </summary>
        [MemoryPackOrder(3)]
        public ActorId ActorId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.ActorId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.G2Room_Reconnect)]
    [ResponseType(nameof(Room2G_Reconnect))]
    public partial class G2Room_Reconnect: MessageObject, IRequest
    {
        public static G2Room_Reconnect Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2Room_Reconnect), isFromPool) as G2Room_Reconnect; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.PlayerId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.Room2G_Reconnect)]
    public partial class Room2G_Reconnect: MessageObject, IResponse
    {
        public static Room2G_Reconnect Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Room2G_Reconnect), isFromPool) as Room2G_Reconnect; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long StartTime { get; set; }
        [MemoryPackOrder(4)]
        public List<LockStepUnitInfo> UnitInfos { get; set; } = new List<LockStepUnitInfo>();
        [MemoryPackOrder(5)]
        public int Frame { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.StartTime = default;
            this.UnitInfos.Clear();
            this.Frame = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.RoomManager2Room_Init)]
    [ResponseType(nameof(Room2RoomManager_Init))]
    public partial class RoomManager2Room_Init: MessageObject, IRequest
    {
        public static RoomManager2Room_Init Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(RoomManager2Room_Init), isFromPool) as RoomManager2Room_Init; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public List<long> PlayerIds { get; set; } = new List<long>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.PlayerIds.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-ClientServer/LockStepInner.proto
    [MemoryPackable]
    [Message(Message_ET_Server.Room2RoomManager_Init)]
    public partial class Room2RoomManager_Init: MessageObject, IResponse
    {
        public static Room2RoomManager_Init Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Room2RoomManager_Init), isFromPool) as Room2RoomManager_Init; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    public static partial class Message_ET_Server
    {
         public const ushort ObjectQueryRequest = 20001;
         public const ushort M2A_Reload = 20002;
         public const ushort A2M_Reload = 20003;
         public const ushort G2G_LockRequest = 20004;
         public const ushort G2G_LockResponse = 20005;
         public const ushort G2G_LockReleaseRequest = 20006;
         public const ushort G2G_LockReleaseResponse = 20007;
         public const ushort ObjectAddRequest = 20008;
         public const ushort ObjectAddResponse = 20009;
         public const ushort ObjectLockRequest = 20010;
         public const ushort ObjectLockResponse = 20011;
         public const ushort ObjectUnLockRequest = 20012;
         public const ushort ObjectUnLockResponse = 20013;
         public const ushort ObjectRemoveRequest = 20014;
         public const ushort ObjectRemoveResponse = 20015;
         public const ushort ObjectGetRequest = 20016;
         public const ushort ObjectGetResponse = 20017;
         public const ushort R2G_GetLoginKey = 20018;
         public const ushort G2R_GetLoginKey = 20019;
         public const ushort G2M_SessionDisconnect = 20020;
         public const ushort ObjectQueryResponse = 20021;
         public const ushort M2M_UnitTransferRequest = 20022;
         public const ushort M2M_UnitTransferResponse = 20023;
         public const ushort G2Match_Match = 20024;
         public const ushort Match2G_Match = 20025;
         public const ushort Match2Map_GetRoom = 20026;
         public const ushort Map2Match_GetRoom = 20027;
         public const ushort G2Room_Reconnect = 20028;
         public const ushort Room2G_Reconnect = 20029;
         public const ushort RoomManager2Room_Init = 20030;
         public const ushort Room2RoomManager_Init = 20031;
    }
}

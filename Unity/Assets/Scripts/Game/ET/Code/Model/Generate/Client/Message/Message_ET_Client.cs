// This is an automatically generated class by Share.Tool. Please do not modify it.

using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    // protofile : ET-Client/ClientMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.Main2NetClient_Login)]
    [ResponseType(nameof(NetClient2Main_Login))]
    public partial class Main2NetClient_Login: MessageObject, IRequest
    {
        public static Main2NetClient_Login Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Main2NetClient_Login), isFromPool) as Main2NetClient_Login; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int OwnerFiberId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [MemoryPackOrder(2)]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MemoryPackOrder(3)]
        public string Password { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.OwnerFiberId = default;
            this.Account = default;
            this.Password = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/ClientMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.NetClient2Main_Login)]
    public partial class NetClient2Main_Login: MessageObject, IResponse
    {
        public static NetClient2Main_Login Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(NetClient2Main_Login), isFromPool) as NetClient2Main_Login; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long PlayerId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.PlayerId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2G_Match)]
    [ResponseType(nameof(G2C_Match))]
    public partial class C2G_Match: MessageObject, ISessionRequest
    {
        public static C2G_Match Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2G_Match), isFromPool) as C2G_Match; 
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

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_Match)]
    public partial class G2C_Match: MessageObject, ISessionResponse
    {
        public static G2C_Match Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_Match), isFromPool) as G2C_Match; 
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
    /// 匹配成功，通知客户端切换场景
    /// </summary>
    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.Match2G_NotifyMatchSuccess)]
    public partial class Match2G_NotifyMatchSuccess: MessageObject, IMessage
    {
        public static Match2G_NotifyMatchSuccess Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Match2G_NotifyMatchSuccess), isFromPool) as Match2G_NotifyMatchSuccess; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// 房间的ActorId
        /// </summary>
        [MemoryPackOrder(1)]
        public ActorId ActorId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.ActorId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    /// <summary>
    /// 客户端通知房间切换场景完成
    /// </summary>
    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2Room_ChangeSceneFinish)]
    public partial class C2Room_ChangeSceneFinish: MessageObject, IRoomMessage
    {
        public static C2Room_ChangeSceneFinish Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2Room_ChangeSceneFinish), isFromPool) as C2Room_ChangeSceneFinish; 
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.PlayerId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.LockStepUnitInfo)]
    public partial class LockStepUnitInfo: MessageObject
    {
        public static LockStepUnitInfo Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(LockStepUnitInfo), isFromPool) as LockStepUnitInfo; 
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(1)]
        public TrueSync.TSVector Position { get; set; }
        [MemoryPackOrder(2)]
        public TrueSync.TSQuaternion Rotation { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.PlayerId = default;
            this.Position = default;
            this.Rotation = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    /// <summary>
    /// 房间通知客户端进入战斗
    /// </summary>
    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.Room2C_Start)]
    public partial class Room2C_Start: MessageObject, IMessage
    {
        public static Room2C_Start Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Room2C_Start), isFromPool) as Room2C_Start; 
        }

        [MemoryPackOrder(0)]
        public long StartTime { get; set; }
        [MemoryPackOrder(1)]
        public List<LockStepUnitInfo> UnitInfo { get; set; } = new List<LockStepUnitInfo>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.StartTime = default;
            this.UnitInfo.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.FrameMessage)]
    public partial class FrameMessage: MessageObject, IMessage
    {
        public static FrameMessage Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(FrameMessage), isFromPool) as FrameMessage; 
        }

        [MemoryPackOrder(0)]
        public int Frame { get; set; }
        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(2)]
        public LSInput Input { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Frame = default;
            this.PlayerId = default;
            this.Input = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.OneFrameInputs)]
    public partial class OneFrameInputs: MessageObject, IMessage
    {
        public static OneFrameInputs Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(OneFrameInputs), isFromPool) as OneFrameInputs; 
        }

        [MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
        [MemoryPackOrder(1)]
        public Dictionary<long, LSInput> Inputs { get; set; } = new();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Inputs.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.Room2C_AdjustUpdateTime)]
    public partial class Room2C_AdjustUpdateTime: MessageObject, IMessage
    {
        public static Room2C_AdjustUpdateTime Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Room2C_AdjustUpdateTime), isFromPool) as Room2C_AdjustUpdateTime; 
        }

        [MemoryPackOrder(0)]
        public int DiffTime { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.DiffTime = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2Room_CheckHash)]
    public partial class C2Room_CheckHash: MessageObject, IRoomMessage
    {
        public static C2Room_CheckHash Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2Room_CheckHash), isFromPool) as C2Room_CheckHash; 
        }

        [MemoryPackOrder(0)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(1)]
        public int Frame { get; set; }
        [MemoryPackOrder(2)]
        public long Hash { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.PlayerId = default;
            this.Frame = default;
            this.Hash = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.Room2C_CheckHashFail)]
    public partial class Room2C_CheckHashFail: MessageObject, IMessage
    {
        public static Room2C_CheckHashFail Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Room2C_CheckHashFail), isFromPool) as Room2C_CheckHashFail; 
        }

        [MemoryPackOrder(0)]
        public int Frame { get; set; }
        [MemoryPackOrder(1)]
        public byte[] LSWorldBytes { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Frame = default;
            this.LSWorldBytes = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/LockStepOuter.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_Reconnect)]
    public partial class G2C_Reconnect: MessageObject, IMessage
    {
        public static G2C_Reconnect Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_Reconnect), isFromPool) as G2C_Reconnect; 
        }

        [MemoryPackOrder(0)]
        public long StartTime { get; set; }
        [MemoryPackOrder(1)]
        public List<LockStepUnitInfo> UnitInfos { get; set; } = new List<LockStepUnitInfo>();
        [MemoryPackOrder(2)]
        public int Frame { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.StartTime = default;
            this.UnitInfos.Clear();
            this.Frame = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.HttpGetRouterResponse)]
    public partial class HttpGetRouterResponse: MessageObject
    {
        public static HttpGetRouterResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(HttpGetRouterResponse), isFromPool) as HttpGetRouterResponse; 
        }

        [MemoryPackOrder(0)]
        public List<string> Realms { get; set; } = new List<string>();
        [MemoryPackOrder(1)]
        public List<string> Routers { get; set; } = new List<string>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Realms.Clear();
            this.Routers.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.RouterSync)]
    public partial class RouterSync: MessageObject
    {
        public static RouterSync Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(RouterSync), isFromPool) as RouterSync; 
        }

        [MemoryPackOrder(0)]
        public uint ConnectId { get; set; }
        [MemoryPackOrder(1)]
        public string Address { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.ConnectId = default;
            this.Address = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_TestRequest)]
    [ResponseType(nameof(M2C_TestResponse))]
    public partial class C2M_TestRequest: MessageObject, ILocationRequest
    {
        public static C2M_TestRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_TestRequest), isFromPool) as C2M_TestRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string request { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.request = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_TestResponse)]
    public partial class M2C_TestResponse: MessageObject, IResponse
    {
        public static M2C_TestResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_TestResponse), isFromPool) as M2C_TestResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public string response { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.response = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2G_EnterMap)]
    [ResponseType(nameof(G2C_EnterMap))]
    public partial class C2G_EnterMap: MessageObject, ISessionRequest
    {
        public static C2G_EnterMap Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2G_EnterMap), isFromPool) as C2G_EnterMap; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_EnterMap)]
    public partial class G2C_EnterMap: MessageObject, ISessionResponse
    {
        public static G2C_EnterMap Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_EnterMap), isFromPool) as G2C_EnterMap; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        /// <summary>
        /// 自己的UnitId
        /// </summary>
        [MemoryPackOrder(3)]
        public long MyId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.MyId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.MoveInfo)]
    public partial class MoveInfo: MessageObject
    {
        public static MoveInfo Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(MoveInfo), isFromPool) as MoveInfo; 
        }

        [MemoryPackOrder(0)]
        public List<Unity.Mathematics.float3> Points { get; set; } = new List<Unity.Mathematics.float3>();
        [MemoryPackOrder(1)]
        public Unity.Mathematics.quaternion Rotation { get; set; }
        [MemoryPackOrder(2)]
        public int TurnSpeed { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Points.Clear();
            this.Rotation = default;
            this.TurnSpeed = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.UnitInfo)]
    public partial class UnitInfo: MessageObject
    {
        public static UnitInfo Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(UnitInfo), isFromPool) as UnitInfo; 
        }

        [MemoryPackOrder(0)]
        public long UnitId { get; set; }
        [MemoryPackOrder(1)]
        public int ConfigId { get; set; }
        [MemoryPackOrder(2)]
        public int Type { get; set; }
        [MemoryPackOrder(3)]
        public Unity.Mathematics.float3 Position { get; set; }
        [MemoryPackOrder(4)]
        public Unity.Mathematics.float3 Forward { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDictionaryOptions(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays)]
        [MemoryPackOrder(5)]
        public Dictionary<int, long> KV { get; set; } = new();
        [MemoryPackOrder(6)]
        public MoveInfo MoveInfo { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.UnitId = default;
            this.ConfigId = default;
            this.Type = default;
            this.Position = default;
            this.Forward = default;
            this.KV.Clear();
            this.MoveInfo = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_CreateUnits)]
    public partial class M2C_CreateUnits: MessageObject, IMessage
    {
        public static M2C_CreateUnits Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_CreateUnits), isFromPool) as M2C_CreateUnits; 
        }

        [MemoryPackOrder(0)]
        public List<UnitInfo> Units { get; set; } = new List<UnitInfo>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Units.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_CreateMyUnit)]
    public partial class M2C_CreateMyUnit: MessageObject, IMessage
    {
        public static M2C_CreateMyUnit Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_CreateMyUnit), isFromPool) as M2C_CreateMyUnit; 
        }

        [MemoryPackOrder(0)]
        public UnitInfo Unit { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Unit = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_StartSceneChange)]
    public partial class M2C_StartSceneChange: MessageObject, IMessage
    {
        public static M2C_StartSceneChange Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_StartSceneChange), isFromPool) as M2C_StartSceneChange; 
        }

        [MemoryPackOrder(0)]
        public long SceneInstanceId { get; set; }
        [MemoryPackOrder(1)]
        public string SceneName { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.SceneInstanceId = default;
            this.SceneName = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_RemoveUnits)]
    public partial class M2C_RemoveUnits: MessageObject, IMessage
    {
        public static M2C_RemoveUnits Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_RemoveUnits), isFromPool) as M2C_RemoveUnits; 
        }

        [MemoryPackOrder(0)]
        public List<long> Units { get; set; } = new List<long>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Units.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_PathfindingResult)]
    public partial class C2M_PathfindingResult: MessageObject, ILocationMessage
    {
        public static C2M_PathfindingResult Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_PathfindingResult), isFromPool) as C2M_PathfindingResult; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public Unity.Mathematics.float3 Position { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Position = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_Stop)]
    public partial class C2M_Stop: MessageObject, ILocationMessage
    {
        public static C2M_Stop Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_Stop), isFromPool) as C2M_Stop; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_PathfindingResult)]
    public partial class M2C_PathfindingResult: MessageObject, IMessage
    {
        public static M2C_PathfindingResult Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_PathfindingResult), isFromPool) as M2C_PathfindingResult; 
        }

        [MemoryPackOrder(0)]
        public long Id { get; set; }
        [MemoryPackOrder(1)]
        public Unity.Mathematics.float3 Position { get; set; }
        [MemoryPackOrder(2)]
        public List<Unity.Mathematics.float3> Points { get; set; } = new List<Unity.Mathematics.float3>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Id = default;
            this.Position = default;
            this.Points.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_Stop)]
    public partial class M2C_Stop: MessageObject, IMessage
    {
        public static M2C_Stop Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_Stop), isFromPool) as M2C_Stop; 
        }

        [MemoryPackOrder(0)]
        public int Error { get; set; }
        [MemoryPackOrder(1)]
        public long Id { get; set; }
        [MemoryPackOrder(2)]
        public Unity.Mathematics.float3 Position { get; set; }
        [MemoryPackOrder(3)]
        public Unity.Mathematics.quaternion Rotation { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Error = default;
            this.Id = default;
            this.Position = default;
            this.Rotation = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2G_Ping)]
    [ResponseType(nameof(G2C_Ping))]
    public partial class C2G_Ping: MessageObject, ISessionRequest
    {
        public static C2G_Ping Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2G_Ping), isFromPool) as C2G_Ping; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_Ping)]
    public partial class G2C_Ping: MessageObject, ISessionResponse
    {
        public static G2C_Ping Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_Ping), isFromPool) as G2C_Ping; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long Time { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Time = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_Test)]
    public partial class G2C_Test: MessageObject, ISessionMessage
    {
        public static G2C_Test Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_Test), isFromPool) as G2C_Test; 
        }

        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_Reload)]
    [ResponseType(nameof(M2C_Reload))]
    public partial class C2M_Reload: MessageObject, ISessionRequest
    {
        public static C2M_Reload Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_Reload), isFromPool) as C2M_Reload; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Account { get; set; }
        [MemoryPackOrder(2)]
        public string Password { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Account = default;
            this.Password = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_Reload)]
    public partial class M2C_Reload: MessageObject, ISessionResponse
    {
        public static M2C_Reload Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_Reload), isFromPool) as M2C_Reload; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2R_Login)]
    [ResponseType(nameof(R2C_Login))]
    public partial class C2R_Login: MessageObject, ISessionRequest
    {
        public static C2R_Login Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2R_Login), isFromPool) as C2R_Login; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        [MemoryPackOrder(1)]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MemoryPackOrder(2)]
        public string Password { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Account = default;
            this.Password = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.R2C_Login)]
    public partial class R2C_Login: MessageObject, ISessionResponse
    {
        public static R2C_Login Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(R2C_Login), isFromPool) as R2C_Login; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public string Address { get; set; }
        [MemoryPackOrder(4)]
        public long Key { get; set; }
        [MemoryPackOrder(5)]
        public long GateId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Address = default;
            this.Key = default;
            this.GateId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2G_LoginGate)]
    [ResponseType(nameof(G2C_LoginGate))]
    public partial class C2G_LoginGate: MessageObject, ISessionRequest
    {
        public static C2G_LoginGate Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2G_LoginGate), isFromPool) as C2G_LoginGate; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        [MemoryPackOrder(1)]
        public long Key { get; set; }
        [MemoryPackOrder(2)]
        public long GateId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Key = default;
            this.GateId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_LoginGate)]
    public partial class G2C_LoginGate: MessageObject, ISessionResponse
    {
        public static G2C_LoginGate Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_LoginGate), isFromPool) as G2C_LoginGate; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public long PlayerId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.PlayerId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_TestHotfixMessage)]
    public partial class G2C_TestHotfixMessage: MessageObject, ISessionMessage
    {
        public static G2C_TestHotfixMessage Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_TestHotfixMessage), isFromPool) as G2C_TestHotfixMessage; 
        }

        [MemoryPackOrder(0)]
        public string Info { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Info = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_TestRobotCase)]
    [ResponseType(nameof(M2C_TestRobotCase))]
    public partial class C2M_TestRobotCase: MessageObject, ILocationRequest
    {
        public static C2M_TestRobotCase Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_TestRobotCase), isFromPool) as C2M_TestRobotCase; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int N { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.N = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_TestRobotCase)]
    public partial class M2C_TestRobotCase: MessageObject, ILocationResponse
    {
        public static M2C_TestRobotCase Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_TestRobotCase), isFromPool) as M2C_TestRobotCase; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public int N { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.N = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_TestRobotCase2)]
    public partial class C2M_TestRobotCase2: MessageObject, ILocationMessage
    {
        public static C2M_TestRobotCase2 Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_TestRobotCase2), isFromPool) as C2M_TestRobotCase2; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int N { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.N = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_TestRobotCase2)]
    public partial class M2C_TestRobotCase2: MessageObject, ILocationMessage
    {
        public static M2C_TestRobotCase2 Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_TestRobotCase2), isFromPool) as M2C_TestRobotCase2; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int N { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.N = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2M_TransferMap)]
    [ResponseType(nameof(M2C_TransferMap))]
    public partial class C2M_TransferMap: MessageObject, ILocationRequest
    {
        public static C2M_TransferMap Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2M_TransferMap), isFromPool) as C2M_TransferMap; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.M2C_TransferMap)]
    public partial class M2C_TransferMap: MessageObject, ILocationResponse
    {
        public static M2C_TransferMap Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(M2C_TransferMap), isFromPool) as M2C_TransferMap; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.C2G_Benchmark)]
    [ResponseType(nameof(G2C_Benchmark))]
    public partial class C2G_Benchmark: MessageObject, ISessionRequest
    {
        public static C2G_Benchmark Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(C2G_Benchmark), isFromPool) as C2G_Benchmark; 
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

    // protofile : ET-Client/OuterMessage.proto
    [MemoryPackable]
    [Message(Message_ET_Client.G2C_Benchmark)]
    public partial class G2C_Benchmark: MessageObject, ISessionResponse
    {
        public static G2C_Benchmark Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(G2C_Benchmark), isFromPool) as G2C_Benchmark; 
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

    public static partial class Message_ET_Client
    {
         public const ushort Main2NetClient_Login = 10001;
         public const ushort NetClient2Main_Login = 10002;
         public const ushort C2G_Match = 10003;
         public const ushort G2C_Match = 10004;
         public const ushort Match2G_NotifyMatchSuccess = 10005;
         public const ushort C2Room_ChangeSceneFinish = 10006;
         public const ushort LockStepUnitInfo = 10007;
         public const ushort Room2C_Start = 10008;
         public const ushort FrameMessage = 10009;
         public const ushort OneFrameInputs = 10010;
         public const ushort Room2C_AdjustUpdateTime = 10011;
         public const ushort C2Room_CheckHash = 10012;
         public const ushort Room2C_CheckHashFail = 10013;
         public const ushort G2C_Reconnect = 10014;
         public const ushort HttpGetRouterResponse = 10015;
         public const ushort RouterSync = 10016;
         public const ushort C2M_TestRequest = 10017;
         public const ushort M2C_TestResponse = 10018;
         public const ushort C2G_EnterMap = 10019;
         public const ushort G2C_EnterMap = 10020;
         public const ushort MoveInfo = 10021;
         public const ushort UnitInfo = 10022;
         public const ushort M2C_CreateUnits = 10023;
         public const ushort M2C_CreateMyUnit = 10024;
         public const ushort M2C_StartSceneChange = 10025;
         public const ushort M2C_RemoveUnits = 10026;
         public const ushort C2M_PathfindingResult = 10027;
         public const ushort C2M_Stop = 10028;
         public const ushort M2C_PathfindingResult = 10029;
         public const ushort M2C_Stop = 10030;
         public const ushort C2G_Ping = 10031;
         public const ushort G2C_Ping = 10032;
         public const ushort G2C_Test = 10033;
         public const ushort C2M_Reload = 10034;
         public const ushort M2C_Reload = 10035;
         public const ushort C2R_Login = 10036;
         public const ushort R2C_Login = 10037;
         public const ushort C2G_LoginGate = 10038;
         public const ushort G2C_LoginGate = 10039;
         public const ushort G2C_TestHotfixMessage = 10040;
         public const ushort C2M_TestRobotCase = 10041;
         public const ushort M2C_TestRobotCase = 10042;
         public const ushort C2M_TestRobotCase2 = 10043;
         public const ushort M2C_TestRobotCase2 = 10044;
         public const ushort C2M_TransferMap = 10045;
         public const ushort M2C_TransferMap = 10046;
         public const ushort C2G_Benchmark = 10047;
         public const ushort G2C_Benchmark = 10048;
    }
}

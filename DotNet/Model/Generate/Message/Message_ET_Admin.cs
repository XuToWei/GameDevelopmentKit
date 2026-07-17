// This is an automatically generated class by Share.Tool. Please do not modify it.

using MemoryPack;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// Shared Proto Types //////
    /// </summary>
    // proto file : ET-Admin/AdminMessage.proto (line:7)
    [MemoryPackable]
    [Message(Message_ET_Admin.FiberInfoProto)]
    public partial class FiberInfoProto: MessageObject
    {
        public static FiberInfoProto Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(FiberInfoProto), isFromPool) as FiberInfoProto; 
        }

        [MemoryPackOrder(0)]
        public int Id { get; set; }
        [MemoryPackOrder(1)]
        public int Zone { get; set; }
        [MemoryPackOrder(2)]
        public string SceneType { get; set; }
        [MemoryPackOrder(3)]
        public string Name { get; set; }
        [MemoryPackOrder(4)]
        public string SchedulerType { get; set; }
        [MemoryPackOrder(5)]
        public int EntityCount { get; set; }
        [MemoryPackOrder(6)]
        public int ProcessId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Id = default;
            this.Zone = default;
            this.SceneType = default;
            this.Name = default;
            this.SchedulerType = default;
            this.EntityCount = default;
            this.ProcessId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:18)
    [MemoryPackable]
    [Message(Message_ET_Admin.SceneInfoProto)]
    public partial class SceneInfoProto: MessageObject
    {
        public static SceneInfoProto Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(SceneInfoProto), isFromPool) as SceneInfoProto; 
        }

        [MemoryPackOrder(0)]
        public int Id { get; set; }
        [MemoryPackOrder(1)]
        public string SceneType { get; set; }
        [MemoryPackOrder(2)]
        public string Name { get; set; }
        [MemoryPackOrder(3)]
        public int Zone { get; set; }
        [MemoryPackOrder(4)]
        public string InnerAddress { get; set; }
        [MemoryPackOrder(5)]
        public string OuterAddress { get; set; }
        [MemoryPackOrder(6)]
        public int PlayerCount { get; set; }
        [MemoryPackOrder(7)]
        public int FiberId { get; set; }
        [MemoryPackOrder(8)]
        public int ProcessId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.Id = default;
            this.SceneType = default;
            this.Name = default;
            this.Zone = default;
            this.InnerAddress = default;
            this.OuterAddress = default;
            this.PlayerCount = default;
            this.FiberId = default;
            this.ProcessId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    /// <summary>
    /// Admin Messages //////
    /// </summary>
    // proto file : ET-Admin/AdminMessage.proto (line:34)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_GetFibersRequest)]
    [ResponseType(nameof(Admin2S_GetFibersResponse))]
    public partial class Admin2S_GetFibersRequest: MessageObject, IRequest
    {
        public static Admin2S_GetFibersRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_GetFibersRequest), isFromPool) as Admin2S_GetFibersRequest; 
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

    // proto file : ET-Admin/AdminMessage.proto (line:39)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_GetFibersResponse)]
    public partial class Admin2S_GetFibersResponse: MessageObject, IResponse
    {
        public static Admin2S_GetFibersResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_GetFibersResponse), isFromPool) as Admin2S_GetFibersResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<FiberInfoProto> Fibers { get; set; } = new List<FiberInfoProto>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Fibers.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:48)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_GetScenesRequest)]
    [ResponseType(nameof(Admin2S_GetScenesResponse))]
    public partial class Admin2S_GetScenesRequest: MessageObject, IRequest
    {
        public static Admin2S_GetScenesRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_GetScenesRequest), isFromPool) as Admin2S_GetScenesRequest; 
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

    // proto file : ET-Admin/AdminMessage.proto (line:53)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_GetScenesResponse)]
    public partial class Admin2S_GetScenesResponse: MessageObject, IResponse
    {
        public static Admin2S_GetScenesResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_GetScenesResponse), isFromPool) as Admin2S_GetScenesResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public List<SceneInfoProto> Scenes { get; set; } = new List<SceneInfoProto>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Scenes.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // Server -> Admin: process status push (one-way)
    // proto file : ET-Admin/AdminMessage.proto (line:62)
    [MemoryPackable]
    [Message(Message_ET_Admin.S2Admin_ProcessStatusReport)]
    public partial class S2Admin_ProcessStatusReport: MessageObject, IMessage
    {
        public static S2Admin_ProcessStatusReport Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(S2Admin_ProcessStatusReport), isFromPool) as S2Admin_ProcessStatusReport; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int ProcessId { get; set; }
        [MemoryPackOrder(2)]
        public int Status { get; set; }
        [MemoryPackOrder(3)]
        public long MemoryUsage { get; set; }
        [MemoryPackOrder(4)]
        public int FiberCount { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.ProcessId = default;
            this.Status = default;
            this.MemoryUsage = default;
            this.FiberCount = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:72)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_KickPlayerRequest)]
    [ResponseType(nameof(Admin2S_KickPlayerResponse))]
    public partial class Admin2S_KickPlayerRequest: MessageObject, IRequest
    {
        public static Admin2S_KickPlayerRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_KickPlayerRequest), isFromPool) as Admin2S_KickPlayerRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public long PlayerId { get; set; }
        [MemoryPackOrder(2)]
        public string Reason { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.PlayerId = default;
            this.Reason = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:79)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_KickPlayerResponse)]
    public partial class Admin2S_KickPlayerResponse: MessageObject, IResponse
    {
        public static Admin2S_KickPlayerResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_KickPlayerResponse), isFromPool) as Admin2S_KickPlayerResponse; 
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

    // proto file : ET-Admin/AdminMessage.proto (line:87)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_ReloadRequest)]
    [ResponseType(nameof(Admin2S_ReloadResponse))]
    public partial class Admin2S_ReloadRequest: MessageObject, IRequest
    {
        public static Admin2S_ReloadRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_ReloadRequest), isFromPool) as Admin2S_ReloadRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        /// <summary>
        /// "code", "config", "all"
        /// </summary>
        [MemoryPackOrder(1)]
        public string Type { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:93)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2S_ReloadResponse)]
    public partial class Admin2S_ReloadResponse: MessageObject, IResponse
    {
        public static Admin2S_ReloadResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2S_ReloadResponse), isFromPool) as Admin2S_ReloadResponse; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int Error { get; set; }
        [MemoryPackOrder(2)]
        public string Message { get; set; }
        [MemoryPackOrder(3)]
        public bool Success { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Error = default;
            this.Message = default;
            this.Success = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    /// <summary>
    /// Agent Messages //////
    /// </summary>
    // proto file : ET-Admin/AdminMessage.proto (line:104)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_StartProcessRequest)]
    [ResponseType(nameof(Admin2Agent_StartProcessResponse))]
    public partial class Admin2Agent_StartProcessRequest: MessageObject, IRequest
    {
        public static Admin2Agent_StartProcessRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_StartProcessRequest), isFromPool) as Admin2Agent_StartProcessRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int ProcessId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.ProcessId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:110)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_StartProcessResponse)]
    public partial class Admin2Agent_StartProcessResponse: MessageObject, IResponse
    {
        public static Admin2Agent_StartProcessResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_StartProcessResponse), isFromPool) as Admin2Agent_StartProcessResponse; 
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

    // proto file : ET-Admin/AdminMessage.proto (line:118)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_StopProcessRequest)]
    [ResponseType(nameof(Admin2Agent_StopProcessResponse))]
    public partial class Admin2Agent_StopProcessRequest: MessageObject, IRequest
    {
        public static Admin2Agent_StopProcessRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_StopProcessRequest), isFromPool) as Admin2Agent_StopProcessRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int ProcessId { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.ProcessId = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:124)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_StopProcessResponse)]
    public partial class Admin2Agent_StopProcessResponse: MessageObject, IResponse
    {
        public static Admin2Agent_StopProcessResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_StopProcessResponse), isFromPool) as Admin2Agent_StopProcessResponse; 
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

    // proto file : ET-Admin/AdminMessage.proto (line:132)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_DeployFileRequest)]
    [ResponseType(nameof(Admin2Agent_DeployFileResponse))]
    public partial class Admin2Agent_DeployFileRequest: MessageObject, IRequest
    {
        public static Admin2Agent_DeployFileRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_DeployFileRequest), isFromPool) as Admin2Agent_DeployFileRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string FileName { get; set; }
        [MemoryPackOrder(2)]
        public byte[] FileData { get; set; }
        [MemoryPackOrder(3)]
        public string TargetPath { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.FileName = default;
            this.FileData = default;
            this.TargetPath = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:140)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_DeployFileResponse)]
    public partial class Admin2Agent_DeployFileResponse: MessageObject, IResponse
    {
        public static Admin2Agent_DeployFileResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_DeployFileResponse), isFromPool) as Admin2Agent_DeployFileResponse; 
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

    // proto file : ET-Admin/AdminMessage.proto (line:148)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_HotReloadRequest)]
    [ResponseType(nameof(Admin2Agent_HotReloadResponse))]
    public partial class Admin2Agent_HotReloadRequest: MessageObject, IRequest
    {
        public static Admin2Agent_HotReloadRequest Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_HotReloadRequest), isFromPool) as Admin2Agent_HotReloadRequest; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public string Type { get; set; }
        [MemoryPackOrder(2)]
        public List<int> ProcessIds { get; set; } = new List<int>();
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.Type = default;
            this.ProcessIds.Clear();
            ObjectPool.Instance.Recycle(this); 
        }
    }

    // proto file : ET-Admin/AdminMessage.proto (line:155)
    [MemoryPackable]
    [Message(Message_ET_Admin.Admin2Agent_HotReloadResponse)]
    public partial class Admin2Agent_HotReloadResponse: MessageObject, IResponse
    {
        public static Admin2Agent_HotReloadResponse Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Admin2Agent_HotReloadResponse), isFromPool) as Admin2Agent_HotReloadResponse; 
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

    // Agent -> Admin: status report (one-way)
    // proto file : ET-Admin/AdminMessage.proto (line:163)
    [MemoryPackable]
    [Message(Message_ET_Admin.Agent2Admin_StatusReport)]
    public partial class Agent2Admin_StatusReport: MessageObject, IMessage
    {
        public static Agent2Admin_StatusReport Create(bool isFromPool = false) 
        { 
            return ObjectPool.Instance.Fetch(typeof(Agent2Admin_StatusReport), isFromPool) as Agent2Admin_StatusReport; 
        }

        [MemoryPackOrder(0)]
        public int RpcId { get; set; }
        [MemoryPackOrder(1)]
        public int AgentProcessId { get; set; }
        [MemoryPackOrder(2)]
        public int ManagedProcessCount { get; set; }
        [MemoryPackOrder(3)]
        public long MemoryUsage { get; set; }
        public override void Dispose() 
        {
            if (!this.IsFromPool) { return; }
            this.RpcId = default;
            this.AgentProcessId = default;
            this.ManagedProcessCount = default;
            this.MemoryUsage = default;
            ObjectPool.Instance.Recycle(this); 
        }
    }

}

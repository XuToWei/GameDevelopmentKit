using System.Collections.Generic;

namespace ET.Server
{
    /// <summary>
    /// 房间目录：房间号到房间 Fiber ActorId 的映射。
    /// 这是本组件的全部状态，不需要额外的 Runtime 包一层——
    /// Runtime 的用途是隔离不可序列化、需要显式释放的 tick 内临时状态。
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public sealed class SurvivorRoomManagerComponent: Entity, IAwake
    {
        public Dictionary<string, ActorId> Rooms { get; set; }
    }
}

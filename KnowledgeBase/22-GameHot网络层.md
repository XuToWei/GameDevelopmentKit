# 模块 22：GameHot 网络层

## 概述
GameHot 模式的网络层位于 `Game/Hot/Loader/Network/`，基于 UGF `INetworkChannelHelper` + protobuf 实现：**对象池包 + protobuf 序列化 + CorrelationID 请求/响应配对 + Handler 事件分发**（RPC 风格）。与 ET 模式的 Fiber/消息驱动风格不同。

## 关键文件
| 文件 | 职责 |
| --- | --- |
| `NetworkServiceHelper.cs` | `INetworkServiceHelper` 实现：创建 WebSocket 频道、维护 `m_WaitedResponseDict`（CorrelationID→`AutoResetUniTaskCompletionSource`）实现请求-响应配对 |
| `NetworkExtension.cs` | `NetworkServiceComponent` 扩展方法：强类型 `SendAsync<T1,T2>`（T1:CSPacketBase → T2:SCPacketBase） |
| `CSPacketBase.cs` / `SCPacketBase.cs` | C2S/S2C 抽象包，含 `CorrelationID` 关联 ID（自增分配） |
| `PacketBase.cs` / `PacketHandlerBase.cs` | protobuf 包基类与 `IPacketHandler` 处理器基类（`Id` + `Handle`） |
| `NetworkChannelHelper.cs` | UGF `INetworkChannelHelper`：反射注册 S2C 包类型与 Handler、protobuf 序列化（`Serialize/DeserializePacket`）、心跳、断线管理 |
| `CSPacketHeader.cs` / `SCPacketHeader.cs` / `PacketHeaderBase.cs` | 包头（4 字节） |
| `Packet/CSHeartBeat.cs` / `PacketHandler/SCHeartBeatHandler.cs` | 心跳收发示例 |

## 收发链路
```
连接：ProcedureGame.OnEnter → GameEntry.NetworkService.InitServiceNetworkHelper(new NetworkServiceHelper())
       → Connect() → NetworkServiceHelper.OnInitialize 创建 "WebSocket" 频道

发送 C2S：NetworkServiceHelper.SendAsync<T> → csPacket.IncrementCorrelationID()
       → 字典登记 TCS → Send(packet) → NetworkChannelHelper.Serialize（protobuf + Fixed32）→ 频道发出

接收 S2C：NetworkChannelHelper.DeserializePacket（按 SCPacketHeader.Id 查反射表实例化）
       → 频道分派给对应 PacketHandler.Handle
       → 同时抛 OnHandelPacketEventArgs → NetworkServiceHelper.OnHandelPacket 按 CorrelationID 匹配 TCS，await 恢复
```

## 真实代码示例
```csharp
// 定义请求包（继承 CSPacketBase）+ 响应包（继承 SCPacketBase），自动生成 Id

// 发送并等待响应（强类型 RPC）
SCSomeResp resp = await GameEntry.NetworkService.SendAsync<CSSomeReq, SCSomeResp>(req);

// 自定义 Handler（处理服务端主动推送的 S2C）
public class SCNoticeHandler : PacketHandlerBase<SCNotice>
{
    protected override void Handle(SCNotice packet)
    {
        // 处理推送
    }
}
```

## 关键 API
```csharp
GameEntry.NetworkService.InitServiceNetworkHelper(helper); // 初始化
GameEntry.NetworkService.Connect();                         // 连接
GameEntry.NetworkService.SendAsync<TReq, TResp>(req);       // 请求-响应
```

## 与 ET 模式网络对比
| 维度 | GameHot（UGF） | ET |
| --- | --- | --- |
| 传输 | WebSocket（NetworkServiceHelper 默认） | KCP/TCP/WebSocket 多传输 |
| 消息模型 | CSPacket/SCPacket + CorrelationID 配对（RPC） | Message + MessageDispatcher（消息驱动） |
| 并发 | 单线程 + UniTask 回调 | Fiber 多纤程 |
| 序列化 | Protobuf-Unity | MemoryPack |

## 官方文档
- `Book/Proto生成工具.md`（UGF 消息 `GameHotMessage` 生成与 PacketHandler）
- 服务端消息处理对应 `Book/管理后台.md` 的 Actor 消息机制

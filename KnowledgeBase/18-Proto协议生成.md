# 模块 18：Proto 协议生成

## 概述
Proto 生成工具（`Share/Tool/Proto2CS/`）把 `Design/Proto/` 下的 `.proto` 文件生成两种客户端代码：**ET（MemoryPack）**与 **UGF（Protobuf）**，服务端共用 ET 生成物。Opcode 由工具按文件顺序自动递增分配，保证双端一致。

## 输入输出
```
Design/Proto/
├── ET-Client/       # Opcode 10000 段
├── ET-ClientServer/ # Opcode 20000 段
├── GameHot/         # Opcode 30000 段（UGF/Protobuf）
└── ET-Admin/        # 管理后台协议

输出：
├── ET:  Scripts/Game/ET/Code/Model/Generate/{Client|ClientServer}/Message/Message_ET_Client.cs + _Id.cs
└── UGF: Scripts/Game/Hot/Code/Generate/Message/Message_GameHot.cs + Id.cs + _PacketHandler.cs
```

## 核心逻辑（Proto2CS.cs）
- 扫描 `Design/Proto/` 直接子目录的 `proto.conf`，按路径排序文件、逐行解析递增分配 Opcode（首条 `startOpcode+1`，上限 60000）。
- `codeType=ET`（`Proto2CS.ET.cs`）：
  - 继承 `MessageObject`，加 `MemoryPackable/Message/ResponseType` 特性。
  - 生成对象池 `Create/Dispose`。
  - `// IRequest`、`// ResponseType` 尾注释生效（标记请求与响应类型）。
- `codeType=UGF`（`Proto2CS.UGF.cs`）：
  - `CS*` / `SC*` 消息分别继承 `CSPacketBase` / `SCPacketBase`。
  - 用 Protobuf-Unity 的 `ProtoContract/ProtoMember`。

## 使用方式
1. 在 `Design/Proto/` 对应目录编写/修改 `.proto`。
2. 运行导出（菜单 `Game/Excel Export` 或 `Proto` 相关工具栏按钮）。
3. 生成的 `Message_*.cs` 放入对应 Generate 目录（**自动生成，勿手改**）。

## 生成的代码示例（概念）
```csharp
// ET 消息（MemoryPack）
[MemoryPackable]
[Message(OuterMessage.C2S_Login)]
public partial class C2S_Login : MessageObject
{
    public string Account;
    public string Password;
}

// UGF 消息（Protobuf）
[ProtoContract]
public class CSLogin : CSPacketBase
{
    [ProtoMember(1)] public string Account;
    [ProtoMember(2)] public string Password;
}
```

## 设计要点
- Opcode 由工具分配而非手写，避免冲突；分段规划（10000/20000/30000）区分客户端类型。
- ET 与 UGF 双生成，双端协议模型共享（服务端读 ET 生成物，客户端按模式选用）。
- 与 MemoryPack Extension、Protobuf Unity 两个 UPM 包配合（见 README 依赖表）。

## 官方文档
- `Book/Proto生成工具.md`（协议代码生成专题）

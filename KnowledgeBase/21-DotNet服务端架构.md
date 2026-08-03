# 模块 21：DotNet 服务端架构

## 概述
ET 独立服务端位于 `DotNet/`，与客户端**共享同一套 ET 源码**（csproj 通过 `<Compile Include>` 直接编译 `Unity/Assets/Scripts/Library/ET` 源码，`DefineConstants=DOTNET` 区分），保证前后端协议、数据模型与逻辑一致。

## 目录结构
```
DotNet/
├── App/        进程入口（App.dll，Program.cs）
├── Loader/     启动装配层（Init/CodeLoader/ConfigReader/TimeNow/RecastFileReader）
├── Hotfix/     热更逻辑层（Server/Admin Blazor 后台、Agent 进程代理）
├── Model/      数据/组件层（Server/Agent；Generate/Message）
├── Core/       核心库（编译引用 Library/ET/Core）
└── ThirdParty/ 第三方（Unity.Mathematics）
```
`DotNet.sln` 含 6 个项目：App、Loader、Hotfix、Model、Core、ThirdParty。

## 启动链路
```
App/Program.cs → Entry.Init()（防裁剪 Model）
 → Loader/Init.cs：解析命令行 Options，注册 Logger/TimeInfo/FiberManager/ConfigComponent/CodeLoaderComponent
 → CodeLoader：AssemblyLoadContext 反射加载 ./Hotfix.dll
 → 反射调 Model ET.Entry.Start
 → EntryEvent2_InitServer：按 DTStartProcessConfig/DTStartSceneConfig 用 FiberManager 创建 NetInner + 各 Scene 纤程
```

## 关键模块
| 模块 | 说明 |
| --- | --- |
| **Admin 管理后台** | 独立 Fiber（AppType=Admin，SceneType.Admin）；`AdminComponent` 内置 ASP.NET Core Blazor Server + MudBlazor 后台，默认 5200 端口；通过 Actor 消息（`Admin2S_*`/`Admin2Agent_*`）管理服务器，LiteDB 存储 |
| **Agent 部署代理** | 本机部署代理；`FiberInit_Agent` 启动本机所有非 Agent/Admin 进程，心跳上报 Admin，处理热重载/启停/部署 |
| **动态扩容** | `Book/动态扩容.md` 为架构提案（**未实现**）：ServiceRegistry/租约/Route Snapshot，改造 ProcessOuterSender 静态查表 |
| **LockStep 帧同步** | Match/Room/Map/Gate 四 Fiber；`LSServerUpdater` 帧推进（配套客户端 Hotfix/LockStep） |

## 网络传输与消息处理（Core）
| 类 | 职责 |
| --- | --- |
| `AService`（抽象）/ `KService`/`KChannel` | KCP 传输（支持 UDP/TCP 双传输，IKcpTransport） |
| `TService`/`TChannel` | TCP 传输 |
| `WService` | WebSocket 传输 |
| `PacketParser` | 组包/拆包 |
| `MessageDispatcher` | 按 MessageHandler/LocationHandler 注册分发消息 |
| `NetComponent`/`NetComponentOnRead` | 网络组件与读消息 |
| `ProcessInnerSender` | 跨进程内网消息发送 |
| `RouterComponent`/`RouterNode` | 路由器：UDP/TCP 转发，客户端 RouterConnector 断线重连 |

## 关键文件速查
1. `DotNet/App/Program.cs` — 入口
2. `DotNet/Loader/CodeLoader.cs` — 热更程序集加载
3. `DotNet/Loader/Init.cs` — 单例装配
4. `Unity/.../Hotfix/Server/Demo/EntryEvent2_InitServer.cs` — Scene 创建
5. `DotNet/Hotfix/Server/Admin/AdminComponent.cs` — 管理后台宿主
6. `DotNet/Hotfix/Server/Agent/FiberInit_Agent.cs` — 进程代理
7. `Library/ET/Core/Runtime/Network/KService.cs` / `TService.cs` — 传输实现
8. `Library/ET/Core/Runtime/World/Module/Actor/MessageDispatcher.cs` — 消息分发
9. `Library/ET/Model/Server/Module/Router/RouterComponent.cs` — 路由器
10. `Config/Luban/dtstart*.bytes` — 进程/Scene 拓扑配置

## 官方文档
- `Book/管理后台.md`（Admin 后台使用）
- `Book/动态扩容.md`（扩容设计，未实现）
- `Book/快速开始.md`（独立服务端启动方式）

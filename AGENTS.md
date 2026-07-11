# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## Project Overview

GameDevelopmentKit is a dual-end (client/server) game development framework for Unity. It combines:
- **Server**: ET 8.1 Framework
- **Client**: UnityGameFramework (GF) with ET modules integrated
- **Hot Reload**: HybridCLR
- **Async**: UniTask (replaces ETTask)
- **Configuration**: Luban

## Build Commands

### Prerequisites
- .NET 8 SDK
- Unity 6000.3.18f1
- MongoDB (optional, for server features)
- Paid plugins: Odin Inspector, SRDebugger

### Running in Editor
1. Open Unity project, wait for compilation
2. Open `Kit.sln` in IDE and compile (needed for tools: config export, file server, code analysis)
3. Click "Launcher" button next to Unity Play button to run ET Demo

### Building
1. Run HybridCLR hot-reload preparation (see Book/HybridCLR热更.md)
2. Use one-click build tool (see Book/一键打包.md)

## Solution Files
- `Kit.sln` - Main solution for tools and utilities
- `DotNet/DotNet.sln` - Server-side .NET projects
- `Unity/Unity.sln` - Unity C# projects

## Architecture

### Directory Structure
```
DotNet/           # Server-side .NET code
├── App/          # Application entry points
├── Core/         # Core server logic
├── Hotfix/       # Hot-reloadable server code
├── Loader/       # Loading logic
├── Model/        # Data models
└── ThirdParty/   # Third-party libraries

Share/            # Shared tools
├── Analyzer/     # Code analyzers
├── FileServer/   # Resource file server
├── Libs/         # Shared libraries
└── Tool/         # Development tools

Tools/            # Build and development tools
├── Config/       # Configuration files
├── Luban/        # Luban config tool with extensions
└── Shell/        # Build scripts

Unity/Assets/Scripts/
├── Game/         # Game-specific code
│   ├── Base/     # Base classes
│   ├── Container/# Containers (e.g., EntityContainer, ResourceContainer)
│   ├── ET/       # ET framework integration (ETUI, ETEntity)
│   │   ├── Code/                    # ET框架业务代码
│   │   │   ├── Editor/              # 编辑器代码
│   │   │   ├── Hotfix/              # 热更逻辑代码
│   │   │   │   ├── Client/          # 客户端逻辑 (Demo, LockStep, Module, Plugins)
│   │   │   │   ├── Server/          # 服务器逻辑 (Benchmark, Demo, LockStep, Module)
│   │   │   │   └── Share/           # 客户端服务器共享代码
│   │   │   ├── HotfixView/          # 热更视图逻辑
│   │   │   ├── Model/               # 数据模型
│   │   │   └── ModelView/           # 视图模型
│   │   ├── Editor/                  # ET编辑器工具
│   │   └── Loader/                  # ET初始化和加载
│   │       ├── Helper/              # 工具类 (PathHelper, UGFHelper等)
│   │       ├── Mono/                # MonoBehaviour脚本
│   │       ├── Plugins/             # 扩展插件
│   │       └── UGF/                 # UnityGameFramework集成
│   │           ├── Entity/          # ETEntity - GF Entity与ET集成
│   │           ├── UIForm/          # ETUI - GF UI与ET集成
│   │           └── UIWidget/        # ET UIWidget组件
│   ├── Entity/   # GF Entity wrappers
│   ├── Generate/ # Generated code (Luban configs, proto)
│   ├── Hot/      # Hot-reloadable game code
│   │   ├── Code/                    # 热更游戏代码 (会被HybridCLR热更)
│   │   │   ├── Base/                # 入口点 (HotEntry.cs)
│   │   │   ├── Definition/          # 定义 (Constant, DataStruct, Enum)
│   │   │   ├── Editor/              # 编辑器代码
│   │   │   ├── Entity/              # 实体 (EntityData, EntityLogic)
│   │   │   ├── Game/                # 游戏模式 (GameBase, SurvivalGame)
│   │   │   ├── Generate/            # 自动生成代码 (勿手动修改)
│   │   │   │   ├── Luban/           # Luban配置表代码
│   │   │   │   ├── Message/         # Proto消息代码
│   │   │   │   └── UGF/             # UGF ID定义 (EntityId, UIFormId等)
│   │   │   ├── HPBar/               # 血条组件
│   │   │   ├── Procedure/           # 游戏流程 (Launch, Menu, Main, ChangeScene等)
│   │   │   ├── Scene/               # 场景相关
│   │   │   ├── Tables/              # Luban表加载
│   │   │   ├── UI/                  # UI界面 (MenuForm, SettingForm, DialogForm等)
│   │   │   └── Utility/             # 工具类
│   │   └── Loader/                  # 非热更初始化代码
│   │       ├── Base/                # HotComponent入口
│   │       ├── Editor/              # 编辑器工具 (Build, ToolBar)
│   │       ├── Entity/              # 基础Entity类
│   │       ├── Network/             # 网络模块 (Packet, PacketHandler)
│   │       └── UI/                  # 基础UI类
│   ├── HybridCLR/# HybridCLR integration
│   ├── Procedure/# Game procedures/states
│   └── UI/       # UI code
├── Library/      # Third-party and extension libraries
│   ├── ET/                      # ET框架核心代码
│   │   ├── Core/                # ET核心模块
│   │   └── ThirdParty/          # ET第三方依赖
│   ├── Extension/               # 扩展库
│   │   ├── StateController/     # 状态控制器
│   │   └── UniTask/             # UniTask异步库扩展
│   ├── FolderTag/               # 文件夹标签工具
│   ├── LubanLib/                # Luban运行时库
│   │   ├── Editor/              # Luban编辑器工具
│   │   └── Runtime/             # Luban运行时代码
│   ├── ReplaceComponent/        # 组件替换工具
│   ├── SocoTool/                # Soco工具集
│   ├── StompyRobot/             # SRDebugger相关
│   ├── UGF/                     # UnityGameFramework
│   │   ├── GameFramework/       # GF核心框架
│   │   ├── GameFramework.prefab # GF预制体
│   │   ├── UnityGameFramework/  # UGF运行时
│   │   └── UnityGameFramework.Extension/  # UGF扩展
│   │       └── Runtime/
│   │           ├── Awaitable/       # 异步等待扩展
│   │           ├── AssetCollection/ # 资源集合
│   │           ├── CodeRunner/      # 代码运行器
│   │           ├── NetworkService/  # 网络服务
│   │           ├── SpriteCollection/# 精灵集合
│   │           ├── TextureSet/      # 纹理集
│   │           ├── Timer/           # 定时器
│   │           ├── TimingWheel/     # 时间轮
│   │           └── WebSocket/       # WebSocket支持
│   └── UXTool/                  # UX工具集
└── Other/        # Miscellaneous code
```

**ET关键文件**: `ET/Loader/Init.cs`, `ET/Loader/CodeLoader.cs`, `ET/Loader/UGF/UGFEntitySystemSingleton.cs`

**Hot关键文件**: `Hot/Code/Base/HotEntry.cs`, `Hot/Code/Procedure/ProcedureComponent.cs`, `Hot/Loader/Init.cs`

**Hot模块注意**: `Code/` 目录会被HybridCLR热更新，`Loader/` 目录不会热更

### Key Patterns

**Module Switching**: The framework supports flexible switching between ET logic and GF logic, and between hot-reload and non-hot-reload modes. This affects link.xml optimization for build size.

**Code Binding**: Uses custom CodeBind tool for resource-to-code mapping. Recommended for UI development.

**State Controller**: Add macro `STATE_CONTROLLER_CODE_BIND` to auto-generate state data code during code binding.

**ETUI**: GF-based UI system integrated with ET (see `Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm`)

**ETEntity**: GF-based Entity system integrated with ET (see `Unity/Assets/Scripts/Game/ET/Loader/UGF/Entity`)

## UI创建流程

### 创建新UI的步骤

#### 第一步：创建预制体

在`Unity/Assets/Res/UI/UIForm/Hot/`目录下创建预制体（如TestForm.prefab）

#### 第二步：添加UI配置到Luban

1. 编辑Excel文件：`Design/Excel/GameHot/Datas/Game/UI.xlsx`
2. 在UIForm表中添加新行：
   - `Id`: 唯一ID (如 103)
   - `CSName`: C#类名 (如 TestForm)
   - `Desc`: 描述
   - `AssetName`: 资源路径 (如 Hot/TestForm)
   - `UIGroupName`: UI分组 (Default/Pop)
   - `AllowMultiInstance`: 是否允许多实例
   - `PauseCoveredUIForm`: 是否暂停被覆盖的UI
3. 运行Luban导出工具，自动生成`UIFormId.cs`

#### 第三步：创建UI代码（二选一）

**GameHot模式**

创建UI逻辑类（`Game/Hot/Code/UI/`）：
```csharp
public class TestForm : StarForceUIForm
{
    [SerializeField]
    private Button m_TestButton = null;

    public void OnTestButtonClick()
    {
        // 按钮点击逻辑
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
    }
}
```

将脚本挂载到预制体，打开UI：
```csharp
GameEntry.UI.OpenUIForm(UIFormId.TestForm);
```

**ET框架模式（ETUI）**

创建Model层（`ET/Code/ModelView/Client/`）：
```csharp
[ComponentOf(typeof(UIComponent))]
public class UIFormTestComponent : UGFUIForm<MonoUIFormTest>,
    IAwake, IUGFUIFormOnOpen, IUGFUIFormOnClose
{
}
```

创建Hotfix层（`ET/Code/HotfixView/Client/`）：
```csharp
[EntitySystemOf(typeof(UIFormTestComponent))]
public static partial class UIFormTestComponentSystem
{
    [UGFUIFormSystem]
    private static void UGFUIFormOnOpen(this UIFormTestComponent self)
    {
        // 打开逻辑
    }
}
```

### UI生命周期方法

- `OnInit`: 初始化（仅首次）
- `OnOpen`: 每次打开时调用
- `OnUpdate`: 每帧更新
- `OnPause/OnResume`: 被覆盖/恢复时
- `OnClose`: 关闭时调用
- `OnRecycle`: 回收到对象池

### UI关键文件位置

| 功能 | 路径 |
|------|------|
| UIFormId定义 | `Game/Hot/Code/Generate/UGF/UIFormId.cs` |
| Luban配置 | `Res/Editor/Luban/dtuiform.json` |
| UI预制体 | `Res/UI/UIForm/Hot/*.prefab` |
| UI代码 | `Game/Hot/Code/UI/*.cs` |
| ET UI框架 | `Game/ET/Loader/UGF/UIForm/` |

## Entity创建流程

### 创建新Entity的步骤

#### 第一步：创建预制体

在`Unity/Assets/Res/Entity/`目录下创建预制体（如NewEntity.prefab）

#### 第二步：添加Entity配置到Luban

1. 编辑Excel文件：`Design/Excel/GameHot/Datas/Game/Entity.xlsx`
2. 在Entity表中添加新行：
   - `Id`: 唯一ID (如 10000)
   - `CSName`: C#常量名 (如 NewEntity)
   - `Desc`: 描述
   - `AssetName`: 预制体名称
   - `EntityGroupName`: Entity分组
   - `Priority`: 优先级
3. 运行Luban导出工具，自动生成`EntityId.cs`

#### 第三步：创建Entity代码（二选一）

**GameHot模式**

创建EntityLogic（`Game/Hot/Code/Entity/EntityLogic/`）：
```csharp
public class NewEntity : Entity
{
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
    }

    protected override void OnHide(bool isShutdown, object userData)
    {
        base.OnHide(isShutdown, userData);
    }
}
```

显示Entity：
```csharp
GameEntry.Entity.ShowEntity<NewEntity>(EntityId.NewEntity);
```

**ET框架模式（ETEntity）**

创建Model层（`ET/Code/ModelView/Client/`）：
```csharp
[ComponentOf(typeof(UGFEntityComponent))]
public class UGFEntityTestComponent : UGFEntity<MonoUGFEntityTest>,
    IAwake, IUGFEntityOnInit, IUGFEntityOnShow, IUGFEntityOnHide
{
}
```

创建Hotfix层（`ET/Code/HotfixView/Client/`）：
```csharp
[EntitySystemOf(typeof(UGFEntityTestComponent))]
public static partial class UGFEntityTestComponentSystem
{
    [UGFEntitySystem]
    private static void UGFEntityOnShow(this UGFEntityTestComponent self)
    {
        // 显示逻辑
    }
}
```

### Entity生命周期方法

- `OnInit`: 首次创建时初始化
- `OnShow`: 显示时调用，接收EntityData
- `OnUpdate`: 每帧更新
- `OnHide`: 隐藏时调用
- `OnRecycle`: 回收到对象池

### Entity关键文件位置

| 功能 | 路径 |
|------|------|
| EntityId定义 | `Game/Hot/Code/Generate/UGF/EntityId.cs` |
| Luban配置 | `Res/Editor/Luban/dtentity.json` |
| Entity预制体 | `Res/Entity/*.prefab` |
| EntityData | `Game/Hot/Code/Entity/EntityData/*.cs` |
| EntityLogic | `Game/Hot/Code/Entity/EntityLogic/*.cs` |
| ET Entity框架 | `Game/ET/Loader/UGF/Entity/` |

## Tools

- **Luban Config Export**: Multi-threaded config export tool (Book/Luban配置.md)
- **Proto Generator**: Generates ET and GF format proto code (Book/Proto生成工具.md)
- **ET Code Generator**: Generates ETUI and GFEntity boilerplate (Book/ET代码生成工具.md)
- **Custom Toolbar**: Unity Editor toolbar extensions (Book/自定义Toolbar.md)
- **Localization**: Auto-generated multi-language configs from Luban export (Book/多语言.md)

## Documentation

Detailed documentation is in the `Book/` directory (in Chinese):
- HybridCLR热更.md - Hot reload workflow
- Luban配置.md - Configuration export
- Proto生成工具.md - Protocol buffer generation
- ET代码生成工具.md - Code generation
- 一键打包.md - One-click build
- 多语言.md - Localization
- 自定义Toolbar.md - Custom toolbar

## QQ Group
949482664

<!-- BEGIN UNITY_AGENT_BRIDGE -->
## Unity Agent Bridge(驱动 Unity 编辑器)——必读,严格遵守

本工程接入了 Unity Agent Bridge:**任何需要让 Unity 编辑器做的事**(查场景、改物体、建资源、跑编译……),你(AI)都必须通过"写请求文件 / 读响应文件"来完成,不能凭空假设 Unity 状态,也不能跳过这套流程直接改工程文件。完整协议见 Unity 包内 `AGENT.md`。

`<root>` 默认 `<工程>/.agentbridge/`,下有 `requests/`(你写)、`responses/`(你读)。

### 第 0 步(每个 session 开头,只做一次)
在发任何其它命令**之前**,先发一次 `list_commands`,把返回的命令清单连同 `commandsVersion`、每条的 `paramsSchema` **记在本 session 里**。可用命令不在文档里、也不写死,只能这样运行时发现。**没做过第 0 步就发别的命令 = 错误。** 之后一直用这份缓存,不要重复调 `list_commands`(何时才需重调见文末)。

### 每让 Unity 做一件事,严格按此 5 步(不可跳步、不可合并、不可并发)
1. **取 schema**:从缓存里找到该命令,按它的 `paramsSchema` 拼 `params`(命令不存在 → 先做"重新发现",别猜)。
2. **起唯一 id**:为这条请求生成一个**全新、从未用过**的 `id`(哪怕是重试,也换新 id)。
3. **原子写**:先写 `<root>/requests/{id}.request.json.tmp`,**再改名**成 `<root>/requests/{id}.request.json`。**绝不能直接写最终名**(会被读到半截)。
   - Windows:`Move-Item -Force {id}.request.json.tmp {id}.request.json`
   - macOS/Linux:`mv {id}.request.json.tmp {id}.request.json`
4. **等这一条的响应**:反复读 `<root>/responses/{id}.response.json`,直到该文件出现(约每 1 秒一次,最多等 ~30 秒;超时按失败处理)。**在读到它之前,绝不发下一条命令**——`responses/` 只保留最新一条,抢发会把你要的那条冲掉。
5. **按 id 核对并处理**:确认响应 `id` 与你发的一致;`status=="ok"` 用 `result`,`status=="error"` 看 `error.code`(如 `INVALID_PARAMS` 改参数、`INTERRUPTED` 换新 id 重发)。顺便对照响应里的 `commandsVersion`(见文末)。

### 完整示例:让 Unity 执行 ping
- 生成 id:`req-8f3a`
- 写 `<root>/requests/req-8f3a.request.json.tmp`,内容:`{"v":1,"id":"req-8f3a","command":"ping","params":{}}`
- 改名为 `<root>/requests/req-8f3a.request.json`
- 轮询读 `<root>/responses/req-8f3a.response.json`,直到出现
- 得到 `{"id":"req-8f3a","status":"ok","result":{"message":"pong",...},"commandsVersion":"..."}` → 成功

### 绝不(违反任一条都会出错)
- 绝不直接写 `.request.json`(必须先 `.tmp` 再改名)。
- 绝不复用 `id`。
- 绝不在上一条响应读到之前,发下一条命令。
- 绝不跳过第 0 步、凭记忆或猜测发命令。

### 何时才重新 `list_commands`(平时都不需要)
仅当以下之一发生:任意响应的 `commandsVersion` 与你缓存的不一致 / 在 Unity 里装卸或启停了扩展 / 某命令返回 `UNKNOWN_COMMAND`。这时重发一次 `list_commands` 刷新缓存,再继续。

（可选)Unity 失焦时默认不轮询;若需失焦也驱动,在 Unity 开 `Window/Agent Bridge` 勾顶部「失焦不节流」。
<!-- END UNITY_AGENT_BRIDGE -->




# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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
- Unity 6000.0.59f2
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

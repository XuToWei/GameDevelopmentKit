# GameDevelopmentKit

GameDevelopmentKit（GDK）是一套 [Unity] 游戏开发框架。服务端基于 [ET 8.1]，客户端以 [UnityGameFramework]（GF）为底座，可选择纯 GF（GameHot）或 ET 开发模式。

## 核心能力

| 领域 | 能力 |
| --- | --- |
| 成熟稳定 | 经商业项目验证，覆盖客户端、服务端、热更新、数据、网络、UI 与构建等完整开发链路 |
| 双端架构 | [Unity] 客户端与 [ET 8.1] 服务端共享协议、配置和基础设施；客户端支持 [纯 GF（GameHot）][模式选择] 与 ET 模式 |
| 热更新 | [HybridCLR] 管理热更程序集、AOT 元数据与构建流程 |
| ET 与 GF 集成 | [ETUI]、[ETEntity] 接入 ET 生命周期，[UniTask] 统一异步模型 |
| 数据与协议 | [Luban] 导出配置，[Proto2CS] 生成 ET/MemoryPack 与 GF/Protobuf 协议代码 |
| 数据绑定 | [ReactiveBinding]、[CodeBind] 与 [StateController] 覆盖响应式数据、组件绑定和 UI 状态 |
| UI 与资源 | [UXTool] 提供 UI 工具，[AssetSet] 管理图片资源，[ResourceOptimize] 优化资源冗余 |
| 网络 | [UnityWebSocket] 提供 WebSocket 通道 |
| 编辑器工具 | [代码生成]、[包更新]、[Toolbar] 与 [一键构建] |

## 运行模式

| 模式 | 编译符号 | 适用场景 |
| --- | --- | --- |
| 纯 GF（GameHot） | `UNITY_GAMEHOT`（必选） | 使用 GF 客户端并加载 GameHot 业务程序集 |
| ET | `UNITY_ET` | ET 实体系统、客户端与服务端共享业务模型 |
| HybridCLR | 叠加 `UNITY_HOTFIX` | 将当前业务模块改为 DLL 资源加载 |

`UNITY_ET` 与 `UNITY_GAMEHOT` 互斥，`UNITY_HOTFIX` 可叠加。编辑器切换模式时会同步更新 Luban 工程、资源收集规则、`link.xml` 和 HybridCLR 程序集列表。

## 快速开始

1. 安装 [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) 和 [Unity 6000.3.18f1](https://unity.com/releases/editor/whats-new/6000.3.18f1)。

2. 在仓库根目录编译工具项目：

   ```powershell
   dotnet build Kit.sln
   ```

   也可通过 [Rider](https://www.jetbrains.com/rider/download/) 编译 `Kit.sln`，或在 Unity 中选择 `Game > Build Tool Editor`。

3. 用 Unity 打开 `Unity/`，加载 `Assets/Launcher.unity`，点击 Play。

模式切换和独立服务端启动方式见 [完整快速开始](Book/快速开始.md)。

## 文档导航

| 主题 | 文档 |
| --- | --- |
| 索引与架构 | [Book 文档索引](Book/README.md)、[项目结构与模式选择](Book/Project结构.md) |
| 业务开发 | [UI 开发](Book/UI开发.md)、[Entity 开发](Book/Entity开发.md) |
| 资源与数据 | [AssetSet](Book/AssetSet.md)、[Luban 配置](Book/Luban配置.md) |
| 协议 | [Proto 生成](Book/Proto生成工具.md) |
| 热更新与构建 | [HybridCLR 热更新](Book/HybridCLR热更.md)、[一键打包](Book/一键打包.md) |

## 主要依赖

| 分类 | 依赖 |
| --- | --- |
| 核心框架 | [UnityGameFramework]、[UGFExtensions]、[ET 8.1] |
| 热更新与配置 | [HybridCLR]、[Luban]、[Luban Extension] |
| 异步、序列化与网络 | [UniTask]、[MemoryPack Extension]、[Protobuf Unity]、[UnityWebSocket] |
| UI 与绑定 | [UXTool]、[CodeBind]、[StateController]、[ReactiveBinding]、[LoopScrollRect] |
| 编辑器工具 | [SocoTools]、[FolderTag] |

[Unity]: https://unity.com/
[UnityGameFramework]: https://github.com/EllanJiang/UnityGameFramework
[UGFExtensions]: https://github.com/FingerCaster/UGFExtensions
[ET 8.1]: https://github.com/egametang/ET/commit/b7bdaa0dcd5c682d968ec8922eb7a6dc4637011c
[HybridCLR]: https://github.com/focus-creative-games/hybridclr
[Luban]: https://github.com/focus-creative-games/luban
[Luban Extension]: https://github.com/XuToWei/Luban-Extension
[UniTask]: https://github.com/Cysharp/UniTask
[MemoryPack Extension]: https://github.com/XuToWei/MemoryPack-Extension
[Protobuf Unity]: https://github.com/XuToWei/Protobuf-Unity
[UnityWebSocket]: https://github.com/psygames/UnityWebSocket
[CodeBind]: https://github.com/XuToWei/CodeBind
[StateController]: https://github.com/XuToWei/StateController
[ReactiveBinding]: https://github.com/XuToWei/ReactiveBinding
[LoopScrollRect]: https://github.com/qiankanglai/LoopScrollRect
[UXTool]: https://uxtool.netease.com/
[SocoTools]: https://github.com/crossous/SocoTools
[FolderTag]: https://github.com/liyingsong99/FolderTag
[模式选择]: Book/Project结构.md
[ETUI]: Book/UI开发.md
[ETEntity]: Book/Entity开发.md
[ResourceOptimize]: Unity/Assets/Scripts/Library/UGF/UnityGameFramework.Extension/Editor/Resource/ResourceOptimize.cs
[Proto2CS]: Book/Proto生成工具.md
[AssetSet]: Book/AssetSet.md
[代码生成]: Book/ET代码生成工具.md
[包更新]: Unity/Assets/Scripts/Game/Editor/Tool/PackageUpdateTool.cs
[Toolbar]: Book/自定义Toolbar.md
[一键构建]: Book/一键打包.md

## 商业依赖、交流与许可

- 商业插件：[Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)，需自行购买并遵守授权条款。
- QQ 群：`949482664`
- 项目代码采用 [MIT License](LICENSE)；第三方资源和插件遵循各自许可。

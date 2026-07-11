# GameDevelopmentKit

> 一套仓库贯通 Unity 客户端、ET 服务端、热更新、配置与发布，加速游戏从原型走向长期迭代。

GameDevelopmentKit（GDK）以 ET 8.1 承载服务端，以 UnityGameFramework（GF）承载客户端，并将 ET 客户端能力模块化接入 GF。项目同时提供 GameHot 业务层，客户端可选择纯 GF（GameHot）或 ET 两种开发路径。

## 核心特色

| 能力 | 说明 |
| --- | --- |
| 双端统一 | Unity 客户端与 ET 服务端共享协议、配置和基础设施 |
| 模块可选 | 纯 GF（GameHot）、ET 两种客户端路径按需切换 |
| 热更新 | HybridCLR 管理热更新程序集、AOT 补充元数据和构建流程 |
| ET × GF | ETUI、ETEntity 将 ET 实体生命周期接入 GF UI 与 Entity 系统 |
| 统一异步 | 全项目使用 UniTask，替代 ETTask 并覆盖 GF 场景 |
| 数据驱动 | Luban 多工程并行导表，并生成 UI、Entity、Scene、Sound 常量 |
| 协议生成 | 同一套 Proto 工具可生成 ET（MemoryPack）或 UGF（protobuf-net）代码 |
| 编辑器工具 | CodeBind、状态控制器、响应式绑定、代码生成、Toolbar 与一键构建 |

## 运行模式

| 模式 | 编译符号 | 适用场景 |
| --- | --- | --- |
| 纯 GF（GameHot） | `UNITY_GAMEHOT` 可选 | 使用 GF 客户端；启用符号后加载 GameHot 业务程序集 |
| ET | `UNITY_ET` | ET 实体系统、客户端与服务端共享业务模型 |
| HybridCLR | 叠加 `UNITY_HOTFIX` | 将当前业务模块改为 DLL 资源加载 |

`UNITY_ET` 与 `UNITY_GAMEHOT` 由编辑器菜单互斥管理。切换模式时会同步调整 Luban 工程、资源收集规则、`link.xml` 和 HybridCLR 程序集列表。

## 快速开始

环境要求：.NET 8 SDK、Unity `6000.3.18f1`。ET 服务端功能按需安装 MongoDB。

```powershell
git submodule update --init --recursive
dotnet build Kit.sln
```

用 Unity 打开 `Unity/`，等待编译完成。选择 `Game/Define Symbol/Add UNITY_ET`，在 `ET/Build Tool` 中使用 `ClientServer`，然后点击播放按钮旁的 `Launcher` 即可运行 ET Demo。

完整步骤、GameHot 与独立服务端运行方式见 [快速开始](Book/快速开始.md)。

## 文档

- [Book 文档索引](Book/README.md)：按上手、开发、工具链、构建和设计分类。
- [项目结构与模式选择](Book/Project结构.md)：理解纯 GF（GameHot）、ET 与热更新边界。
- [UI 开发](Book/UI开发.md) / [Entity 开发](Book/Entity开发.md)：两套业务模式的完整创建流程。
- [Luban 配置](Book/Luban配置.md) / [Proto 生成](Book/Proto生成工具.md)：数据与协议生成链路。
- [HybridCLR 热更新](Book/HybridCLR热更.md) / [一键打包](Book/一键打包.md)：从 DLL 到资源和安装包。

## 主要依赖

| 分类 | 依赖 |
| --- | --- |
| 客户端与服务端 | [UnityGameFramework]、[UGFExtensions]、[ET 8.1] |
| 热更新与配置 | [HybridCLR]、[Luban]、[Luban Extension] |
| 异步与协议 | [UniTask]、[MemoryPack Extension]、[Protobuf Unity]、[protobuf-net] |
| 代码与界面工作流 | [CodeBind]、[StateController]、[ReactiveBinding]、[LoopScrollRect] |
| 编辑器与资源优化 | [SocoTools]、[FolderTag] |

[UnityGameFramework]: https://github.com/EllanJiang/UnityGameFramework
[UGFExtensions]: https://github.com/FingerCaster/UGFExtensions
[ET 8.1]: https://github.com/egametang/ET/commit/faa825d22a5b05d727f4878dfe34600628942579
[HybridCLR]: https://github.com/focus-creative-games/hybridclr
[Luban]: https://github.com/focus-creative-games/luban
[Luban Extension]: https://github.com/XuToWei/Luban-Extension
[UniTask]: https://github.com/Cysharp/UniTask
[MemoryPack Extension]: https://github.com/XuToWei/MemoryPack-Extension
[Protobuf Unity]: https://github.com/XuToWei/Protobuf-Unity
[protobuf-net]: https://github.com/protobuf-net/protobuf-net
[CodeBind]: https://github.com/XuToWei/CodeBind
[StateController]: https://github.com/XuToWei/StateController
[ReactiveBinding]: https://github.com/XuToWei/ReactiveBinding
[LoopScrollRect]: https://github.com/qiankanglai/LoopScrollRect
[SocoTools]: https://github.com/crossous/SocoTools
[FolderTag]: https://github.com/liyingsong99/FolderTag

## 商业插件

项目依赖 [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)，请自行购买并遵守其授权条款。

## 交流与许可

QQ 群：`949482664`

代码按 [MIT License](LICENSE) 开源；第三方资源与商业插件遵循各自许可。

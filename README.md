# GameDevelopmentKit

> 一套仓库贯通 Unity 客户端、ET 服务端、热更新、配置与发布，加速游戏从原型走向长期迭代。

GameDevelopmentKit（GDK）以 ET 8.1 承载服务端，以 UnityGameFramework（GF）承载客户端，并将 ET 客户端能力模块化接入 GF。项目同时提供 GameHot 业务层，允许团队按项目需求选择纯 GF、GameHot 或 ET 开发路径。

GDK 的重点不是简单集合第三方库，而是打通客户端、服务端、配置、协议、热更新、资源发布与日常开发工具，让团队把时间集中在玩法和内容上。

## 为什么选择 GDK

| 优势 | 带来的价值 |
| --- | --- |
| 一套工程贯通双端 | Unity 与 ET 共用协议、配置和基础设施，减少重复定义与联调成本 |
| 复杂度按需启用 | 可从纯 GF、GameHot 到 ET 逐级选择，不必为轻量项目强行引入完整服务端体系 |
| 热更新形成闭环 | HybridCLR、AOT 元数据、资源更新、版本检查和构建工具已经串联 |
| ET 与 GF 深度融合 | ETUI、ETEntity、UIWidget 将 ET 生命周期接入成熟的 GF UI 与 Entity 管理 |
| 数据与协议自动生成 | Luban、Proto、UI/Entity ID 和 ET 模板生成降低手写样板代码与编号错误 |
| 规范进入工具链 | UniTask、资源所有权容器和 Roslyn Analyzer 把异步、释放与 ET 规则前置到开发阶段 |

## 已落地的实用细节

- **模式切换不只改宏**：菜单会同步 Luban 工程、资源收集规则、`link.xml` 与 HybridCLR 程序集列表。
- **运营更新链路完整**：包含应用版本检查、版本列表更新、资源校验、差异资源下载、移动网络确认、进度与失败重试。
- **UI 生产能力丰富**：支持 CodeBind、状态控制器、响应式绑定、循环列表、红点、新手引导、安全区适配和多语言。
- **异步调用更统一**：资源、场景、UI、Entity、下载和网络请求均提供 UniTask/Awaitable 封装，并支持取消与集中释放。
- **联网玩法有现成参考**：ET 示例覆盖登录、网关、地图、AOI、机器人和压测；LockStep 覆盖匹配、回滚、校验、重连与录像。
- **开发发布更省步骤**：配置与协议生成、代码模板、资源清理、Toolbar、HybridCLR 准备、一键构建和本地资源服务器均有工具入口。

## 商业化应用方向

| 项目方向 | 可复用的框架基础 |
| --- | --- |
| 原型、独立游戏、休闲项目 | 纯 GF 或 GameHot、数据表、UI/Entity、流程管理和资源构建 |
| 长期运营的移动端或 PC 游戏 | 代码热更新、资源更新、版本流程、多语言和多屏适配 |
| RPG、SLG、多人在线项目 | ET 双端架构、Actor 消息、网关与地图服务、AOI、机器人和压测示例 |
| 房间制竞技或动作玩法 | 匹配、帧同步、回滚、Hash 校验、断线重连与录像回放示例 |
| 多项目或多人团队 | 模式化目录、自动生成、静态分析和统一构建流程 |

GDK 提供的是可扩展的商业项目工程底座。渠道登录、支付、广告、数据分析、崩溃上报、CDN、灰度发布、安全与合规仍需按产品和发行地区接入。

## 运行模式

| 模式 | 编译符号 | 适用场景 |
| --- | --- | --- |
| 纯 GF | 不启用 `UNITY_ET`、`UNITY_GAMEHOT` | 只使用 GF 客户端能力 |
| GameHot | `UNITY_GAMEHOT` | MonoBehaviour 风格的可热更业务代码 |
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
- [项目结构与模式选择](Book/Project结构.md)：理解 GF、GameHot、ET 与热更新边界。
- [UI 开发](Book/UI开发.md) / [Entity 开发](Book/Entity开发.md)：两套业务模式的完整创建流程。
- [Luban 配置](Book/Luban配置.md) / [Proto 生成](Book/Proto生成工具.md)：数据与协议生成链路。
- [HybridCLR 热更新](Book/HybridCLR热更.md) / [一键打包](Book/一键打包.md)：从 DLL 到资源和安装包。

## 主要依赖

| 分类 | 依赖 |
| --- | --- |
| 客户端与服务端 | [UnityGameFramework](https://github.com/EllanJiang/UnityGameFramework)、[UGFExtensions](https://github.com/FingerCaster/UGFExtensions)、[ET 8.1](https://github.com/egametang/ET/commit/faa825d22a5b05d727f4878dfe34600628942579) |
| 热更新与配置 | [HybridCLR](https://github.com/focus-creative-games/hybridclr)、[Luban](https://github.com/focus-creative-games/luban)、[Luban Extension](https://github.com/XuToWei/Luban-Extension) |
| 异步与协议 | [UniTask](https://github.com/Cysharp/UniTask)、[MemoryPack Extension](https://github.com/XuToWei/MemoryPack-Extension)、[Protobuf Unity](https://github.com/XuToWei/Protobuf-Unity)、[protobuf-net](https://github.com/protobuf-net/protobuf-net) |
| 代码与界面工作流 | [CodeBind](https://github.com/XuToWei/CodeBind)、[StateController](https://github.com/XuToWei/StateController)、[ReactiveBinding](https://github.com/XuToWei/ReactiveBinding)、[LoopScrollRect](https://github.com/qiankanglai/LoopScrollRect) |
| 编辑器与资源优化 | [SocoTools](https://github.com/crossous/SocoTools)、[FolderTag](https://github.com/liyingsong99/FolderTag) |

## 商业插件

项目依赖 [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)，请自行购买并遵守其授权条款。

## 交流与许可

QQ 群：`949482664`

代码按 [MIT License](LICENSE) 开源；第三方资源与商业插件遵循各自许可。

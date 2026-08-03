# GameDevelopmentKit 知识库（模块梳理）

> 基于对仓库 `E:\Project\GameDevelopmentKit` 的 20 轮循环梳理整理，并经多 agent 评审补充。
> 本知识库按模块组织，覆盖客户端（Unity/GF）、服务端（ET）、工具链与构建，目标是让开发者快速定位"某个功能在哪、怎么用"。

## 模块清单（26 篇）

### 一、架构与入口
| # | 模块 | 文档 | 核心位置 |
| --- | --- | --- | --- |
| 1 | 总体架构与启动流程 | [01-总体架构与启动流程.md](01-总体架构与启动流程.md) | `Unity/Assets/Scripts/Game/Procedure/`、`Game/Base/` |
| 2 | 模式选择与代码分层 | [02-模式选择与代码分层.md](02-模式选择与代码分层.md) | `Game/ET/`、`Game/Hot/`、`DefineSymbolTool` |

### 二、业务模块（GameHot）
| # | 模块 | 文档 | 核心位置 |
| --- | --- | --- | --- |
| 3 | GameHot 业务入口与流程 | [03-GameHot业务入口与流程.md](03-GameHot业务入口与流程.md) | `Game/Hot/Loader/Init.cs`、`Hot/Code/Base/HotEntry.cs` |
| 4 | UI 窗体体系 | [04-UI窗体体系.md](04-UI窗体体系.md) | `Game/UI/Common/` |
| 5 | UI 容器与组件绑定 | [05-UI容器与组件绑定.md](05-UI容器与组件绑定.md) | `Game/Container/`、CodeBind、ReactiveBinding |
| 6 | Entity 实体模块 | [06-Entity实体模块.md](06-Entity实体模块.md) | `Game/Entity/` |
| 7 | AssetSet 资源设置 | [07-AssetSet资源设置.md](07-AssetSet资源设置.md) | `Library/UGF/.../AssetSet/`、`Game/AssetSet/` |
| 8 | 场景与相机 | [08-场景与相机模块.md](08-场景与相机模块.md) | `Game/Scene/`、`Game/Camera/` |
| 9 | 音频模块 | [09-音频模块.md](09-音频模块.md) | `Game/Sound/SoundExtension.cs` |
| 10 | 本地化（多语言） | [10-本地化模块.md](10-本地化模块.md) | `Game/Localization/`、`Game/Builtin/` |
| 11 | 事件与日志 | [11-事件与日志模块.md](11-事件与日志模块.md) | `Game/Event/`、`Game/Log/` |
| 12 | 平台与 SDK 适配 | [12-平台与SDK适配.md](12-平台与SDK适配.md) | `Game/Platform/` |
| 13 | 内置窗体与通用 UI | [13-内置窗体与通用UI.md](13-内置窗体与通用UI.md) | `Game/Builtin/` |
| 14 | 工具类库 | [14-工具类库.md](14-工具类库.md) | `Game/Utility/`、`Game/Debugger/` |

### 三、ET 模式
| # | 模块 | 文档 | 核心位置 |
| --- | --- | --- | --- |
| 15 | ET 模块（Loader 与四程序集） | [15-ET模块.md](15-ET模块.md) | `Game/ET/Loader/`、`Game/ET/Code/`、`Library/ET/Core/` |
| 16 | ET 动态事件 | [16-ET动态事件.md](16-ET动态事件.md) | `Game/ET/Code/Model/Share/Module/DynamicEvent/` |

### 四、数据与协议
| # | 模块 | 文档 | 核心位置 |
| --- | --- | --- | --- |
| 17 | Luban 配置表 | [17-Luban配置表.md](17-Luban配置表.md) | `Design/Excel/`、`Share/Tool/ExcelExporter/`、`Game/Tables/` |
| 18 | Proto 协议生成 | [18-Proto协议生成.md](18-Proto协议生成.md) | `Design/Proto/`、`Share/Tool/Proto2CS/` |

### 五、编辑器与构建
| # | 模块 | 文档 | 核心位置 |
| --- | --- | --- | --- |
| 19 | 编辑器工具集 | [19-编辑器工具集.md](19-编辑器工具集.md) | `Game/Editor/`（Build、DefineSymbol、Toolbar、CodeCreator） |
| 20 | HybridCLR 热更新与一键打包 | [20-热更新与一键打包.md](20-热更新与一键打包.md) | `Book/HybridCLR热更.md`、`Game/Editor/Build/` |

### 六、服务端与运维（评审补充）
| # | 模块 | 文档 | 核心位置 |
| --- | --- | --- | --- |
| 21 | DotNet 服务端架构 | [21-DotNet服务端架构.md](21-DotNet服务端架构.md) | `DotNet/`（App/Loader/Hotfix/Model/Core/ThirdParty） |
| 22 | GameHot 网络层 | [22-GameHot网络层.md](22-GameHot网络层.md) | `Game/Hot/Loader/Network/` |
| 23 | 静态分析与代码生成器 | [23-静态分析与代码生成器.md](23-静态分析与代码生成器.md) | `Share/Analyzer/`、`Share/SourceGenerator/` |
| 24 | UGF 扩展组件与 Library 附属包 | [24-UGF扩展组件与Library附属包.md](24-UGF扩展组件与Library附属包.md) | `Library/UGF/.../Extension/`、FolderTag、ReplaceComponent、SocoTool |
| 25 | Recast 寻路链路 | [25-Recast寻路链路.md](25-Recast寻路链路.md) | `Tools/RecastNavExportor/`、`Config/Recast/`、`Share/Libs/`、`DotNet/Loader/RecastFileReader.cs` |
| 26 | 服务端运维组件 | [26-服务端运维组件.md](26-服务端运维组件.md) | `Share/Aspire/`、`Share/FileServer/`、`Config/NLog/`、Admin/Agent |

## 阅读建议
- 第一次接触：先读 #1、#2，了解双模式架构。
- 纯 GF 开发：重点 #3~#14、#22（网络）。
- ET 开发：重点 #15、#16 + #2、#21（服务端）。
- 配表/协议/构建：读 #17~#20。
- 服务端与运维：读 #21、#23、#25、#26。
- 官方详细教程见仓库 `Book/` 目录，本知识库与其互为补充。

# 模块 24：UGF 扩展组件与 Library 附属包

## 概述
`Unity/Assets/Scripts/Library/UGF/UnityGameFramework.Extension/` 是 GDK 对 UGF 的核心扩展库：除 AssetSet（见模块 07）外，还提供网络、屏幕适配、池化集合、热更代码启动、异步桥等组件。Library 下还有三个第三方附属包：FolderTag、ReplaceComponent、SocoTool。

## UGF Extension 运行时组件（Runtime/）
| 组件 | 作用 | 关键类 |
| --- | --- | --- |
| NetworkService | 网络高层封装：托管 helper 生命周期、订阅网络事件 | `NetworkServiceComponent`、`INetworkServiceHelper` |
| Screen | UGUI 屏幕安全区/分辨率/CanvasScaler 适配 | `ScreenComponent` |
| Resource | WebGL 资源加载辅助 | `WebGLResourceHelper`、`SceneAsset` |
| Collection | 可池化的集合容器（避免 GC） | `UGFDictionary/List/HashSet/Queue/StringBuilder` |
| CodeRunner | 热更代码启动（加载类型并 AddComponent） | `CodeRunnerComponent` |
| Loader | 启动流程（从 AssetBundle/编辑器加载 GameEntry） | `GameEntryLoader` |
| Awaitable | UGF 各模块事件 → UniTask 异步桥 | `Awaitable` 分部类（Download/Entity/Scene/UI/Resource 等） |
| Build | 版本信息模型 | `VersionInfo` |

### 常用示例
```csharp
// 池化集合（零 GC）
using var dict = UGFDictionary<int, string>.Create();
dict.Add(1, "hello");

// 异步加载实体
Entity entity = await GameEntry.Entity.Awaitable.ShowEntityAsync(entityId, data);

// 热更代码启动
GameEntry.CodeRunner.AddComponent(entryType);
```

## UGF Extension 编辑器工具（Editor/）
| 工具 | 作用 |
| --- | --- |
| `Resource/ResourceOptimize.cs` | 资源合并优化（去冗余） |
| `ResourceRule` | 资源收集规则编辑器（模式切换时激活 ET/GameHot 规则） |
| `MergeFileToVFS` | 打包进 VFS 文件系统 |
| `ResourceVersionAnalyzer` | 资源版本分析 |
| `CodeRunner` / `Build` | 编辑器侧配套 |

## Library 附属包
### FolderTag（文件夹标签）
- 编辑器工具：文件夹颜色标签与场景图标标记。
- 关键类：`FoldersBrowser`/`FolderInspector`/`FolderSettings`；配置存 `ProjectSettings/FolderTag_Prefs.json`。
- 作用：大型工程中按目录着色快速定位模块。

### ReplaceComponent（组件替换）
- 添加组件时按 `[ReplaceComponent(Type)]` 属性自动替换为新组件。
- 关键类：`ReplaceComponentEditor`（监听 `ObjectFactory.componentWasAdded`）。
- 作用：用框架组件替代原生组件（如 Image → UXImage），减少手动操作。

### SocoTool（Shader 变体工具）
- Shader 变体收集与剔除工具，控制包体。
- 关键类：`SocoShaderVariantsCollection`/`Stripper`、`ShaderVariantCollectionToolWindow`、`InvalidVariantStrip`、`ShaderVariantsStripperCondition` 系列。

## 注意
- `Game/Resource`、`Game/Screen` 目录当前为空（仅 meta），对应能力在 Library Extension 层实现，业务层尚未落地。
- 各组件挂载方式：在 GameEntry.prefab 上添加组件，经 `GameEntry.Extension.cs` 暴露访问（如 `GameEntry.NetworkService`、`GameEntry.Screen`）。

## 官方文档
- `Book/快速开始.md`（CodeRunner 启动）
- `Book/AssetSet.md`（AssetSet 扩展组件）

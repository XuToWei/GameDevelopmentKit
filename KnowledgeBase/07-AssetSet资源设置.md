# 模块 07：AssetSet 资源设置

## 概述
AssetSet 解决"**把资源设置到目标控件**"的完整生命周期问题：以 `Target + AssetPath + AssetType` 描述一次设置，合并重复请求、处理界面关闭/图片被替换/延迟回收，避免资源泄漏与闪烁。同一"路径+类型"只发一次加载，多目标共享池内资源。

## 核心机制
- 三种加载来源：**GF Resource**（包内）、**AssetSet FileSystem**（本地缓存文件系统）、**WebRequest**（远程下载 + 落盘持久化）。
- 目标销毁或换图后：Resource 源走 `UnloadAsset`，FileSystem/Web 源走 `Destroy`。
- 回收策略：30 秒释放检查、60 秒整理（`AssetSetComponent` 定时器）。

## 核心类（`Library/UGF/UnityGameFramework.Extension/Runtime/AssetSet/`）
| 类 | 职责 |
| --- | --- |
| `IAssetSet` / `AssetSet<T>` | 接口与抽象基类（AssetPath/Target/AssetType、SetAsset、IsCanRelease） |
| `AssetSetComponent` | 主组件（挂在 GameEntry.prefab，经 `GameEntry.AssetSet` 访问）；对象池、加载合并、定时整理 |
| `AssetSetObject` | 池内资源对象，释放时 UnloadAsset/Destroy |
| `LoadedAssetSet` | 目标-资源引用跟踪记录 |
| `AssetSetComponent.Resource.cs` / `WebRequest.cs` / `FileSystem.cs` | 三种加载源分部实现 |
| `ImageSet` / `UXImageSet` / `RawImageSet` | 同步版设置项 |
| `WaitableImageSet` / `WaitableRawImageSet` / `WaitableUXImageSet` | 带 `AutoResetUniTaskCompletionSource`，返回 UniTask，Clear 置取消 |

## 业务扩展（`Game/AssetSet/`）
| 类 | 职责 |
| --- | --- |
| `SetSpriteExtension` | 便捷扩展方法（同步/异步设置 Image.sprite） |
| `UXImageSet` | UXImage（UXTool）专用设置项 |
| `WaitableImageSet` 等 | 可等待版本，配合 UniTask 等待加载完成 |

## 典型用法
```csharp
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine.UI;

// 同步触发：本地/包内图片
icon.SetSprite("Assets/Res/UI/UISprite/Icon/world.png");

// 异步等待：远程头像（WebRequest 下载 + 本地缓存）
await avatar.SetTextureByWebRequestAsync($"https://cdn.example.com/avatar/{playerId}.png");

// 显式等待一个设置完成
await GameEntry.AssetSet.SetImageAsync(image, "Assets/Res/UI/xxx.png");
```

## 使用要点
- 同一路径+类型重复设置：只发一次底层加载，其余共享，避免重复 IO。
- UI 窗体关闭时 AssetSet 自动回收对应设置项（与 UI 容器配合）。
- 远程图片自动落盘，二次加载直接读本地文件系统，断网可复用。

## 官方文档
- `Book/AssetSet.md`（资源设置专题：图片加载、远程缓存、共享与回收）

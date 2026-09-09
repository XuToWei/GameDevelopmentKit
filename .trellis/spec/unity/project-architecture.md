# Project Architecture and Mode Boundaries

## Runtime modes

`Unity/Assets/Scripts/Game/Procedure/ProcedurePreset.cs` 按编译宏选择启动流程：

```text
ProcedurePreset
  ├─ UNITY_ET       → ProcedureET       → CodeRunner.StartRun("ET.Init")
  ├─ UNITY_GAMEHOT  → ProcedureGameHot  → CodeRunner.StartRun("Game.Hot.Init")
  └─ default        → ProcedureGame
```

- `UNITY_ET` 与 `UNITY_GAMEHOT` 是互斥业务模式。
- `UNITY_HOTFIX` 是热更装载能力，不等于业务模式本身。
- 修改宏、asmdef define constraints 或启动流程时，必须同时检查两条路径和默认路径。
- `ProcedureET`/`ProcedureGameHot` 负责启动和停止 CodeRunner；业务类不手动模拟另一模式的入口。

## Loader versus hot Code

### GameHot

```text
Unity/Assets/Scripts/Game/Hot/Loader/  稳定初始化、组件注册、资源/网络基础设施
Unity/Assets/Scripts/Game/Hot/Code/    GameHot 业务代码，可由 HybridCLR 热更
```

`Game.Hot.HotEntry` 只持有入口需要的组件，如 `Procedure`、`Tables` 和项目组件。Loader 不依赖只能在热更程序集存在的类型。

### ET

```text
Unity/Assets/Scripts/Game/ET/Loader/         World、Fiber、CodeLoader、GF/ET bridge
Unity/Assets/Scripts/Game/ET/Code/Model/     ET entity/component 与共享模型
Unity/Assets/Scripts/Game/ET/Code/ModelView/ 客户端 ET entity 与 Mono/GF view
Unity/Assets/Scripts/Game/ET/Code/Hotfix/    服务器/共享热更系统
Unity/Assets/Scripts/Game/ET/Code/HotfixView/ 客户端 view 热更系统
```

`ET/Loader/Init.cs` 创建 `World`、Logger、TimeInfo、FiberManager、Config 和 CodeLoader；`CodeLoader.cs` 根据模式从 Unity 程序集或 `.dll.bytes` 加载 Model、ModelView、Hotfix、HotfixView，再执行 `ET.Entry.Start`。启动层不复制 ET 系统注册逻辑。

## ET/GF bridge

ET 逻辑对象和 GF 表现对象有不同所有权：

- `UGFUIForm<T>` 将 ET entity 与 `AETMonoUGFUIForm`/GF UIForm 关联。
- `UGFEntity<T>` 将 ET entity 与 `AETMonoUGFEntity`/GF Entity 关联。
- ET entity `Dispose()` 会取消自己的异步操作并隐藏关联 GF 对象；只隐藏 GF 对象不会自动销毁 ET entity。
- `UGFSystemSingleton` 通过 `UGFUIFormSystemAttribute`、`UGFUIWidgetSystemAttribute` 和 `UGFEntitySystemAttribute` 分发回调。

UI/Entity 的长期业务状态放在 ET entity/component 或 GameHot EntityData 中，Mono view 只保存 Unity 组件绑定和表现状态。

## Assembly rules

- 先查看 asmdef 的 `references`、`includePlatforms` 和 `defineConstraints`，再决定文件位置。
- Editor-only 的 AgentBridge、CodeBind、构建和导表工具不得进入 Player Runtime assembly。
- 生成代码所在 assembly 必须与 generator output target 匹配；不要复制文件规避 asmdef 引用错误。
- Unity 生成的 `.csproj`/solution 不手工维护；工具构建按 `CLAUDE.md` 使用 `Kit.sln`。

## Checklist

- [ ] 改动只进入一个明确模式/层，或已证明 shared 层是两边共同依赖。
- [ ] Loader 没有引用热更专属业务；HotfixView 没有把 Unity view 类型泄露到服务器层。
- [ ] ET Entity 与 GF view 的创建、隐藏、Dispose 由同一 owner 管理。
- [ ] 模式切换后 Luban/Proto 输出、资源路径、生成 ID 和入口仍一致。
- [ ] code reload/domain reload 后没有重复注册或残留静态状态。

## References

- `Unity/Assets/Scripts/Game/Procedure/ProcedurePreset.cs`
- `Unity/Assets/Scripts/Game/Procedure/ProcedureET.cs`
- `Unity/Assets/Scripts/Game/Procedure/ProcedureGameHot.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/Init.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/CodeLoader.cs`
- `Unity/Assets/Scripts/Game/Hot/Code/Base/HotEntry.cs`

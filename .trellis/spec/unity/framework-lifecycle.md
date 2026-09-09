# Framework-Managed UI and Entity Lifecycle

## Scope

本规范适用于由 UnityGameFramework 或 ET/GF bridge 管理的 UIForm、UIWidget、Entity 和 UIEntity。普通 MonoBehaviour、UICell、纯 ET Entity（没有 UGF view）不机械套用本规范。

GDK 有两套真实实现：GameHot 的 `AUIForm`/`AExUIForm`、`AUIWidget`/`AExUIWidget`、`AEntity`/`AExEntity`，以及 ET 的 `UGFUIForm<T>`、`UGFUIWidget<T>`、`UGFEntity<T>` 与 `UGFSystemSingleton`。

## Lifecycle matrix

| 对象 | 初始化/创建 | 每次进入 | 每次退出 | 临时整体显隐 |
|---|---|---|---|---|
| GameHot UIForm | `OnInit` | `OnOpen` | `OnClose` | GF `Visible`、Pause/Resume |
| GameHot UIWidget | `OnInit` | `OnOpen` | `OnClose` | `Visible` |
| GameHot Entity | `OnInit` | `OnShow` | `OnHide` | `Visible` |
| ET UIForm/Widget | ET `IAwake` + Mono view init | UGF lifecycle system | UGF close system / ET remove | view/GF visibility |
| ET Entity view | ET `IAwake` + GF show | `IUGFEntityOnShow` | `IUGFEntityOnHide` / `IDestroy` | GF view visibility |

`OnInit`/`IAwake` 只承担一次性初始化。`OnOpen`/`OnShow` 建立当前轮次的 userData、事件、临时集合和异步任务；`OnClose`/`OnHide` 只清理本层在该轮次新增的所有权。

## GF lifecycle APIs

- UIForm 进入使用 `GameEntry.UI.OpenUIForm` 或项目异步扩展，结束使用自身 `Close()` 或 `GameEntry.UI.CloseUIForm`。
- UIWidget 由所属 `AExUIForm`/`AExUIWidget` `AddUIWidget` 后使用 `OpenUIWidget`/`DynamicOpenUIWidget` 和 `CloseUIWidget`。
- Entity 由 owner container 或 `GameEntry.Entity` `ShowEntity`/`ShowEntityAsync`，结束使用对应 `HideEntity`。
- 已经进入生命周期、只需要暂时隐藏整体表现时使用 `Visible`；需要撤销事件、资源或异步工作时必须 Close/Hide。
- 受管根节点不得直接 `SetActive`，不得从业务代码调用 `InternalSetVisible` 或手动调用 `OnOpen`/`OnClose`/`OnShow`/`OnHide`。内部纯表现子节点可以 `SetActive`。

```csharp
// Form 退出当前生命周期。
Close();

// Widget 由其 parent 管理。
OpenUIWidget(widget, userData);
CloseUIWidget(widget);

// Entity 由 owner 管理，而不是直接失活根节点。
HideEntity(entity);
```

## ETUI and ETEntity ownership

ETUI 的逻辑所有者是 ET Entity；GF 对象只是 view。唯一实例使用 `UIComponent.AddUIFormComponentAsync<T>` 或 `GFEntityComponent.AddGFEntityComponentAsync<T>`，多实例使用对应 `Add...ChildAsync<T>`。`CloseAllUIForms`/owner remove 会移除 ET 子实体或组件，不只是隐藏 GF 对象。

`UGFUIForm<T>.Dispose()` 会取消在途打开并关闭 GF UIForm；`UGFEntity<T>.Dispose()` 会取消在途显示并隐藏 GF Entity。只调用 GF hide 不等价于 ET `Dispose`。

ET 系统通过 `EntitySystemOf` 加生命周期 marker 接收分发，例如 `IUGFUIFormOnOpen`、`IUGFUIFormOnClose`、`IUGFEntityOnShow`。只有声明接口的实体才应实现对应系统方法。Mono view 通过 CodeBind 提供引用，System 通过 `self.View` 使用。

## Base calls and cleanup

每个 override/系统生命周期实现遵循基类或 dispatcher 的约定：进入时先让框架建立可用状态，再做本层工作；退出时先撤销本层直接订阅、CTS 和临时所有权，再交给 base/owner 完成框架清理。不要重复调用 owner 已经负责的 `UnsubscribeAll`、`HideAllEntity`、`UnloadAllAssets` 或 Widget 全量清理。

- `AExUIForm`/`AExEntity` 已拥有事件、Entity、资源和对象池容器；派生类只清理直接注册且不由容器托管的资源。
- `OnInit` 建立的固定按钮绑定不在每次关闭时反复解绑。
- 本轮创建的 `CancellationTokenSource` 在退出时 Cancel/Dispose；回调或 await 完成后还要检查 owner/lifetime 是否有效。
- 对象池回收、ET `IDestroy` 和 GF `OnRecycle` 必须清除本对象持有的业务状态，不清除其他 owner 的状态。

## Async and visibility

异步加载完成后再检查当前生命周期/取消 token，再写入 view。ET `UGFEntity` 和 `UGFUIForm` 已提供取消/Dispose 边界，不要另建 `m_IsAlive` 或第二套生命周期版本。`Visible` 只用于保留状态的临时显隐，不能替代退出。

## Verification checklist

- [ ] 连续两轮 Open/Close 或 Show/Hide 不重复订阅、不累积临时对象。
- [ ] owner 关闭时 Widget、Entity、资源和事件按 owner contract 清理。
- [ ] 异步完成前退出时，旧结果不会写入已关闭或新一轮对象。
- [ ] 受管根没有直接/间接 `SetActive`、手动生命周期调用或 `InternalSetVisible`。
- [ ] ET logical entity、GF view 和 Mono binding 的所有权清楚；GF hide 与 ET dispose 未混淆。
- [ ] 所有生命周期 override 遵守 base/dispatcher 调用约定。

## References

- `Unity/Assets/Scripts/Game/UI/Common/AUIForm.cs`
- `Unity/Assets/Scripts/Game/UI/Common/AExUIForm.cs`
- `Unity/Assets/Scripts/Game/UI/Common/AUIWidget.cs`
- `Unity/Assets/Scripts/Game/UI/Common/AExUIWidget.cs`
- `Unity/Assets/Scripts/Game/Entity/EntityLogic/AExEntity.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm/UGFUIForm.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/UGF/UIWidget/UGFUIWidget.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/UGF/Entity/UGFEntity.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/UGF/UGFSystemSingleton.cs`
- `Book/UI开发.md`
- `Book/Entity开发.md`

# Reactive State and UI Refresh

## Scope

GDK 同时存在两套响应式机制，必须按层区分：

1. 外部 `me.xw.reactivebinding` 包提供 `IReactiveObserver`、`ReactiveSource`、`ReactiveBind`、`VersionField` 和版本集合。
2. ET 在 `Game/ET/Code/Model/Share/Module/Reactive/` 定义 `IETReactive`、`ETReactiveSystem`、`ETReactiveSource`、`ETReactiveBind`，由 SourceGenerator 生成观察缓存和方法。

不能把两套 attribute 混用，也不能根据一个机制的示例推断另一机制的签名。

## One source of truth

UI 表现应来自权威领域状态或明确 UI context，而不是直接持有网络 response、临时兼容 cache 或第三份状态。一个独立表现应只有一个更新 owner：首次观察负责初始投影，后续由同一 Bind/system 更新。

```text
ET/GameHot domain state
  → version / reactive source / ET system
  → minimal binding or projection
  → UI view
```

如果状态不可观察，先修正版本传播、ET reactive 声明或领域 owner，不叠加事件与手动 Refresh 作为补丁。

## Minimal projection

先列出每项独立 UI 变化的最小输入：文本只依赖对应值和语言格式；按钮只依赖可用/请求中/权限状态；Icon 只依赖对应配置/资源引用；一个 Bind 不应因同属一个 Form 就刷新整页无关控件。

Bind/system 只更新表现，不发请求、不写领域数据、不承担生命周期清理。多个输入只有在共同决定同一投影时才合并。

## External ReactiveBinding

使用包内 API 前先确认类型签名和生成器约束：`ReactiveSource` 可标记字段、属性或无参返回方法，`ReactiveBind` 指向 source 名称，`VersionList`/`VersionDictionary`/`VersionHashSet` 负责集合版本传播。对象池或重新绑定上下文时按包 API 调用 `ResetChanges`，不要直接修改普通集合后期待自动刷新。

## ET reactive

ET reactive owner 必须实现 `IETReactive`，按 source generator 要求声明 `partial`，system 使用 `[ETReactiveSystem]`，source 使用 `[ETReactiveSource]`，bind 使用 `[ETReactiveBind(nameof(...))]`。SourceGenerator 的诊断规则拥有具体签名和 `nameof` 要求；修改后先修复编译/生成诊断。

ET reactive 的状态 owner 是 ET Entity/Component；Mono view 通过 `self.View` 接收最终投影，不把 ET reactive attribute 放到不受其 generator 管理的 GameHot MonoBehaviour。

## Avoid duplicate paths

不要同时使用 event、OnOpen/SetData 手动刷新、ReactiveBind 和 UI cache 刷新同一控件。仍有必要承担请求/生命周期职责的事件可以保留，但不能再直接更新已由响应式投影拥有的控件。

## Checklist

- [ ] 已确认使用的是外部 ReactiveBinding 还是 ET reactive generator。
- [ ] Source 来自权威领域状态/UI context，未读取临时 response/cache。
- [ ] 每个独立表现只有一个更新 owner，Bind 不发请求/写状态。
- [ ] source 输入最小，集合修改通过可观察版本机制。
- [ ] ET owner 满足 `IETReactive`/partial/generator 约束。
- [ ] 对象池、上下文切换和 UI 关闭后不会触发旧回调。

## References

- `Unity/Library/PackageCache/me.xw.reactivebinding@*/Runtime/`
- `Unity/Assets/Scripts/Game/ET/Code/Model/Share/Module/Reactive/ETReactiveAttributes.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Model/Share/Module/Reactive/IETReactive.cs`
- `Share/SourceGenerator/Generator/ETReactiveSystemGenerator/ETReactiveSystemGenerator.cs`
- `Share/Analyzer/Analyzer/EntityMethodDeclarationAnalyzer.cs`

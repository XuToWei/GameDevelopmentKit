# HybridCLR and UniTask

## Scope

本规范约束 Unity 中 GameHot/ET 的热更装载和异步代码。GDK 使用 UniTask；不要把其他项目的 ETTask 约定复制进来，也不要为同一条流程创建第二套 awaitable。

## Hot reload boundaries

- GameHot 的稳定基础设施在 `Game/Hot/Loader/`，可热更业务在 `Game/Hot/Code/`。
- ET 的 Loader 在 `Game/ET/Loader/`；业务按 Model、ModelView、Hotfix、HotfixView 分装。
- `ProcedureGameHot` 和 `ProcedureET` 通过 `GameEntry.CodeRunner.StartRun` 启动对应入口，退出时停止 CodeRunner。
- ET `CodeLoader` 在 editor/非热更场景使用已加载程序集，在 code-bytes 模式装载 `Game.ET.Code.Model*`、`Hotfix*` 的 `.dll.bytes`/`.pdb.bytes`，然后建立 `CodeTypes` 并调用 `ET.Entry.Start`。

修改热更代码后，必须等待 Unity compile/domain reload 或对应 code bytes 生成完成，再验证新程序集。不要在仍有旧程序集时用猜测的类型或缓存的 method ID 继续验证。

## Async contract

- 网络、资源、UI、场景和 ET 入口使用 `UniTask`/`UniTaskVoid`，异步方法不使用阻塞 `.Wait()`、`.Result` 或 `async void`。
- `UniTaskVoid` 只用于明确的 fire-and-forget 入口，并且 owner 已有生命周期/错误记录；需要调用者等待和观察失败时返回 `UniTask`。
- 所有权属于 UIForm、Widget、Entity、ET Entity 或组件；owner 退出时取消/Dispose CTS、token 或框架提供的取消边界。
- await 后不要直接假定对象仍有效；检查取消、owner identity、请求 token、ET `IsDisposed` 或 GF 生命周期后再 Apply。
- 取消是控制流，不应被当作业务错误重复记录；网络/协议/生成失败必须保留失败语义，不以空数据完成。

## Existing patterns

GameHot WebSocket 的 `NetworkServiceHelper.SendAsync` 用 correlation ID 保存等待的 response，在 disconnect 时取消并清空 waiter。ET message handler 使用 ET 的 handler attribute 和 UniTask，例如 `MessageLocationHandler` 的 `Run`。

```csharp
public async UniTask<T> SendAsync<T>(Packet packet, object userData, CancellationToken cancellationToken)
    where T : Packet
{
    // 由现有 helper 统一分配 correlation、等待和取消。
}
```

不要在调用点重新实现 correlation map、断线清理、WebSocket channel 或 ET fiber 调度。

## AOT and generated code

涉及 HybridCLR 构建、AOT metadata、热更 DLL、link.xml 或 code bytes 时，遵循项目 `Book/HybridCLR热更.md` 和现有 Build/HybridCLR 工具顺序。不要把 Editor 程序集、AgentBridge 引用或测试代码带入 Player Runtime；不要手动编辑生成的 DLL/bytes。

## Verification checklist

- [ ] 修改前确认当前是 `UNITY_ET` 还是 `UNITY_GAMEHOT`，并验证对应 CodeRunner 入口。
- [ ] 异步方法可等待、可取消，或明确是 owner 托管的 fire-and-forget。
- [ ] await 后有生命周期/请求身份检查，退出不会回写旧对象。
- [ ] 热更程序集重新生成/加载后再验证，未使用旧 domain 的类型缓存。
- [ ] 构建顺序、AOT metadata 和 code bytes 验证按项目文档执行。

## References

- `Unity/Assets/Scripts/Game/Procedure/ProcedurePreset.cs`
- `Unity/Assets/Scripts/Game/Procedure/ProcedureET.cs`
- `Unity/Assets/Scripts/Game/Procedure/ProcedureGameHot.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/Init.cs`
- `Unity/Assets/Scripts/Game/ET/Loader/CodeLoader.cs`
- `Unity/Assets/Scripts/Game/Hot/Loader/Network/NetworkServiceHelper.cs`
- `Book/HybridCLR热更.md`

# Network Synchronization and Async Requests

## Scope

GDK 网络不是单一管线：ET 使用 ET message/handler/fiber；GameHot 使用 GF Network/WebSocket packet helper；项目也可能接入 WebRequest/HTTP。先按当前模式和实际协议选择 owner，不复制另一模式的请求模型。

## ET messages

ET 消息由 `Design/Proto/ET-*` 生成，通常进入 `Model` 的共享消息类型，由 `Hotfix`/`HotfixView` 的 `MessageHandler`、`MessageLocationHandler` 等系统处理。handler 负责请求上下文、业务结果和领域状态更新；不要在 UI 或 Mono view 重复解析同一消息。

目标中的实际 handler 形状可参考 `C2M_TestRobotCaseHandler.cs`：使用生成的请求/响应类型和 ET 的 `MessageLocationHandler`，在 `Run` 中完成领域动作并写入响应。实际 handler 泛型、`SceneType` 和响应类型以生成消息与 ET 版本为准；不要复制不存在的示例类型。

## GameHot WebSocket

`Game/Hot/Loader/Network/NetworkServiceHelper.cs` 建立 GF WebSocket channel，使用 packet correlation ID 和 waiter 等待对应响应；断线时取消并清空等待者。调用方应复用 helper，不重新实现 correlation dictionary、channel 或断线清理。

仓库中的 `wss://echo.websocket.events` 是示例 endpoint，不代表生产服务地址。修改 endpoint、packet 或 error handling 前先确认当前运行配置和调用方实际存在性。

## Failure, cancellation, and stale

请求 owner 处理真实失败边界：连接断开、超时、解析失败、业务错误、取消、会话/UID 切换、对象已销毁和请求 token 过期。失败不应用数据，也不以空列表/默认对象覆盖有效状态；stale 结果直接忽略。成功且仍属于当前 owner 的必需 payload 进入领域状态。

网络层或 handler 已经完成的校验不要在 UI 再重复一遍。协议明确可选字段和合法空结果按其业务语义处理，但不能把每个成功字段都当作 nullable。

## HTTP boundary

如果任务确实使用 HTTP，endpoint、序列化、retry、response parser 和错误码必须集中在现有 HTTP owner。不要因为其他项目存在 HTTP 文档就凭空创建 GameHot HTTP gateway；没有实际外部替换实现或测试替身时，直接复用当前网络 API。

## UI interaction

UI 只触发领域 owner 的请求/刷新入口并观察状态，不拼 endpoint、不解析 wire response、不直接维护另一份服务器缓存。ETUI 通过 ET component/system，GameHot 通过其现有 Component/Procedure/Network owner。

## Verification checklist

- [ ] 当前模式与协议类型已确认（ET message、GameHot UGF packet 或实际 HTTP）。
- [ ] endpoint/channel/handler 复用了已有 owner，没有重复请求管线。
- [ ] 网络/解析/业务失败、取消、会话切换和 stale 都不 Apply。
- [ ] 成功响应由唯一领域 owner 落地，UI 不直接保存 response。
- [ ] ET handler、GF packet 和 ET/GameHot 生成类型未混用。
- [ ] 异步退出不会访问已销毁 Entity、关闭 UI 或新上下文。

## References

- `Unity/Assets/Scripts/Game/Hot/Loader/Network/NetworkServiceHelper.cs`
- `Unity/Assets/Scripts/Game/Hot/Loader/Network/NetworkExtension.cs`
- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/`
- `Unity/Assets/Scripts/Game/ET/Code/Model/Generate/Client/Message/`
- `Design/Proto/ET-Client/`
- `Design/Proto/ET-ClientServer/`
- `Design/Proto/GameHot/`
- `Book/Proto生成工具.md`

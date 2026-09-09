# Reuse Existing APIs and Keep Error Handling Proportional

## Search first

新增方法、接口、容器、缓存、网络流程或刷新机制前，先搜索已有 owner：

| 需求 | 优先使用 | 位置 |
|---|---|---|
| GameHot 启动/更新 | `GameEntry.CodeRunner`、`HotComponentEntry`、既有 Procedure | `Game/Procedure/`、`Game/Hot/Loader/` |
| ET entity/system | ET `AddChild`/`AddComponent`、`EntitySystemOf`、UGF bridge | `Game/ET/Code/`、`Game/ET/Loader/UGF/` |
| GF UI | `GameEntry.UI`、`AExUIForm`、owner Widget API | `Game/UI/Common/` |
| GF Entity | `GameEntry.Entity`、`EntityContainer`、owner API | `Game/Entity/` |
| 表格/生成数据 | `TablesComponent`、`HotEntry.Tables`、对应 ET Tables | `Game/Hot/Code/Tables/`、生成目录 |
| Excel/Luban | AgentBridge command 与 `ExcelExporter` | `Game/Editor/AgentBridge/`、`Share/Tool/ExcelExporter/` |
| 网络 | ET message handler 或 `NetworkServiceHelper` | `Game/ET/Code/Hotfix/`、`Game/Hot/Loader/Network/` |

搜索行为和所有权，不只搜索名字。已有 API 满足需求时直接使用。

## Abstraction admission

1. 现有生产 API 能完成任务：直接调用。
2. 原 owner 只缺一个内聚能力：扩展原 owner。
3. 单次复杂步骤需要表达意图：提取 private 方法。
4. 两个真实消费者共享稳定契约：抽取最小公共类型/方法。
5. 两个运行时实现确实需要替换，或外部系统有实际测试替身：抽取 interface/transport。

“以后可能复用”“方便 mock”“分层更整齐”没有真实消费者/实现时，不足以新增 gateway、facade、provider、adapter 或 wrapper。

## Error ownership

每类错误只由最接近且能采取动作的边界负责：传输/解析失败由网络或协议入口处理；ET 路由/handler 失败由对应 system/handler 处理；GameHot WebSocket 断开、相关 ID 和取消由 `NetworkServiceHelper`/请求 owner 处理；配置、生成、Prefab、CodeBind 错误回到源表、配置、Prefab 或工具；已关闭/销毁对象由生命周期 owner 处理。

不能恢复、转换或补充唯一上下文时不要 catch。不要原样重抛前重复打印同一个异常，不要 catch 后返回空列表伪装成功。

## Trusted contracts

初始化已建立的必需对象、生成配置的必需表行、成功协议 payload、Prefab 必需绑定和生产资源默认是可信契约。缺失时修复源头，而不是在每个消费点添加 `?.`、`??`、默认值、占位对象或静默 return。

真正允许判空或 fallback 的场景包括：用户输入、协议明确的可选字段、合法空态、字典试探查询、取消/stale、对象池/生命周期失效和可选 callback。每个判断都要说明值为何可缺失以及当前层的动作。

## Existing examples

- ET 消息处理复用 `MessageLocationHandler` 和生成的消息类型：`C2M_TestRobotCaseHandler.cs`。
- ET UI 单实例/多实例分别复用 `UIComponent.AddUIFormComponentAsync` 与 `AddUIFormChildAsync`。
- GameHot network 调用复用 `NetworkServiceHelper` 的 correlation、等待和断线取消。
- GF UI/Entity 复用 `AExUIForm`、`AExEntity` 的容器清理，不在派生层再全量清一次。

## Checklist

- [ ] 新代码前已搜索已有 owner/API。
- [ ] 新抽象能指出真实第二消费者/实现或外部系统边界。
- [ ] 每个 catch、判空、Try 和 fallback 都对应真实契约与动作。
- [ ] 失败没有被默认值、空结果或占位资源伪装成成功。
- [ ] 同一失败只在一个明确边界记录。
- [ ] 没有复制 ET/GF、Excel/Luban 或 UI/Entity 的已有所有权逻辑。

## References

- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/Server/Demo/Map/C2M_TestRobotCaseHandler.cs`
- `Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/UI/UIComponentSystem.cs`
- `Unity/Assets/Scripts/Game/Hot/Loader/Network/NetworkServiceHelper.cs`
- `Unity/Assets/Scripts/Game/UI/Common/AExUIForm.cs`
- `Unity/Assets/Scripts/Game/Entity/EntityLogic/AExEntity.cs`

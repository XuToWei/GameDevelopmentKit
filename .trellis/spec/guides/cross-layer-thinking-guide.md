# Cross-Layer Thinking Guide

> 跨层任务先画数据流，再确定每条边界的格式、所有权、失败语义和验证方式。

## The problem

多数难定位问题发生在层与层之间，而不是单层函数内部。GDK 的典型链路包括：

```text
Excel / proto source
  → Luban / Proto2CS generator
  → generated C# and runtime data
  → Loader / ET Model or GameHot owner
  → Hotfix / ModelView / HotfixView or GF UI/Entity
  → presentation
```

另一条工具链是：

```text
AgentBridge request
  → editor ICommandHandler
  → Excel/Luban/CodeBind operation
  → generated files / AssetDatabase refresh
  → compile result and readback
```

## Before implementing

### 1. Map the complete flow

写出 `Source → Transform → Store → Retrieve → Display`，并为每个箭头回答：

- 输入和输出的真实类型/编码是什么？
- 哪一层负责转换、校验、版本或取消？
- 数据失败时是否应停止、忽略 stale、重同步，还是表现合法空态？
- 结果能否从真正的状态 owner 读回，而不是从刚写入的参数推断？

### 2. Identify boundaries

| Boundary | 要明确的契约 |
|---|---|
| Excel/proto → generator | 源文件、配置、目标模式、生成目录和生成命令。 |
| generated protocol → ET | opcode/消息类型、MemoryPack/Protobuf 约定、Handler owner。 |
| generated config → runtime | bin/json 形式、资源路径、表 ID 和严格/可选读取语义。 |
| ET Model → Hotfix | Entity/Component 所有权、系统 attribute、生命周期和程序集引用。 |
| ModelView/HotfixView → UGF | ET logical object、Mono view、UGF ID、owner container 和回调。 |
| network → domain state | 连接/解析/业务失败、取消、顺序、stale 和 Apply 入口。 |
| domain state → UI/Entity | 唯一表现 owner、最小投影、版本/事件信号和关闭清理。 |
| Agent → Unity Editor | fixed-slot single-flight、schema discovery、原子文件 exchange 和 response ack。 |

### 3. Define the contract

在写代码前记录：

- 这条边界的输入/输出格式；
- 必需值、合法空值和用户输入；
- 错误码、异常或取消如何传播；
- 哪个对象拥有写入、清理、重试和日志；
- 修改后哪个测试、场景或 readback 能证明往返正确。

## Common mistakes

### Implicit format assumptions

不要假定时间单位、ID 类型、opcode 区间、Luban bin/json 或 Proto code type。读取当前 `luban.conf`、`proto.conf`、生成代码和调用方，在边界处显式转换。

### Scattered validation

不要让 Generator、Loader、Domain、UI 各自对同一个必需字段做不同的默认化。入口负责可拒绝输入和传输失败；成功协议字段按实际契约落地；真正的可选字段在拥有该语义的层处理。

### Leaky abstractions

UI 不应知道生成器命令行、ET 数据库或网络 wire format；Mono view 不应拥有 ET entity 的长期业务状态；Editor command 不应把导出产物当源文件。

### Every consumer parses the payload

同一消息/配置被多个消费者使用时，在 shared/generated/model owner 处定义类型和转换；展示层只格式化结果，不重复从 `object`、JSON 或网络包中读取字段。

### Stale results overwrite newer state

异步结果必须带当前 owner、session、request token、版本或生命周期身份。结果回来后再次检查，再执行 Apply；过期结果应忽略，不能用空数据覆盖新状态。

## Event and projection boundary

如果某个模块确实使用 append-only event、消息队列或事件回放，保持以下单一所有权：

- 中央消息/事件类型和解码；
- 从 `unknown`/wire payload 到领域类型的 normalization；
- 事件顺序、版本或 ID 的分配；
- reducer/projection；
- UI/命令消费的只读投影。

没有事件回放需求时，不要为了形式创建第二套 event log 或 reducer。

## Verification checklist

Before implementation:

- [ ] 已画出从源数据到表现的完整链路。
- [ ] 已列出所有跨层边界和每条边界的 owner。
- [ ] 已确认模式、编码、ID、时间和版本语义。
- [ ] 已决定校验、错误、取消和重试各自归属。

After implementation:

- [ ] 成功、失败、取消、空值和 stale 都有对应验证。
- [ ] 数据能从状态 owner 读回，证明没有只写未读。
- [ ] 消费者复用共享 decoder/projection，不做局部 cast。
- [ ] 生成产物来自源配置，未手工打补丁。
- [ ] AgentBridge exchange 遵守 canonical protocol。
- [ ] UI/Entity 关闭、对象池复用和异步退出不会回写旧状态。

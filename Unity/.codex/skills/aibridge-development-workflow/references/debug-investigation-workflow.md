# 调试诊断工作流

## 适用范围

用户要求排查问题、追踪运行时信息、分析日志、复现异常、定位根因、比较 Runtime/Player/Play Mode 行为时，使用本分支。默认目标是诊断结论，不是改代码。

## 核心原则

1. 先证据后结论；没有命令输出、日志、截图、堆栈、Runtime 状态或代码证据的判断只能标记为 `uncertain`。
2. Runtime 错误在调试阶段是证据，不是 workflow 失败条件；调试 recipe 的 gate 应优先检查证据是否存在。
3. 不在弱证据下修改代码或资源。只有 confirmed 根因且用户要求修复，才交接到实施分支。
4. 每个候选根因必须有 `confirmed`、`refuted` 或 `uncertain` 状态。
5. 报告必须区分已验证事实、推断、未验证项和下一步动作。

## 阶段

### 1. 问题定界

记录：

- 症状和触发条件。
- 期望结果与实际结果。
- Unity Editor、Play Mode、Player、平台、Runtime target id 或 URL。
- 复现步骤、频率、最近改动、用户是否要求修复。

### 2. 基线证据

常用命令：

```bash
$CLI compile unity
$CLI get_logs --logType Error --count 100
$CLI get_logs --logType Warning --count 100
$CLI get_logs --regex "<关键字|异常名|对象名>"
```

编译失败时先把编译错误作为最高优先级证据；不要继续基于过期 Runtime 状态推断。

### 3. Runtime 追踪

常用命令：

```bash
$CLI runtime list_targets
$CLI runtime list_targets --probe true
$CLI runtime status --target latest
$CLI runtime diagnose --target latest
$CLI runtime logs --target latest --logType Error --count 100
$CLI runtime screenshot --target latest
$CLI runtime perf --target latest --duration 5s --interval 100ms
$CLI runtime handlers --target latest
```

需要执行项目暴露的诊断 handler 时：

```bash
$CLI runtime call --target latest --action <handler> --json "<json>"
```

多目标或多平台差异排查时，优先使用 `runtime-target-sweep` 或 `runtime-debug-investigation` recipe，并保留 target id、URL、截图、日志和 perf artifact。

`runtime list_targets` 默认是 quick 路径；只有确实需要本机端口扫描时才加 `--probe true`，深诊断用 `runtime diagnose` 显式触发。

### 4. 复现与交互

Play Mode UI 或输入路径问题可使用：

```bash
$CLI input click --path "<Canvas/Button>"
$CLI input click_pct --x 0.5 --y 0.5
$CLI screenshot game
$CLI screenshot gif
```

动作必须串行执行，避免输入状态竞争。截图或 GIF 只证明可见状态，不单独证明业务状态。

### 5. 代码和资源关联

- C# 堆栈、符号、定义、引用、调用链：优先 `aibridge-code-index`。
- 日志文本、配置、资源路径、Prefab/Scene 名称、非 C# 内容：使用 `rg` 或文件读取。
- Prefab/Scene/asset 结构问题只读检查优先；需要修改时再加载对应修改 Skill。

### 6. 候选根因验证

输出结构：

```text
候选根因：
1. 状态：confirmed/refuted/uncertain
   证据：命令、日志行、截图 artifact、代码位置或 Runtime target
   说明：为什么证据支持或反驳该判断
   剩余风险：仍未覆盖的环境、平台或输入
```

### 7. 结论与交接

调试结论必须包含：

- 根因状态和置信度。
- 关键证据列表。
- 复现路径或无法复现的原因。
- 未验证项。
- 如果需要修复：建议修改范围、风险、验证命令。

# Thinking Guides

这些指南用于在修改代码前发现跨层契约、重复实现和隐藏的所有权问题。它们是通用思考规则；Unity、GameHot、ET 和工具的具体约束见 [`../unity/index.md`](../unity/index.md)。

## Available Guides

| Guide | Purpose | When to use |
|---|---|---|
| [Code Reuse Thinking Guide](./code-reuse-thinking-guide.md) | 在新增 helper、接口、组件或转换逻辑前寻找已有所有者，避免重复实现 | 看到相似代码、重复常量、第二个数据读取者或准备新增抽象时 |
| [Cross-Layer Thinking Guide](./cross-layer-thinking-guide.md) | 绘制数据流并明确各层的输入、输出、错误和版本契约 | 修改协议、Luban/Proto 生成、ET/GF 桥接、网络、配置或 UI 数据流时 |

## Quick Triggers

### Think about cross-layer behavior when

- [ ] 数据经过 Proto/Luban 生成代码、Loader、领域状态和表现层。
- [ ] 同时触及 ET `Model`/`Hotfix`/`ModelView`/`HotfixView` 或 GameHot `Loader`/`Code`。
- [ ] 数据格式、序列化方式、时间单位、ID 或版本语义发生变化。
- [ ] 多个消费者需要读取同一消息、配置或派生状态。
- [ ] 你不确定逻辑应该属于生成器、Loader、领域 owner 还是 UI/Entity。
- [ ] UI 开始直接解析网络响应或在多个路径手动刷新同一表现。

→ 阅读 [Cross-Layer Thinking Guide](./cross-layer-thinking-guide.md)。

### Think about reuse when

- [ ] 你准备创建新的 `*Helper`、`*Utility`、wrapper、gateway、adapter 或 extension。
- [ ] 你准备修改常量、配置、协议字段或生成路径。
- [ ] 两个文件读取相同的未类型化 payload 字段或重复处理同一种结果。
- [ ] 同一个状态由多个分支分别更新，可能需要一个集中 reducer/dispatcher。
- [ ] 你正在复制 UI/Entity 的生命周期、取消、资源或容器清理代码。

→ 阅读 [Code Reuse Thinking Guide](./code-reuse-thinking-guide.md)。

## Review Reminder

AI 或人工 review 的每个重要 finding 都必须回到实际代码、调用链和数据来源核对。尤其要区分：

1. 外部输入、用户输入和仓库内可信配置；
2. 真实缺失契约和框架已经保证的必需对象；
3. 有意的生命周期/模式分支和意外的 fallthrough；
4. 测试真正验证生产行为，还是只验证测试自己写入的值。

## Pre-Modification Rule

**修改任何值、配置、ID、路径或协议字段前先搜索全部引用。**

```bash
grep -R "value_to_change" -n .
```

修改后再次搜索旧值和新值，确认没有漏改、重复注册或生成路径漂移。

## How to use this directory

1. 开始任务时阅读适用的 guide 和 [`../unity/index.md`](../unity/index.md)。
2. 发现新的边界规则时，把它写入拥有该规则的具体 spec，而不是在多个文件复制一份。
3. 完成后用 guide 的 checklist 做一次反向检查，并如实记录未运行的验证。

**核心原则：先画清所有权和数据流，再写最小改动。**

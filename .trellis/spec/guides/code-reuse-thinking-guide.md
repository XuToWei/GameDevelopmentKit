# Code Reuse Thinking Guide

> 新增代码前先确认已有所有者和实现，避免同义 API、重复状态和分叉契约。

## Why duplication hurts

复制或重新包装已有逻辑会让修复只落在一个副本上，最终造成行为漂移。游戏框架中尤其容易重复：

- ET entity 与 GF view 的创建/销毁；
- GameHot 和 ET 各自的网络、配置、生命周期入口；
- Proto/Luban 生成代码之外的手写 DTO；
- 同一 UI 的事件刷新、手动刷新和响应式刷新；
- AgentBridge 命令的参数校验、文件安全和错误转换。

## Search before writing

先搜索语义和所有者，而不只搜索准备使用的名字：

```bash
grep -R "ShowEntityAsync\|OpenUIFormAsync" -n Unity/Assets/Scripts
grep -R "LoadAsync\|GetString\|MessageHandler" -n Unity/Assets/Scripts Share DotNet
grep -R "new .*Helper\|interface I.*Transport" -n Unity/Assets/Scripts
```

确认以下问题：

| 问题 | 如果答案是“是” |
|---|---|
| 已有生产 API 能完成这件事吗？ | 直接调用；不要增加同义 wrapper。 |
| 已有组件/Entity/系统拥有这份状态吗？ | 扩展原 owner，保持状态只有一个来源。 |
| 这是生成内容吗？ | 修改源表、proto、配置或 generator；不要改生成文件。 |
| 这是两个模式都需要的稳定契约吗？ | 放到 shared/model 层，并确认两边都引用它。 |
| 只有一个调用点或一个实现吗？ | 先直接实现，不为“以后复用”预建接口。 |
| 有两个真实消费者/实现或外部替换边界吗？ | 才抽取最小公共类型或接口。 |

## GDK ownership map

- GameHot 运行时业务在 `Unity/Assets/Scripts/Game/Hot/Code/`，稳定入口和非热更代码在对应 `Loader`/`Unity/Assets/Scripts/Game/`。
- ET 业务按 `Unity/Assets/Scripts/Game/ET/Code/Model`、`ModelView`、`Hotfix`、`HotfixView` 分层；不要把只属于某层的实现复制到另一层。
- ET logical entity 与 GF/UGF view 的关联由 `UGFEntity<T>`、`UGFUIForm<T>` 和 owner component 管理；不要在业务处另建 Mono view registry。
- UIForm、Widget、Entity 的 GF 容器由 `AExUIForm`/`AExEntity` 或 ET UGF owner 管理；不要复制全量 `HideAll`、`UnsubscribeAll` 或资源释放。
- 编辑器命令集中在 `Unity/Assets/Scripts/Game/Editor/AgentBridge/`；新增 Excel/Luban/CodeBind 能力前先扩展现有 command owner。

## Prefer the smallest abstraction

允许抽象的证据至少满足一项：

- 两个真实消费者共享稳定语义；
- 两个运行时实现确实需要替换；
- 外部系统边界需要隔离，并且已有实际测试替身；
- 原 owner 扩展会破坏明确的程序集或生命周期边界。

只因“方便 mock”“看起来更分层”“可能以后用到”而创建 `interface`、gateway、facade、provider、adapter 或通用 base class，不属于足够证据。单次复杂步骤可以提取 `private` 方法来表达意图，但不要顺手建立新层。

## Centralize repeated contracts

如果多个消费者读取同一消息、配置、文件或 JSON 字段，建立一个靠近数据 owner 的类型、解码器、normalizer 或 projection。消费者使用共享结果，不各自做 cast、默认值转换或字段拼接。

状态由 `kind`、`action`、`status`、阶段或消息类型驱动时，优先使用一个清晰的 `switch`/dispatcher 维护完整转换表；只有在该状态确实由事件/消息驱动时才使用此规则，不要为简单顺序代码机械引入 reducer。

## Generated and mirrored outputs

Luban、Proto2CS、CodeBind、UI/Entity ID 和本地化导出文件都是派生结果。源文件与导出路径必须保持单一注册点：

- 表格和 `luban.conf`：`Design/Excel/ET/`、`Design/Excel/GameHot/`；
- 协议源：`Design/Proto/` 各模块及其 `proto.conf`；
- 生成器：`Share/Tool/ExcelExporter/`、`Share/Tool/Proto2CS/`；
- 生成代码/数据：由配置决定的 `Unity/Assets/Scripts/.../Generate`、`Unity/Assets/Res/...` 和 `Config/Luban`。

新增目标后搜索所有复制/生成路径；如果 init/export 或 ET/GameHot 两条机制产生同一类输出，优先让它们共享配置发现逻辑，而不是维护第二份手工列表。

## After batch changes

1. 搜索旧名字、旧路径和新名字，确认没有漏改或重复注册。
2. 对比生成目录、程序集引用和资源路径，确认模式边界没有漂移。
3. 对同一状态的所有消费者做一次契约 review。
4. 删除只服务于旧路径的兼容 wrapper、cache 或 fallback，但不要借机迁移无关历史代码。

## Checklist

- [ ] 已搜索现有所有者和相同能力。
- [ ] 没有复制生命周期、网络、解析、资源或配置逻辑。
- [ ] 新抽象有真实第二消费者/实现或外部边界证据。
- [ ] 共享 payload/config 契约只有一个解码/投影 owner。
- [ ] 生成文件和导出产物没有手工修改。
- [ ] 批量修改后已搜索旧/新引用并检查输出路径。

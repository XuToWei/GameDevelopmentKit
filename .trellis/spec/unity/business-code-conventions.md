# Business Code Conventions

适用于 GDK 的 GameHot、ET Model/Hotfix/ModelView/HotfixView 和共享 C# 业务代码。生成的 Luban、Proto2CS、CodeBind 和 ID 文件不手工修改。

## Keep ownership visible

简单逻辑写在真正拥有状态的对象中：

- GameHot：对应 `HotComponent`、EntityData、EntityLogic、UIForm 或 Widget；
- ET：对应 Entity/Component 或其 `EntitySystemOf` 系统；
- ET/GF bridge：`UIComponent`、`GFEntityComponent` 和 `UGFUIForm`/`UGFEntity` owner；
- Editor：已有 `ICommandHandler`、Excel/Luban exporter 或 CodeBind owner。

只有存在真实第二消费者、第二实现、外部边界替换或清晰程序集边界时，才提取最小 interface、gateway、adapter 或 utility。一次 private 方法提取可以改善可读性，但不应制造多层转发。

## C# and layer conventions

- 命名空间、类和文件跟随周围目录与 asmdef；GameHot 使用 `Game.Hot`，ET 按现有 `ET`/`ET.Client`/`ET.Server` 分区。
- ET 数据声明和系统实现分离：实体/组件声明放 Model/ModelView，`EntitySystemOf` 逻辑放 Hotfix/HotfixView。
- UI Mono view 使用 CodeBind 生成引用，系统通过 `self.View` 访问；不要在 HotfixView 重新 `GetComponent` 并复制绑定状态。
- 异步使用项目 `UniTask`/`UniTaskVoid` 约定；不得引入第二套异步抽象。
- 短方法签名、attribute 和调用保持紧凑；仅在嵌套、超长或语义分组时换行。

## State and field comments

新增或修改的业务状态、配置字段、ID、计数、时间、比例、引用和版本信号，应在相邻位置说明来源、身份、单位、特殊值、全量/增量语义或生命周期。

```csharp
/// <summary>
/// 当前实体类型配置 ID；来自对应 Luban Entity 表，不能当作运行时实例 ID。
/// </summary>
private int m_EntityTypeId;
```

纯组件引用、显然的局部变量、CTS 和自动生成字段不需要机械注释。注释不要只是翻译字段名。

## Required data contracts

初始化、配置、Prefab、表格或成功协议建立的必需对象，业务消费点应直接使用，并让错误暴露在源头：

- 必需 Luban 表行使用生成表提供的严格访问；只有业务明确允许缺行时才使用可选查询。
- CodeBind/Inspector 的必需引用直接使用；漏绑应修复 Prefab 或绑定生成。
- 生产资源、UIForm/UIEntity/Entity ID 和路径直接走已有 `AssetUtility`、`GameEntry` 或 owner API。
- 成功且当前的协议 payload 直接进入领域 owner；不构造默认对象/空集合掩盖协议错误。

`?.`、`??`、Try + 默认值、占位资源和静默 `return` 只允许出现在真实可选、用户输入、失败、取消、stale 或生命周期失效边界，并且当前层必须有明确动作。

## Generated code boundary

以下内容由工具产生，不手工编辑：

- `Unity/Assets/Scripts/Game/**/Generate/` 中的 Luban、Proto2CS、UI/Entity/Sound/Scene ID；
- `Unity/Assets/Res/**/Luban/`、Localization `.bytes/.json`；
- CodeBind 的 `*.Bind.cs`；
- Unity/HybridCLR 的装载产物和程序集缓存。

要改变生成结果，修改 `Design/Excel`、`Design/Proto`、`luban.conf`/`proto.conf`、generator 或 Editor 工具，再重新生成并检查 diff。

## Review checklist

- [ ] 代码位置符合模式和 asmdef，业务 owner 清晰。
- [ ] 新抽象有真实使用证据，没有同义 wrapper 或第二状态容器。
- [ ] 业务字段注释包含来源、单位、身份或特殊值等有效信息。
- [ ] 必需表、Prefab、资源和成功协议没有防御性默认化。
- [ ] 只有真实可选/失败/取消/stale/生命周期边界保留判断和 catch。
- [ ] 生成文件未手工修改，源配置和输出 diff 可追溯。

## References

- `Unity/Assets/Scripts/Game/Hot/Code/`
- `Unity/Assets/Scripts/Game/ET/Code/Model/`
- `Unity/Assets/Scripts/Game/ET/Code/ModelView/`
- `Unity/Assets/Scripts/Game/ET/Code/Hotfix/`
- `Unity/Assets/Scripts/Game/ET/Code/HotfixView/`
- `Unity/Assets/Scripts/Game/ET/Code/HotfixView/Client/Demo/UI/UILogin/UIFormLoginComponentSystem.cs`

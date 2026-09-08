# Localization Workflow

## Source and runtime chain

玩家可见文本的权威来源是 `Design/Excel/Localization.xlsx`，不是业务 C#、Prefab 或已生成文件。标准链路为：

```text
Localization.xlsx
  → ExcelExporter_Localization
  → Unity/Assets/Res/Localization/<Language>/Localization.bytes 或 .json
  → LocalizationExtension.LoadLanguageAsync
  → GameEntry.Localization
  → UI
```

`ExcelExporter_Localization` 也生成 `AssetUtility.Localization.cs`、`LocalizationReadyLanguage.cs` 和模式对应的 `LocalizationKey.cs`。这些文件不可手工修改。

## Text rules

- 静态 Prefab 文本优先由 UX/UI 组件的 localization 配置绑定 key；不要在业务代码硬编码玩家语言正文。
- 动态文本使用 `GameEntry.Localization.GetString(key, args)` 或项目已有的本地化扩展，再把最终字符串交给 UI。
- key 使用稳定、可读、按功能分组的英文语义；新增前查重，不用中文正文、行号或随机后缀。
- 完整句式、标点、占位符和富文本标签属于表格契约；代码只传参数，不拼接半句或控制语言词序。
- 各语言必须保留相同的占位符集合，富文本标签闭合且语义一致。
- 日志、协议字段、资源路径、key、本地 UID 和纯数值不因本规范强行翻译。

```csharp
// Good: 代码只提供稳定 key 和参数。
string text = GameEntry.Localization.GetString("Dialog.ConfirmButton");

// Bad: 玩家可见正文硬编码在业务代码。
label.text = "Confirm";
```

## Table and export ownership

目标项目有四种内置语言资源，具体启用语言以 `GameFramework.Localization.Language`、Excel 表头和导出结果为准。不要把示例语言列或 key 当作永远不变的 schema。

修改 Localization.xlsx 时使用目标已有的 AgentBridge `excel` command：先 `list_commands`，再 `inspect`/`find_rows` 查表头、sheet、key 和版本；写入使用 `dryRun`、`expectedVersion` 和 `upsert_rows`/其他运行时发现的动作；写后回读并执行 Luban/ExcelExporter export。不要使用 openpyxl、Excel/WPS UI、手工 ZIP/XML 或直接改 `.bytes`。

`ExcelCommand.cs` 的 schema 与限制以当前运行时 `list_commands` 为准。它支持仓库内已有工作簿的 inspect、范围读取、表头查询和受版本保护的写入，但不能据此假定可创建/删除 sheet 或任意重排结构。命令不支持 Unity Undo；并发冲突必须重新读取、审阅后用新 request ID 重做。

导出后确认：

- `Unity/Assets/Res/Localization/` 的语言产物；
- `Unity/Assets/Scripts/Game/Generate/Localization/AssetUtility.Localization.cs`；
- `Unity/Assets/Scripts/Game/Editor/Generate/Localization/LocalizationReadyLanguage.cs`；
- 对应 GameHot/ET `LocalizationKey.cs`；
- exporter 成功日志、AssetDatabase refresh 和 Unity compile。

## Checklist

- [ ] 新增文案已有稳定 key，且全表无重复。
- [ ] 静态文本使用组件绑定，动态文本使用 `GetString`/现有 localization API。
- [ ] 所有语言占位符和富文本标签契约一致。
- [ ] Excel 修改走 AgentBridge schema discovery、版本保护、dry-run、写后回读。
- [ ] 写入后实际执行 export，未直接改生成产物。
- [ ] 导出和编译完成，无新增错误或无关资源 diff。

## References

- `Design/Excel/Localization.xlsx`
- `Unity/Assets/Scripts/Game/Localization/LocalizationExtension.cs`
- `Unity/Assets/Scripts/Game/Localization/LubanLocalizationHelper.cs`
- `Unity/Assets/Scripts/Game/Generate/Localization/AssetUtility.Localization.cs`
- `Unity/Assets/Scripts/Game/Editor/Generate/Localization/LocalizationReadyLanguage.cs`
- `Unity/Assets/Scripts/Game/Editor/AgentBridge/ExcelCommand.cs`
- `Share/Tool/ExcelExporter/ExcelExporter.Localization.cs`
- `Unity/Assets/Res/Builtin/UGFLocalizationDictionaryEnglish.asset`

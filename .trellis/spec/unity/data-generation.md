# Luban, Proto, and Generated Code

## Source of truth

Excel、Proto 和生成配置是源文件；生成的 C#、JSON/bin、Localization 资源和 ID 只属于派生结果。不得手工修改 `Unity/Assets/Scripts/**/Generate/`、`*.Bind.cs`、`Unity/Assets/Res/**/Luban`、Localization `.bytes/.json` 或 HybridCLR 装载产物。

修改生成结果的顺序是：修改 `Design/Excel`/`Design/Proto`/`luban.conf`/`proto.conf` 或 generator → 构建 `Kit.sln` → 运行相应导出 → 等待 Unity refresh/compile → 检查完整 diff。

## Luban has two configurations

GDK 不使用单一 Luban 工程。`Design/Excel/ET/luban.conf` 和 `Design/Excel/GameHot/luban.conf` 分别服务 ET 与 GameHot，active 状态由当前模式/任务决定，不能假定两份同时或永远启用。

ET 配置的主要 target 和输出：

```text
Game client       → Unity/Assets/Scripts/Game/Generate/Luban
                    Unity/Assets/Res/Luban
ET client         → Unity/Assets/Scripts/Game/ET/Code/Model/Generate/Client/Luban
                    Unity/Assets/Res/ET/Client/Luban
ET clientserver   → Unity/Assets/Scripts/Game/ET/Code/Model/Generate/ClientServer/Luban
                    Unity/Assets/Res/ET/ClientServer/Luban
                    Config/Luban
ET editor         → Unity/Assets/Scripts/Game/ET/Editor/Generate/Luban
```

GameHot 配置的 client/editor target 输出到 `Game/Hot/Code/Generate/Luban`、`Game/Hot/Code/Editor/Generate/Luban` 和相应 `Res/Hot/Luban` 目录。准确 target 以当前 `luban.conf` 为准；不要手写第二份 target 清单。

`ExcelExporter_Luban.DoExport` 会发现 active 配置、并行执行命令、复制多目标输出，并在结束后生成 UI/Entity/Scene/Sound ID。`LubanCommand` 的 editor command 只有 `validate` 和 `export` 两类语义：validate 检查而不写生成结果，export 写结果并安排 AssetDatabase refresh。

## Runtime loading

`Game/Hot/Code/Tables/TablesComponent.Load.cs` 根据生成的 `TablesComponent.LoadAsync` 签名选择 ByteBuf/bin 或 JSON loader。ET 的 Tables 由 ET Config/Loader 负责。资源加载路径必须来自生成的 `AssetUtility` 或当前 Tables owner，不能在业务代码中拼第二套路由。

## Proto2CS

Proto 源码位于 `Design/Proto/` 的直接子目录，每个启用目录有 `proto.conf`。`Share/Tool/Proto2CS/Proto2CS.cs` 递归读取 `.proto`，按路径排序后生成：

| 配置 | 类型 | 起始区间 | 主要输出 |
|---|---|---:|---|
| `ET-Client` | ET/MemoryPack | 10000 | ET Model Client/ClientServer Message |
| `ET-ClientServer` | ET/MemoryPack | 20000 | ET Model ClientServer Message |
| `GameHot` | UGF/Protobuf | 30000 | GameHot Message |
| `ET-Admin` | ET/MemoryPack | 30000 | DotNet Model Message |

实际区间和 active 状态以 `proto.conf` 为准。Proto 文件排序和消息顺序影响 Opcode；已发布消息不得随意插入或重排。不同配置的 opcode 区间不得重叠。

ET 消息由 ET 运行时和对象池约定处理；UGF `CS*`/`SC*` 消息进入 GameHot packet 管线。不要把 ET MemoryPack 消息当作 UGF Packet，也不要把两个模式的生成输出合并。

## UI/Entity/Localization generators

UI、UIEntity、Entity、Scene 和 Sound ID 来自对应 Excel 与 `Share/Tool/ExcelExporter/Generate/`；GameHot 与 ET 有不同生成目录和 ID 类型。Localization 源表为 `Design/Excel/Localization.xlsx`，导出由 `ExcelExporter_Localization` 生成语言资源、`AssetUtility.Localization.cs`、`LocalizationReadyLanguage.cs` 和模式对应的 `LocalizationKey.cs`。

生成异常回到源 Excel/Proto、配置或 generator 修复；不要在生成 C# 或运行时资源上打补丁。

## Verification checklist

- [ ] 已确认本次改动所属 ET/GameHot target 和 `active` 状态。
- [ ] 已构建 `Kit.sln`，再执行 validate/export 或 Proto2CS。
- [ ] 生成目录、资源目录和 `Config/Luban` 的 diff 与源改动一致。
- [ ] 未手工修改生成 `.cs`、`.Bind.cs`、`.bytes`、`.json` 或 ID 文件。
- [ ] Proto 消息顺序、codeType 和 Opcode 区间没有意外变化。
- [ ] Unity refresh/compile 完成且没有新增错误。

## References

- `Design/Excel/ET/luban.conf`
- `Design/Excel/GameHot/luban.conf`
- `Design/Proto/*/proto.conf`
- `Share/Tool/ExcelExporter/ExcelExporter.Luban.cs`
- `Share/Tool/ExcelExporter/ExcelExporter.Localization.cs`
- `Share/Tool/Proto2CS/Proto2CS.cs`
- `Unity/Assets/Scripts/Game/Editor/AgentBridge/LubanCommand.cs`
- `Book/Luban配置.md`
- `Book/Proto生成工具.md`

# 模块 17：Luban 配置表

## 概述
GDK 使用 **Luban** 作为配置方案：Excel 源数据在 `Design/Excel/`，导出工具在 `Share/Tool/ExcelExporter/`，生成代码与二进制/JSON 数据输出到客户端与 Config 目录。支持**多工程并行导出**（ET / GameHot / Localization）。

## 数据流
```
Design/Excel/{ET|GameHot}/Datas/*.xlsx + luban.conf + Defines/builtin.xml
   │  Share/Tool/ExcelExporter/ExcelExporter.Luban.cs（并行执行 dotnet Luban.dll）
   ▼
Unity/Assets/Res/Luban（GF 公共数据，Json/Bytes）
Unity/Assets/Res/ET/Client|ClientServer|Editor（ET 数据）
Unity/Assets/Res/Hot/Luban（GameHot 数据）
Config/Luban（clientserver 复制一份，供独立服务端）
Unity/Assets/Scripts/Game/Generate/Luban（生成代码）
```

## 工程结构（Design/Excel/）
- `ET/`：`luban.conf`、`Defines/builtin.xml`、`Datas/`（`__tables__/__beans__/__enums__` 及业务表）。
- `GameHot/`：同上结构，业务表含 UI/Entity/Scene/Sound 表。
- `Localization.xlsx`：多语言表。
- `gen all bin.bat`：一键导出批处理。

## 导出工具（Share/Tool/ExcelExporter/ExcelExporter.Luban.cs）
- 扫描 `Design/Excel` 直接子目录中 `active=true` 的 `luban.conf`。
- 展开路径变量：`%UNITY_ASSETS%`、`%ROOT%`、`%GEN_CLIENT%` 等。
- `Parallel.ForEachAsync` 并行执行 `dotnet Luban.dll`。
- 支持 `--Customs=Json/Check/ShowCmd/ShowInfo`；多目标目录（逗号分隔）以首目录为源复制到其余。
- 导出后二次生成 `UGFUIFormId/UGFEntityId/UGFSceneId/UGFSoundId` 常量（读 `Res/Editor/Luban` JSON），并导出 `Localization.xlsx`。

## 运行时访问（`Game/Tables/TablesComponent.Load.cs`）
```csharp
// TablesComponent 是 GF 组件（GameEntry.Tables）
public enum TablesLoadType : byte { Undefined, Bytes, Json, Code }

// 加载全部表（按 LoadType 选择 Bytes/Json/内嵌代码）
await GameEntry.Tables.LoadAllAsync();

// 业务访问（生成代码提供强类型属性）
GameEntry.Tables.DTMusic.GetOrDefault(musicId);
GameEntry.Tables.DTScene.GetOrDefault(sceneId);
GameEntry.Tables.DREntity.GetOrDefault(entityId);
```
- `LoadType`：`Code` 表示表数据内嵌在程序集（调试友好）；`Bytes`/`Json` 表示运行时从资源加载。
- 表内存统一由 `TablesMemory` 管理，Awake/OnDestroy 自动清理。

## 模式联动
- 切换 `UNITY_ET` / `UNITY_GAMEHOT` 时自动激活对应 `luban.conf.active`（见模块 02）。
- 多语言表见 `Book/多语言.md`；排错与生成物说明见 `Book/Luban配置.md`。

## 官方文档
- `Book/Luban配置.md`（多工程并行导出、生成物与排错）

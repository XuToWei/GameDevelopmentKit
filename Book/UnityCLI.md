# Unity CLI 自定义命令

本工程通过 Unity CLI 和 `com.unity.pipeline` 调用已打开的 Unity Editor。命令代码位于 `Unity/Assets/Scripts/Game/Editor/Cli/`。

## 环境与编译检查

当前工程使用 Unity `6000.6.7f1`，已包含 `com.unity.pipeline` `0.9.1-exp.1`。Unity CLI 需在本机单独安装，本次验证使用 `1.0.0-beta.15`。先用指定版本的 Editor 打开 `Unity/`，等待包解析与脚本编译完成，再连接 CLI。

升级 Unity 或修改脚本后，可在仓库根目录检查：

```bash
unity --json recompile --project-path ./Unity --timeout 90
unity --json command --project-path ./Unity script recompile_status
```

检查 `script recompile_status` 的返回结果：`status` 为 `completed` 或 `up_to_date`，`failed=false`、`compilationFailed=false`，且 `errors` 为空。`up_to_date` 表示本次没有脚本需要重新编译。编译错误导致 Editor 停留在 Safe Mode 时，先通过 Console 修复错误，等待正常重载后再连接 Pipeline。

## 连接与发现

在仓库根目录执行：

```bash
unity command --project-path ./Unity
unity command --project-path ./Unity game.codebind
unity command --project-path ./Unity game.statecontroller
unity command --project-path ./Unity game.uxtool
unity command --project-path ./Unity game.excel
unity command --project-path ./Unity game.luban
```

不指定动作时，CLI 返回该组的命令、参数、类型和默认值。新增命令需要等待 Unity 编译完成。实际可用接口以连接到 Editor 后的发现结果为准。

命令采用 `unity command <group> <action> --argument value`。CLI 参数使用 snake_case；Excel 的结构化 `input` 字段使用 camelCase。布尔值可明确写为 `--dry_run true`。数组和对象使用单引号包裹的 JSON。

CLI 自身占用 `--format`、`--color`、`--offset`、`--limit` 等选项，所以项目命令分别使用 `--data_format`、`--tint`、`--skip_rows`、`--max_rows`。需要 JSON 输出时，将 `--json` 放在 `command` 前：

```bash
unity --json command --project-path ./Unity game.excel inspect \
  --file_path Design/Excel/GameHot/Datas/Game/UI.xlsx
```

这些命令需独立执行，不放入 `job batch`。磁盘文件修改和 Prefab 保存不具备 Unity Undo 事务回滚能力，命令使用 Pipeline 的 Task 返回值边界阻止加入同步批次。

## CodeBind

| 动作 | 用途 |
| --- | --- |
| `all` | 生成绑定代码并尝试设置引用；新字段尚未编译时返回 `serializationDeferred=true` |
| `generate_code` | 生成 `*.Bind.cs` |
| `set_serialization` | 设置已经编译的序列化引用 |
| `rename_node` | 将节点改为 CodeBind 绑定名 |

共同目标参数为 `--asset_path`（Prefab）、`--instance_id`（场景对象实例 ID）、`--path`（场景层级路径）。优先级依次为 asset_path、instance_id、path；全部省略时使用当前选择。层级查找包含未激活对象；重名导致路径有歧义时使用实例 ID。

```bash
unity command --project-path ./Unity game.codebind generate_code \
  --asset_path Assets/Res/UI/UIForm/Hot/TestForm.prefab

unity command --project-path ./Unity script recompile
unity command --project-path ./Unity script recompile_status

# 等待编译完成且无错误后执行
unity command --project-path ./Unity game.codebind set_serialization \
  --asset_path Assets/Res/UI/UIForm/Hot/TestForm.prefab

unity command --project-path ./Unity game.codebind rename_node \
  --asset_path Assets/Res/UI/UIForm/Hot/TestForm.prefab \
  --node_path Content/Button \
  --bind_name ConfirmButton
```

`rename_node` 另支持 `--array_index`（非负数组索引）和 `--separator`（单字符，默认读取 CodeBind 编辑器设置）。`node_path` 相对于目标根节点，省略时操作根节点。Prefab 修改自动保存，场景对象修改标记 dirty。

## StateController

| 动作 | 必填业务参数 | 可选业务参数 |
| --- | --- | --- |
| `list` | 无 | 无 |
| `list_nodes` | 无 | `group_name`、`node_path` |
| `set_state` | `group_name` | `state_name` 与 `state_index` 必须且只能传一个；`controller_path` 用于消歧 |
| `add_group` | `group_name` | `states`（JSON 字符串数组）、`controller_path` |
| `add_state` | `group_name`、`state_name` | `controller_path` |
| `add_state_node` | `group_name`、`state_type` | `node_path` |

所有动作均支持与 CodeBind 相同的目标参数。原接口的 `dataName` 在 CLI 中统一称为 `group_name`，新增组的动作称为 `add_group`。

```bash
unity command --project-path ./Unity game.statecontroller list \
  --asset_path Assets/Res/UI/UIForm/Hot/TestForm.prefab

unity command --project-path ./Unity game.statecontroller add_group \
  --asset_path Assets/Res/UI/UIForm/Hot/TestForm.prefab \
  --group_name Tab --states '["Normal","Selected","Disabled"]'

unity command --project-path ./Unity game.statecontroller set_state \
  --asset_path Assets/Res/UI/UIForm/Hot/TestForm.prefab \
  --group_name Tab --state_name Selected
```

Prefab 写操作自动保存；场景对象写操作标记 dirty。添加状态时会对齐对应绑定节点的状态槽。

## UXTool

```bash
unity command --project-path ./Unity game.uxtool add_background \
  --design_image Assets/Res/UI/Design/Login.png \
  --tint 1,1,1,0.5
```

`design_image` 和 `tint` 均可省略。颜色格式为 `r,g,b` 或 `r,g,b,a`，每个分量必须在 0..1 范围内。该命令调用 UXTool QuickBackground 创建临时设计稿背景，用于编辑预览。

## Excel

| 动作 | 输入形式 |
| --- | --- |
| `inspect` | `--file_path` |
| `read_range` | `--file_path`、`--sheet`、`--range`，以及可选读取参数 |
| `find_rows` | `--file_path`、`--sheet`、`--where`（JSON 对象），以及可选筛选、分页参数 |
| `set_cells` | `--input`（结构化对象）和 `--dry_run` |
| `upsert_rows` | `--input`（结构化对象）和 `--dry_run` |

```bash
unity command --project-path ./Unity game.excel read_range \
  --file_path Design/Excel/GameHot/Datas/Game/UI.xlsx \
  --sheet UI界面 --range A1:H10

unity command --project-path ./Unity game.excel find_rows \
  --file_path Design/Excel/GameHot/Datas/Game/UI.xlsx \
  --sheet UI界面 --where '{"CSName":"UIHelp"}' \
  --select '["Id","CSName","AssetName"]' --skip_rows 0 --max_rows 20

unity command --project-path ./Unity game.excel set_cells \
  --input '{
    "filePath":"Design/Excel/GameHot/Datas/Game/UI.xlsx",
    "sheet":"UI界面",
    "cells":[{"cell":"D4","value":"新备注","expectedValue":"帮助界面"}]
  }' --dry_run true

unity command --project-path ./Unity game.excel upsert_rows \
  --input '{
    "filePath":"Design/Excel/GameHot/Datas/Game/UI.xlsx",
    "sheet":"UI界面",
    "keyColumns":["Id"],
    "rows":[{"Id":999,"CSName":"UITest","Desc":"测试界面","AssetName":"Hot/UITest"}]
  }' --dry_run true
```

先用 `inspect` 或查询获取 SHA-256 `version`，正式写入时在 `input` 中回传 `expectedVersion`；`set_cells` 也支持 `expectedValue` / `expectedFormula` 校验旧值。预览使用 `--dry_run true`，实际写入省略此参数或传 `false`。`value:null` 清空单元格；`formula` 与 `value` 二选一。

仅访问仓库内已有的 `.xlsx` / `.xls` 和已有工作表；写入使用同目录临时文件替换。`upsert_rows` 保留模板样式、公式和数据验证复制能力。Excel 命令只修改源表，不自动导出 Luban，详情见 [Luban 配置](Luban配置.md)。

## Luban

先编译 `Kit.sln`，生成 `Bin/Tool.exe` 或 `Bin/Tool.dll`：

```bash
dotnet build Kit.sln

unity command --project-path ./Unity game.luban validate --data_format bin
unity command --project-path ./Unity game.luban export --data_format bin
```

`data_format` 为枚举，支持 `Bin` / `Json`，调用时不区分大小写。`validate` 只校验，不写生成产物；`export` 生成代码和数据，并安排 AssetDatabase 刷新及 UXTool 本地化缓存清理。默认超时 600 秒，可通过 `--timeout` 调整。

导出或生成源码后可能触发 domain reload。等待 `script recompile_status` 报告完成且无编译错误后，再运行依赖新代码的命令；重载期间允许短暂连接失败。

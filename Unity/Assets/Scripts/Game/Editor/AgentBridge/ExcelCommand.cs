using System;
using System.Threading.Tasks;
using AgentBridge;
using Newtonsoft.Json.Linq;

namespace Game.Editor
{
    /// <summary>
    /// Unity Agent Bridge command for inspecting, querying, and updating repository Excel workbooks.
    /// </summary>
    public sealed partial class ExcelCommand : ICommandHandler
    {
        private const string ErrorCode = "EXCEL_ERROR";
        private const string ConflictErrorCode = "EXCEL_CONFLICT";

        public string Command => "excel";
        public string Description => "Excel 工具：action=inspect/read_range/find_rows/set_cells/upsert_rows。支持查看工作簿结构、按范围读取、按表头查找、带 dryRun/版本与旧值校验的批量改单元格，以及按键列更新或追加整行。仅访问仓库内已有 .xlsx/.xls；写入使用同目录临时文件替换，无法通过 Unity Undo 撤销。";
        public string Group => "Game";
        public bool CanDisable => true;
        public CommandBatchMode BatchMode => CommandBatchMode.NotAllowed;

        // AgentBridge's Task<object> contract requires an async method builder, while ET0501
        // normally forbids non-UniTask async methods in project code.
        public Task<object> ExecuteAsync(JObject @params)
        {
            string action = GetString(@params, "action", "set_cells").ToLowerInvariant();
            switch (action)
            {
                case "inspect":
                    return Task.FromResult(Inspect(@params));
                case "read_range":
                    return Task.FromResult(ReadRange(@params));
                case "find_rows":
                    return Task.FromResult(FindRows(@params));
                case "set_cells":
                    return Task.FromResult(SetCells(@params));
                case "upsert_rows":
                    return Task.FromResult(UpsertRows(@params));
                default:
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Unknown action: {action}. Supported: inspect, read_range, find_rows, set_cells, upsert_rows");
            }
        }

        public JObject ParamsSchema { get; } = JObject.Parse(@"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""action"": {
      ""type"": ""string"",
      ""default"": ""set_cells"",
      ""enum"": [""inspect"", ""read_range"", ""find_rows"", ""set_cells"", ""upsert_rows""],
      ""description"": ""inspect 查看工作簿；read_range 读取范围；find_rows 按表头查询；set_cells 修改 A1 单元格；upsert_rows 按键列更新或追加整行。""
    },
    ""filePath"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""maxLength"": 2048,
      ""description"": ""仓库根目录下的 .xlsx/.xls 路径，例如 Design/Excel/GameHot/Datas/Game/UI.xlsx；也接受位于本仓库内的绝对路径。所有 action 必填。""
    },
    ""sheet"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""maxLength"": 31,
      ""description"": ""已有工作表名称；除 inspect 外必填。""
    },
    ""range"": {
      ""type"": ""string"",
      ""pattern"": ""^[A-Za-z]{1,3}[1-9][0-9]{0,6}(:[A-Za-z]{1,3}[1-9][0-9]{0,6})?$"",
      ""description"": ""read_range 必填，例如 A1:H30 或 B4。""
    },
    ""includeEmpty"": {
      ""type"": ""boolean"",
      ""default"": true,
      ""description"": ""read_range 是否返回空单元格，默认 true。""
    },
    ""maxCells"": {
      ""type"": ""integer"",
      ""minimum"": 1,
      ""maximum"": 2000,
      ""default"": 500,
      ""description"": ""read_range 允许读取的最大单元格数量，默认 500、最大 2000。""
    },
    ""headerRow"": {
      ""type"": ""integer"",
      ""minimum"": 1,
      ""default"": 1,
      ""description"": ""find_rows/upsert_rows 的表头行，使用 Excel 1 基行号，默认第 1 行。""
    },
    ""dataStartRow"": {
      ""type"": ""integer"",
      ""minimum"": 1,
      ""description"": ""find_rows/upsert_rows 的首个数据行。省略时自动识别 Luban ##var/##type/## 表头，否则使用 headerRow+1。""
    },
    ""endRow"": {
      ""type"": ""integer"",
      ""minimum"": 1,
      ""description"": ""find_rows 可选的最后扫描行；默认扫描到工作表最后一行。""
    },
    ""where"": {
      ""type"": ""object"",
      ""description"": ""find_rows 必填。属性名必须是表头，属性值是要匹配的常量；多个条件同时满足才返回。"",
      ""additionalProperties"": {
        ""anyOf"": [
          { ""type"": ""string"", ""maxLength"": 32767 },
          { ""type"": ""number"" },
          { ""type"": ""boolean"" },
          { ""type"": ""null"" }
        ]
      }
    },
    ""select"": {
      ""type"": ""array"",
      ""minItems"": 1,
      ""maxItems"": 100,
      ""items"": { ""type"": ""string"", ""minLength"": 1 },
      ""description"": ""find_rows 返回的表头列；省略时返回全部非空表头列。""
    },
    ""match"": {
      ""type"": ""string"",
      ""default"": ""exact"",
      ""enum"": [""exact"", ""contains""],
      ""description"": ""find_rows 匹配方式，默认 exact。contains 按单元格显示文本包含匹配。""
    },
    ""ignoreCase"": {
      ""type"": ""boolean"",
      ""default"": false,
      ""description"": ""find_rows 字符串匹配是否忽略大小写。""
    },
    ""offset"": {
      ""type"": ""integer"",
      ""minimum"": 0,
      ""default"": 0,
      ""description"": ""find_rows 分页偏移。""
    },
    ""limit"": {
      ""type"": ""integer"",
      ""minimum"": 1,
      ""maximum"": 200,
      ""default"": 50,
      ""description"": ""find_rows 最大返回行数，默认 50、最大 200。""
    },
    ""cells"": {
      ""type"": ""array"",
      ""minItems"": 1,
      ""maxItems"": 500,
      ""description"": ""set_cells 必填；同一请求中不能重复地址。"",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {
          ""cell"": {
            ""type"": ""string"",
            ""pattern"": ""^[A-Za-z]{1,3}[1-9][0-9]{0,6}$"",
            ""description"": ""A1 地址，例如 B4、AA12。""
          },
          ""value"": {
            ""description"": ""写入常量；null 清空。与 formula 二选一。"",
            ""anyOf"": [
              { ""type"": ""string"", ""maxLength"": 32767 },
              { ""type"": ""number"" },
              { ""type"": ""boolean"" },
              { ""type"": ""null"" }
            ]
          },
          ""formula"": {
            ""type"": ""string"",
            ""minLength"": 1,
            ""maxLength"": 8192,
            ""description"": ""写入公式，可带或不带开头的 =。与 value 二选一。""
          },
          ""expectedValue"": {
            ""description"": ""可选。写入前要求单元格当前常量/公式缓存值等于它；不符返回 EXCEL_CONFLICT。"",
            ""anyOf"": [
              { ""type"": ""string"", ""maxLength"": 32767 },
              { ""type"": ""number"" },
              { ""type"": ""boolean"" },
              { ""type"": ""null"" }
            ]
          },
          ""expectedFormula"": {
            ""type"": ""string"",
            ""minLength"": 1,
            ""maxLength"": 8192,
            ""description"": ""可选。写入前要求当前公式相同；可带或不带开头的 =。不能与 expectedValue 同时使用。""
          }
        },
        ""required"": [""cell""],
        ""oneOf"": [
          {
            ""required"": [""value""],
            ""not"": { ""required"": [""formula""] }
          },
          {
            ""required"": [""formula""],
            ""not"": { ""required"": [""value""] }
          }
        ]
      }
    },
    ""dryRun"": {
      ""type"": ""boolean"",
      ""default"": false,
      ""description"": ""set_cells/upsert_rows 只计算并返回变化，不保存工作簿。""
    },
    ""expectedVersion"": {
      ""type"": ""string"",
      ""pattern"": ""^[A-Fa-f0-9]{64}$"",
      ""description"": ""set_cells/upsert_rows 可选。必须等于 inspect/read/find 返回的 SHA-256 version，否则返回 EXCEL_CONFLICT。""
    },
    ""keyColumns"": {
      ""type"": ""array"",
      ""minItems"": 1,
      ""maxItems"": 8,
      ""items"": { ""type"": ""string"", ""minLength"": 1 },
      ""description"": ""upsert_rows 必填。用于唯一定位行的表头列，例如 [Id] 或 [Id, CSName]。""
    },
    ""rows"": {
      ""type"": ""array"",
      ""minItems"": 1,
      ""maxItems"": 200,
      ""description"": ""upsert_rows 必填。每个对象的属性名必须是表头；值可为常量，或包含 formula 属性的对象。"",
      ""items"": {
        ""type"": ""object"",
        ""additionalProperties"": {
          ""anyOf"": [
            { ""type"": ""string"", ""maxLength"": 32767 },
            { ""type"": ""number"" },
            { ""type"": ""boolean"" },
            { ""type"": ""null"" },
            {
              ""type"": ""object"",
              ""additionalProperties"": false,
              ""properties"": {
                ""formula"": { ""type"": ""string"", ""minLength"": 1, ""maxLength"": 8192 }
              },
              ""required"": [""formula""]
            }
          ]
        }
      }
    },
    ""templateRow"": {
      ""type"": ""integer"",
      ""minimum"": 1,
      ""description"": ""upsert_rows 新增行时用于复制样式、公式和数据验证的模板行；省略时使用最后一个数据行。""
    },
    ""copyTemplate"": {
      ""type"": ""boolean"",
      ""default"": true,
      ""description"": ""upsert_rows 新增行时是否复制模板行的样式、公式与数据验证，默认 true；模板常量会被清空。""
    }
  },
  ""required"": [""filePath""]
}");
    }
}

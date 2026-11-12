using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Pipeline.Commands;

namespace Game.Editor
{
    /// <summary>
    /// Unity CLI commands for inspecting, querying, and updating repository Excel workbooks.
    /// </summary>
    public static partial class ExcelCommand
    {
        private const string Group = "game.excel";
        private const string ErrorCode = "EXCEL_ERROR";
        private const string ConflictErrorCode = "EXCEL_CONFLICT";

        // The CLI Task boundary retains the previous no-batch policy: job batch refuses Task
        // handlers, and external workbook writes cannot participate in Unity Undo transactions.
#pragma warning disable ET0501
        [CliGroupCommand(Group, "Inspect an existing repository .xlsx/.xls workbook and return its SHA-256 version.")]
        public static Task<object> Inspect(
            [CliArg("file_path", "Workbook path relative to the repository root, or an absolute path inside it.", Required = true)] string filePath)
        {
            return Task.FromResult(Inspect(new JObject { ["filePath"] = filePath }));
        }

        [CliGroupCommand(Group, "Read an A1 range from an existing repository workbook, including values and formulas.")]
        public static Task<object> ReadRange(
            [CliArg("file_path", "Workbook path inside the repository.", Required = true)] string filePath,
            [CliArg("sheet", "Existing worksheet name (1..31 characters).", Required = true)] string sheet,
            [CliArg("range", "A1 address or range, for example B4 or A1:H30.", Required = true)] string range,
            [CliArg("include_empty", "Include blank cells.")] bool includeEmpty = true,
            [CliArg("max_cells", "Maximum requested cells (1..2000). Default: 500.")] int maxCells = 500)
        {
            return Task.FromResult(ReadRange(new JObject
            {
                ["filePath"] = filePath,
                ["sheet"] = sheet,
                ["range"] = range,
                ["includeEmpty"] = includeEmpty,
                ["maxCells"] = maxCells
            }));
        }

        [CliGroupCommand(Group, "Find rows by header names and constant filters; returns values, formulas, and the workbook version.")]
        public static Task<object> FindRows(
            [CliArg("file_path", "Workbook path inside the repository.", Required = true)] string filePath,
            [CliArg("sheet", "Existing worksheet name (1..31 characters).", Required = true)] string sheet,
            [CliArg("where", "Non-empty JSON object mapping header names to string, number, boolean, or null filters; all conditions must match.", Required = true)] string where,
            [CliArg("header_row", "Excel 1-based header row.")] int headerRow = 1,
            [CliArg("data_start_row", "First data row; omitted = detect Luban headers or use header_row + 1.")] int? dataStartRow = null,
            [CliArg("end_row", "Last row to scan; omitted = last worksheet row. At most 100000 rows may be scanned.")] int? endRow = null,
            [CliArg("select", "JSON array of 1..100 header names; omitted = all named data columns.")] string select = null,
            [CliArg("match", "Match exact values or contains in displayed cell text.")] ExcelMatchMode match = ExcelMatchMode.Exact,
            [CliArg("ignore_case", "Ignore case when matching strings.")] bool ignoreCase = false,
            [CliArg("skip_rows", "Number of matching rows to skip (>= 0).")] int offset = 0,
            [CliArg("max_rows", "Maximum returned rows (1..200). Default: 50.")] int limit = 50)
        {
            if (!Enum.IsDefined(typeof(ExcelMatchMode), match))
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"Unknown match mode '{match}'. Supported: exact, contains.");
            }
            if (dataStartRow.HasValue && dataStartRow.Value < 1 ||
                endRow.HasValue && endRow.Value < 1)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    "data_start_row and end_row must be >= 1 when supplied.");
            }

            var parameters = new JObject
            {
                ["filePath"] = filePath,
                ["sheet"] = sheet,
                ["where"] = ParseJsonArgument<JObject>(where, "where"),
                ["headerRow"] = headerRow,
                ["match"] = match.ToString().ToLowerInvariant(),
                ["ignoreCase"] = ignoreCase,
                ["offset"] = offset,
                ["limit"] = limit
            };
            if (dataStartRow.HasValue)
            {
                parameters["dataStartRow"] = dataStartRow.Value;
            }
            if (endRow.HasValue)
            {
                parameters["endRow"] = endRow.Value;
            }
            if (select != null)
            {
                parameters["select"] = ParseJsonArgument<JArray>(select, "select");
            }
            return Task.FromResult(FindRows(parameters));
        }

        [CliGroupCommand(Group, "Set up to 500 cells using --input JSON. dry_run previews changes; expectedVersion and old-value checks guard writes. Writes are atomic and cannot be undone via Unity Undo.")]
        public static Task<object> SetCells(
            [CliArg("input", "Cell batch with camelCase fields: filePath, sheet, cells, expectedVersion.", Required = true)] ExcelSetCellsInput input,
            [CliArg("dry_run", "Preview without saving; overrides input.dryRun when supplied. Default: false.")] bool? dryRun = null)
        {
            return Task.FromResult(SetCells(WriteArguments(input, false, dryRun)));
        }

        [CliGroupCommand(Group, "Update or append up to 200 keyed rows using --input JSON. Supports dry_run, expectedVersion, and copying template styles, formulas, and validation. Writes are atomic and cannot be undone via Unity Undo.")]
        public static Task<object> UpsertRows(
            [CliArg("input", "Row batch with camelCase fields: filePath, sheet, keyColumns, rows, headerRow, dataStartRow, templateRow, copyTemplate, expectedVersion.", Required = true)] ExcelUpsertRowsInput input,
            [CliArg("dry_run", "Preview without saving; overrides input.dryRun when supplied. Default: false.")] bool? dryRun = null)
        {
            return Task.FromResult(UpsertRows(WriteArguments(input, true, dryRun)));
        }

#pragma warning restore ET0501

        private static T ParseJsonArgument<T>(string json, string name) where T : JContainer
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    T result = JsonConvert.DeserializeObject<T>(json,
                        new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (JsonException ex)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"Invalid JSON for '{name}': {ex.Message}");
                }
            }
            throw new GameCommandException(GameCommandException.InvalidParams,
                $"'{name}' must be a JSON {(typeof(T) == typeof(JObject) ? "object" : "array")}.");
        }

        private static JObject WriteArguments(ExcelWriteInput input, bool upsert, bool? dryRun)
        {
            if (input == null)
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Missing 'input' object.");
            }
            var serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            JObject parameters = JObject.FromObject(input, serializer);
            if (dryRun.HasValue)
            {
                parameters["dryRun"] = dryRun.Value;
            }
            ValidateWriteInput(parameters, upsert);
            return parameters;
        }

        // CLI DTO conversion is permissive about scalar types and unknown members. Validate the
        // original JSON before populating the DTO so the former schema's protections are retained.
        internal static void ValidateWriteInput(JObject input, bool upsert)
        {
            if (upsert)
            {
                RequireInputProperties(input, "filePath", "sheet", "dryRun", "expectedVersion",
                    "keyColumns", "rows", "headerRow", "dataStartRow", "templateRow", "copyTemplate");
            }
            else
            {
                RequireInputProperties(input, "filePath", "sheet", "dryRun", "expectedVersion", "cells");
            }
            RequireInputString(input, "filePath", true, 1, 2048);
            RequireInputString(input, "sheet", true, 1, 31);
            ValidateInputBoolean(input, "dryRun");
            if (HasParam(input, "expectedVersion"))
            {
                RequireInputString(input, "expectedVersion", true, 64, 64);
                ValidateExpectedVersionFormat(input["expectedVersion"].Value<string>());
            }

            if (!upsert)
            {
                JArray cells = RequireInputArray(input, "cells", 500);
                foreach (JToken item in cells)
                {
                    if (!(item is JObject cell))
                    {
                        throw new GameCommandException(GameCommandException.InvalidParams,
                            "Every cells item must be an object.");
                    }
                    RequireInputProperties(cell, "cell", "value", "formula", "expectedValue", "expectedFormula");
                    RequireInputString(cell, "cell", true, 1, 10);
                    ValidateInputFormula(cell, "formula");
                    ValidateInputFormula(cell, "expectedFormula");
                    if (HasParam(cell, "value"))
                    {
                        ValidateLiteral(cell["value"], "value");
                    }
                    if (HasParam(cell, "expectedValue"))
                    {
                        ValidateLiteral(cell["expectedValue"], "expectedValue");
                    }
                }
                return;
            }

            ValidateInputBoolean(input, "copyTemplate");
            ValidateInputRowNumber(input, "headerRow");
            ValidateInputRowNumber(input, "dataStartRow");
            ValidateInputRowNumber(input, "templateRow");
            ValidateColumnNames(RequireInputArray(input, "keyColumns", 8), "keyColumns", 8);
            JArray rows = RequireInputArray(input, "rows", 200);
            foreach (JToken item in rows)
            {
                if (!(item is JObject row) || !row.HasValues)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        "Every upsert_rows row must be a non-empty object.");
                }
                foreach (JProperty property in row.Properties())
                {
                    if (property.Value is JObject formula)
                    {
                        RequireInputProperties(formula, "formula");
                        RequireInputString(formula, "formula", true, 1, 8192);
                    }
                    else
                    {
                        ValidateLiteral(property.Value, property.Name);
                    }
                }
            }
        }

        private static void RequireInputProperties(JObject input, params string[] allowed)
        {
            foreach (JProperty property in input.Properties())
            {
                if (Array.IndexOf(allowed, property.Name) < 0)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"Unknown input field '{property.Name}'. Field names are case-sensitive.");
                }
            }
        }

        private static void RequireInputString(JObject input, string name, bool required,
            int minimumLength, int maximumLength)
        {
            if (!HasParam(input, name) && !required)
            {
                return;
            }
            JToken token = input[name];
            if (token == null || token.Type != JTokenType.String ||
                token.Value<string>().Length < minimumLength || token.Value<string>().Length > maximumLength)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"'{name}' must be a string of {minimumLength}..{maximumLength} characters.");
            }
        }

        private static void ValidateInputFormula(JObject input, string name)
        {
            RequireInputString(input, name, false, 1, 8192);
        }

        private static JArray RequireInputArray(JObject input, string name, int maximumCount)
        {
            if (!(input[name] is JArray array) || array.Count < 1 || array.Count > maximumCount)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"'{name}' must contain 1..{maximumCount} items.");
            }
            return array;
        }

        private static void ValidateInputBoolean(JObject input, string name)
        {
            if (HasParam(input, name) && input[name].Type != JTokenType.Boolean)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"'{name}' must be a boolean.");
            }
        }

        private static void ValidateInputRowNumber(JObject input, string name)
        {
            if (HasParam(input, name) &&
                (input[name].Type != JTokenType.Integer ||
                 !int.TryParse(input[name].ToString(Formatting.None), NumberStyles.None,
                     CultureInfo.InvariantCulture, out int value) || value < 1))
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"'{name}' must be a positive 1-based row number.");
            }
        }

        private static void ValidateColumnNames(JArray columns, string name, int maximumCount)
        {
            if (columns.Count < 1 || columns.Count > maximumCount)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"'{name}' must contain 1..{maximumCount} column names.");
            }
            foreach (JToken column in columns)
            {
                if (column.Type != JTokenType.String || string.IsNullOrEmpty(column.Value<string>()))
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"Every '{name}' column must be a non-empty string.");
                }
            }
        }

        private static void ValidateLiteral(JToken value, string name)
        {
            switch (value?.Type)
            {
                case JTokenType.Null:
                case JTokenType.Boolean:
                    return;
                case JTokenType.String:
                    if (value.Value<string>().Length <= 32767)
                    {
                        return;
                    }
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                    if (TryGetDouble(((JValue)value).Value, out _))
                    {
                        return;
                    }
                    break;
            }
            throw new GameCommandException(GameCommandException.InvalidParams,
                $"'{name}' must be a string (at most 32767 characters), finite number, boolean, or null.");
        }
    }

    public enum ExcelMatchMode
    {
        Exact,
        Contains
    }
}

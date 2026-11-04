using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Pipeline.Commands;

namespace Game.Editor
{
    public abstract class ExcelWriteInput : IStructuredCommandInput
    {
        [CliArg("filePath", "Existing .xlsx/.xls path inside the repository (1..2048 characters).", Required = true)]
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [CliArg("sheet", "Existing worksheet name (1..31 characters).", Required = true)]
        [JsonProperty("sheet")]
        public string Sheet { get; set; }

        [CliArg("dryRun", "Preview changes without saving the workbook. Default: false.")]
        [JsonProperty("dryRun")]
        public bool DryRun { get; set; }

        [CliArg("expectedVersion", "Optional 64-hex SHA-256 version returned by inspect, read_range, or find_rows.")]
        [JsonProperty("expectedVersion")]
        public string ExpectedVersion { get; set; }
    }

    [JsonConverter(typeof(ExcelWriteInputConverter))]
    public sealed class ExcelSetCellsInput : ExcelWriteInput
    {
        [CliArg("cells", "1..500 unique A1 cell changes.", Required = true)]
        [JsonProperty("cells")]
        public List<ExcelCellInput> Cells { get; set; }
    }

    public sealed class ExcelCellInput : IStructuredCommandInput
    {
        [CliArg("cell", "A1 address, for example B4 or AA12.", Required = true)]
        [JsonProperty("cell")]
        public string Cell { get; set; }

        // CLR null means omitted; JValue.CreateNull() means an explicit JSON null (clear/check blank).
        [CliArg("value", "Constant string (<=32767 characters), finite number, boolean, or null to clear; use exactly one of value/formula.")]
        [JsonProperty("value", NullValueHandling = NullValueHandling.Include)]
        public JToken Value { get; set; }

        [CliArg("formula", "Formula (1..8192 characters), with or without '='; mutually exclusive with value.")]
        [JsonProperty("formula")]
        public string Formula { get; set; }

        [CliArg("expectedValue", "Require the previous constant/cached formula value; null checks blank. Mutually exclusive with expectedFormula.")]
        [JsonProperty("expectedValue", NullValueHandling = NullValueHandling.Include)]
        public JToken ExpectedValue { get; set; }

        [CliArg("expectedFormula", "Require the previous formula (1..8192 characters), with or without '='.")]
        [JsonProperty("expectedFormula")]
        public string ExpectedFormula { get; set; }

        public bool ShouldSerializeValue() => Value != null;
        public bool ShouldSerializeExpectedValue() => ExpectedValue != null;
    }

    [JsonConverter(typeof(ExcelWriteInputConverter))]
    public sealed class ExcelUpsertRowsInput : ExcelWriteInput
    {
        [CliArg("keyColumns", "1..8 distinct header names identifying each row.", Required = true)]
        [JsonProperty("keyColumns")]
        public List<string> KeyColumns { get; set; }

        [CliArg("rows", "1..200 objects keyed by dynamic header names. Values are constants or {\"formula\":\"...\"}; every key column needs a non-null constant.", Required = true)]
        [JsonProperty("rows")]
        public List<JObject> Rows { get; set; }

        [CliArg("headerRow", "Excel 1-based header row. Default: 1.")]
        [JsonProperty("headerRow")]
        public int HeaderRow { get; set; } = 1;

        [CliArg("dataStartRow", "First data row; omitted = detect Luban headers or use headerRow + 1.")]
        [JsonProperty("dataStartRow")]
        public int? DataStartRow { get; set; }

        [CliArg("templateRow", "Template for inserted rows; omitted = last existing data row.")]
        [JsonProperty("templateRow")]
        public int? TemplateRow { get; set; }

        [CliArg("copyTemplate", "Copy template styles, formulas, and data validation; clear its constants. Default: true.")]
        [JsonProperty("copyTemplate")]
        public bool CopyTemplate { get; set; } = true;
    }

    // Newtonsoft otherwise coerces invalid scalar types and drops unknown fields before the
    // handler sees them. Validate the original JSON before populating these typed inputs.
    public sealed class ExcelWriteInputConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) =>
            objectType == typeof(ExcelSetCellsInput) || objectType == typeof(ExcelUpsertRowsInput);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    "'input' must be a JSON object.");
            }
            var input = JObject.Load(reader, new JsonLoadSettings
            {
                DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
            });
            ExcelCommand.ValidateWriteInput(input, objectType == typeof(ExcelUpsertRowsInput));
            object result = Activator.CreateInstance(objectType);
            using (JsonReader inputReader = input.CreateReader())
            {
                serializer.Populate(inputReader, result);
            }
            if (result is ExcelSetCellsInput setCells)
            {
                // Retain property presence even when Newtonsoft maps JSON null to CLR null.
                var cells = (JArray)input["cells"];
                for (int index = 0; index < cells.Count; index++)
                {
                    var cell = (JObject)cells[index];
                    setCells.Cells[index].Value = cell.Property("value", StringComparison.Ordinal)?.Value;
                    setCells.Cells[index].ExpectedValue = cell.Property("expectedValue", StringComparison.Ordinal)?.Value;
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}

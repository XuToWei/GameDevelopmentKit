using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AgentBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Game.Editor
{
    public sealed partial class ExcelCommand
    {
        private static object SetCells(JObject @params)
        {
            WorkbookPathInfo path = ResolveWorkbookPath(GetString(@params, "filePath", null));
            string sheetName = RequireString(@params, "sheet");
            bool dryRun = GetBool(@params, "dryRun", false);
            List<CellChange> changes = ParseCellChanges(@params?["cells"] as JArray, path);
            IWorkbook workbook = null;

            try
            {
                string versionBefore = GetFileVersion(path.FullPath);
                ValidateExpectedVersion(@params, versionBefore);
                workbook = LoadWorkbook(path);
                ISheet sheet = RequireSheet(workbook, sheetName, path.RelativePath);
                var results = new JArray();
                int changedCount = 0;
                int estimatedCharacters = 0;

                foreach (CellChange change in changes)
                {
                    IRow row = sheet.GetRow(change.RowIndex);
                    ICell cell = row?.GetCell(change.ColumnIndex);
                    CellSnapshot previous = ReadCell(cell);
                    ValidateExpectedCell(change, cell, previous);

                    bool changed = change.Formula != null
                        ? !CellHasFormula(cell, change.Formula)
                        : !CellHasLiteralValue(cell, change.Value);
                    if (changed)
                    {
                        row ??= sheet.CreateRow(change.RowIndex);
                        cell ??= row.CreateCell(change.ColumnIndex);
                        try
                        {
                            if (change.Formula != null)
                            {
                                cell.SetCellFormula(change.Formula);
                            }
                            else
                            {
                                SetCellValue(cell, change.Value);
                            }
                        }
                        catch (CommandException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            throw new CommandException(ErrorCodes.InvalidParams,
                                $"Cannot set cell {change.Address}: {ex.Message}");
                        }
                        changedCount++;
                    }

                    CellSnapshot current = changed ? ReadCell(cell) : previous;
                    estimatedCharacters += EstimateSnapshotCharacters(previous) +
                                           EstimateSnapshotCharacters(current);
                    if (estimatedCharacters > MaxQueryResponseCharacters)
                    {
                        throw new CommandException(ErrorCodes.InvalidParams,
                            "set_cells response would be too large. Split the changes into smaller requests.");
                    }
                    results.Add(new JObject
                    {
                        ["cell"] = change.Address,
                        ["changed"] = changed,
                        ["previous"] = SnapshotResult(previous),
                        ["current"] = SnapshotResult(current)
                    });
                }

                if (changedCount > 0)
                {
                    sheet.ForceFormulaRecalculation = true;
                }

                string versionAfter = versionBefore;
                EnsureFileUnchanged(path.FullPath, versionBefore);
                if (!dryRun && changedCount > 0)
                {
                    SaveWorkbook(workbook, path.FullPath);
                    versionAfter = GetFileVersion(path.FullPath);
                }

                return new
                {
                    action = "set_cells",
                    filePath = path.RelativePath,
                    sheet = sheetName,
                    dryRun,
                    saved = !dryRun && changedCount > 0,
                    versionBefore,
                    versionAfter,
                    requestedCount = changes.Count,
                    changedCount,
                    cells = results
                };
            }
            catch (CommandException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot update '{path.RelativePath}'. Close the workbook in Excel and try again. {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot update '{path.RelativePath}': {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode,
                    $"Failed to update '{path.RelativePath}': {ex.Message}");
            }
            finally
            {
                (workbook as IDisposable)?.Dispose();
            }
        }

        private static object UpsertRows(JObject @params)
        {
            WorkbookPathInfo path = ResolveWorkbookPath(GetString(@params, "filePath", null));
            string sheetName = RequireString(@params, "sheet");
            bool dryRun = GetBool(@params, "dryRun", false);
            bool copyTemplate = GetBool(@params, "copyTemplate", true);
            JArray requestedRows = @params?["rows"] as JArray;
            JArray requestedKeys = @params?["keyColumns"] as JArray;
            if (requestedRows == null || requestedRows.Count == 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "upsert_rows requires a non-empty 'rows' array.");
            }
            if (requestedKeys == null || requestedKeys.Count == 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "upsert_rows requires a non-empty 'keyColumns' array.");
            }

            IWorkbook workbook = null;
            try
            {
                string versionBefore = GetFileVersion(path.FullPath);
                ValidateExpectedVersion(@params, versionBefore);
                workbook = LoadWorkbook(path);
                ISheet sheet = RequireSheet(workbook, sheetName, path.RelativePath);
                HeaderInfo headers = ReadHeaders(sheet, GetInt(@params, "headerRow", 1), path.MaxRows);
                int dataStartRow = ResolveDataStartRow(@params, sheet, headers, path.MaxRows);
                if (sheet.LastRowNum - dataStartRow + 1 > 100000)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        "upsert_rows supports at most 100000 existing data rows per request.");
                }
                List<string> keyColumns = ResolveKeyColumns(requestedKeys, headers);
                List<UpsertInput> inputs = ParseUpsertInputs(requestedRows, keyColumns, headers);
                Dictionary<string, int> existingRows = IndexExistingRows(
                    sheet, headers, keyColumns, dataStartRow);

                int lastDataRow = FindLastMeaningfulRow(sheet, headers, dataStartRow);
                int templateRowIndex = ResolveTemplateRow(
                    @params, sheet, dataStartRow, lastDataRow, copyTemplate);
                IRow templateRow = templateRowIndex >= 0 ? sheet.GetRow(templateRowIndex) : null;
                var results = new JArray();
                int insertedCount = 0;
                int updatedCount = 0;
                int unchangedCount = 0;
                int changedCellCount = 0;
                bool workbookChanged = false;
                int estimatedCharacters = 0;

                foreach (UpsertInput input in inputs)
                {
                    bool inserted = !existingRows.TryGetValue(input.CanonicalKey, out int rowIndex);
                    bool templateCopied = false;
                    IRow row;
                    if (inserted)
                    {
                        rowIndex = Math.Max(dataStartRow, lastDataRow + 1);
                        if (rowIndex >= path.MaxRows)
                        {
                            throw new CommandException(ErrorCodes.InvalidParams,
                                $"Cannot append another row: sheet '{sheetName}' reached its row limit.");
                        }
                        lastDataRow = rowIndex;
                        row = PrepareInsertedRow(sheet, rowIndex, templateRow, copyTemplate,
                            out templateCopied);
                        if (templateCopied)
                        {
                            CopyDataValidations(sheet, templateRowIndex, rowIndex);
                        }
                        workbookChanged = true;
                        insertedCount++;
                    }
                    else
                    {
                        row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
                    }

                    var cellChanges = new JArray();
                    foreach (JProperty property in input.Values.Properties())
                    {
                        int columnIndex = headers.ByName[property.Name];
                        ICell cell = row.GetCell(columnIndex);
                        CellSnapshot previous = ReadCell(cell);
                        JObject formulaObject = property.Value as JObject;
                        bool isFormula = formulaObject != null;
                        string formula = isFormula
                            ? NormalizeFormula(formulaObject["formula"]?.Value<string>(),
                                GetCellAddress(rowIndex, columnIndex))
                            : null;
                        bool changed = isFormula
                            ? !CellHasFormula(cell, formula)
                            : !CellHasLiteralValue(cell, property.Value);
                        if (!changed)
                        {
                            continue;
                        }

                        cell ??= row.CreateCell(columnIndex);
                        try
                        {
                            if (isFormula)
                            {
                                cell.SetCellFormula(formula);
                            }
                            else
                            {
                                SetCellValue(cell, property.Value);
                            }
                        }
                        catch (CommandException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            throw new CommandException(ErrorCodes.InvalidParams,
                                $"Cannot set column '{property.Name}' at row {rowIndex + 1}: {ex.Message}");
                        }

                        workbookChanged = true;
                        changedCellCount++;
                        CellSnapshot current = ReadCell(cell);
                        estimatedCharacters += property.Name.Length +
                                               EstimateSnapshotCharacters(previous) +
                                               EstimateSnapshotCharacters(current);
                        if (estimatedCharacters > MaxQueryResponseCharacters)
                        {
                            throw new CommandException(ErrorCodes.InvalidParams,
                                "upsert_rows response would be too large. Split rows into smaller requests.");
                        }
                        cellChanges.Add(new JObject
                        {
                            ["column"] = property.Name,
                            ["cell"] = GetCellAddress(rowIndex, columnIndex),
                            ["previous"] = SnapshotResult(previous),
                            ["current"] = SnapshotResult(current)
                        });
                    }

                    if (!inserted && cellChanges.Count == 0)
                    {
                        unchangedCount++;
                    }
                    else if (!inserted)
                    {
                        updatedCount++;
                    }

                    existingRows[input.CanonicalKey] = rowIndex;
                    results.Add(new JObject
                    {
                        ["operation"] = inserted ? "insert" : cellChanges.Count == 0 ? "unchanged" : "update",
                        ["row"] = rowIndex + 1,
                        ["key"] = (JObject)input.Key.DeepClone(),
                        ["templateCopied"] = templateCopied,
                        ["templateRow"] = templateCopied ? new JValue(templateRowIndex + 1) : JValue.CreateNull(),
                        ["changes"] = cellChanges
                    });
                }

                if (workbookChanged)
                {
                    sheet.ForceFormulaRecalculation = true;
                }

                string versionAfter = versionBefore;
                EnsureFileUnchanged(path.FullPath, versionBefore);
                if (!dryRun && workbookChanged)
                {
                    SaveWorkbook(workbook, path.FullPath);
                    versionAfter = GetFileVersion(path.FullPath);
                }

                return new
                {
                    action = "upsert_rows",
                    filePath = path.RelativePath,
                    sheet = sheetName,
                    dryRun,
                    saved = !dryRun && workbookChanged,
                    versionBefore,
                    versionAfter,
                    headerRow = headers.RowIndex + 1,
                    dataStartRow = dataStartRow + 1,
                    keyColumns,
                    insertedCount,
                    updatedCount,
                    unchangedCount,
                    changedCellCount,
                    rows = results
                };
            }
            catch (CommandException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot upsert '{path.RelativePath}'. Close the workbook in Excel and try again. {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot upsert '{path.RelativePath}': {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode,
                    $"Failed to upsert '{path.RelativePath}': {ex.Message}");
            }
            finally
            {
                (workbook as IDisposable)?.Dispose();
            }
        }

        private static List<CellChange> ParseCellChanges(JArray cells, WorkbookPathInfo path)
        {
            if (cells == null || cells.Count == 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "set_cells requires a non-empty 'cells' array.");
            }

            var addresses = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<CellChange>(cells.Count);
            foreach (JToken item in cells)
            {
                if (!(item is JObject cell))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        "Every cells item must be an object.");
                }

                ParseAddress(cell["cell"]?.Value<string>(), path,
                    out string address, out int rowIndex, out int columnIndex);
                if (!addresses.Add(address))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Duplicate cell address '{address}' in one request.");
                }

                bool hasValue = cell.Property("value", StringComparison.Ordinal) != null;
                bool hasFormula = cell.Property("formula", StringComparison.Ordinal) != null;
                if (hasValue == hasFormula)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Cell {address} must contain exactly one of 'value' or 'formula'.");
                }

                bool hasExpectedValue = cell.Property("expectedValue", StringComparison.Ordinal) != null;
                bool hasExpectedFormula = cell.Property("expectedFormula", StringComparison.Ordinal) != null;
                if (hasExpectedValue && hasExpectedFormula)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Cell {address} cannot contain both expectedValue and expectedFormula.");
                }

                result.Add(new CellChange
                {
                    Address = address,
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                    Value = hasValue ? cell["value"] : null,
                    Formula = hasFormula
                        ? NormalizeFormula(cell["formula"].Value<string>(), address)
                        : null,
                    HasExpectedValue = hasExpectedValue,
                    ExpectedValue = hasExpectedValue ? cell["expectedValue"] : null,
                    ExpectedFormula = hasExpectedFormula
                        ? NormalizeFormula(cell["expectedFormula"].Value<string>(), address)
                        : null
                });
            }
            return result;
        }

        private static void ValidateExpectedCell(CellChange change, ICell cell, CellSnapshot previous)
        {
            if (change.HasExpectedValue && !CellMatchesToken(cell, change.ExpectedValue))
            {
                throw new CommandException(ConflictErrorCode,
                    $"Cell {change.Address} no longer has expectedValue. Actual: {SnapshotResult(previous).ToString(Formatting.None)}");
            }
            if (change.ExpectedFormula != null && !CellHasFormula(cell, change.ExpectedFormula))
            {
                throw new CommandException(ConflictErrorCode,
                    $"Cell {change.Address} no longer has expectedFormula '={change.ExpectedFormula}'. Actual: {SnapshotResult(previous).ToString(Formatting.None)}");
            }
        }

        private static List<string> ResolveKeyColumns(JArray requestedKeys, HeaderInfo headers)
        {
            var result = new List<string>(requestedKeys.Count);
            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (JToken item in requestedKeys)
            {
                string name = item.Value<string>();
                if (!unique.Add(name))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"keyColumns contains duplicate column '{name}'.");
                }
                if (!headers.ByName.ContainsKey(name))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Key column '{name}' was not found in header row {headers.RowIndex + 1}.");
                }
                result.Add(name);
            }
            return result;
        }

        private static List<UpsertInput> ParseUpsertInputs(
            JArray requestedRows, List<string> keyColumns, HeaderInfo headers)
        {
            var result = new List<UpsertInput>(requestedRows.Count);
            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (JToken item in requestedRows)
            {
                JObject row = item as JObject;
                if (row == null || !row.HasValues)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        "Every upsert_rows row must be a non-empty object.");
                }

                foreach (JProperty property in row.Properties())
                {
                    if (!headers.ByName.ContainsKey(property.Name))
                    {
                        throw new CommandException(ErrorCodes.InvalidParams,
                            $"Column '{property.Name}' was not found in header row {headers.RowIndex + 1}.");
                    }
                    if (property.Value is JObject formulaObject &&
                        (formulaObject.Count != 1 || formulaObject.Property("formula", StringComparison.Ordinal) == null))
                    {
                        throw new CommandException(ErrorCodes.InvalidParams,
                            $"Column '{property.Name}' formula value must contain only a 'formula' property.");
                    }
                }

                var key = new JObject();
                foreach (string keyColumn in keyColumns)
                {
                    JProperty keyProperty = row.Property(keyColumn, StringComparison.Ordinal);
                    if (keyProperty == null || keyProperty.Value.Type == JTokenType.Null ||
                        keyProperty.Value.Type == JTokenType.Object)
                    {
                        throw new CommandException(ErrorCodes.InvalidParams,
                            $"Every upsert row requires a non-null constant key column '{keyColumn}'.");
                    }
                    key[keyColumn] = keyProperty.Value.DeepClone();
                }

                string canonicalKey = BuildCanonicalKey(key, keyColumns);
                if (!unique.Add(canonicalKey))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"upsert_rows contains duplicate key {key.ToString(Formatting.None)}.");
                }
                result.Add(new UpsertInput
                {
                    Values = row,
                    Key = key,
                    CanonicalKey = canonicalKey
                });
            }
            return result;
        }

        private static Dictionary<string, int> IndexExistingRows(
            ISheet sheet, HeaderInfo headers, List<string> keyColumns, int dataStartRow)
        {
            var result = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int rowIndex = dataStartRow; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row == null)
                {
                    continue;
                }

                var key = new JObject();
                bool allBlank = true;
                foreach (string keyColumn in keyColumns)
                {
                    CellSnapshot snapshot = ReadCell(row.GetCell(headers.ByName[keyColumn]));
                    JToken value = snapshot.Value == null
                        ? JValue.CreateNull()
                        : JToken.FromObject(snapshot.Value);
                    key[keyColumn] = value;
                    allBlank &= snapshot.Type == "blank" || snapshot.Value == null;
                }
                if (allBlank)
                {
                    continue;
                }

                string canonicalKey = BuildCanonicalKey(key, keyColumns);
                if (result.TryGetValue(canonicalKey, out int existingRow))
                {
                    throw new CommandException(ConflictErrorCode,
                        $"Existing key {key.ToString(Formatting.None)} is duplicated at rows {existingRow + 1} and {rowIndex + 1}.");
                }
                result.Add(canonicalKey, rowIndex);
            }
            return result;
        }

        private static string BuildCanonicalKey(JObject key, List<string> keyColumns)
        {
            var parts = new List<string>(keyColumns.Count);
            foreach (string column in keyColumns)
            {
                JToken value = key[column];
                switch (value.Type)
                {
                    case JTokenType.Integer:
                    case JTokenType.Float:
                        parts.Add("n:" + value.Value<double>().ToString("R", CultureInfo.InvariantCulture));
                        break;
                    case JTokenType.Boolean:
                        parts.Add(value.Value<bool>() ? "b:1" : "b:0");
                        break;
                    case JTokenType.String:
                    {
                        string text = value.Value<string>();
                        parts.Add($"s:{text.Length}:{text}");
                        break;
                    }
                    default:
                        parts.Add("null");
                        break;
                }
            }
            return string.Join("\u001f", parts);
        }

        private static int ResolveTemplateRow(JObject @params, ISheet sheet,
            int dataStartRow, int lastDataRow, bool copyTemplate)
        {
            if (!copyTemplate)
            {
                return -1;
            }

            int requested = GetInt(@params, "templateRow", 0);
            int rowIndex = requested > 0 ? requested - 1 : lastDataRow;
            if (rowIndex < dataStartRow)
            {
                return -1;
            }
            if (rowIndex > sheet.LastRowNum || sheet.GetRow(rowIndex) == null)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"templateRow {rowIndex + 1} does not exist in sheet '{sheet.SheetName}'.");
            }
            return rowIndex;
        }

        private static IRow PrepareInsertedRow(ISheet sheet, int rowIndex,
            IRow templateRow, bool copyTemplate, out bool templateCopied)
        {
            templateCopied = false;
            if (copyTemplate && templateRow != null)
            {
                IRow existing = sheet.GetRow(rowIndex);
                if (existing != null)
                {
                    sheet.RemoveRow(existing);
                }

                IRow copied = templateRow.CopyRowTo(rowIndex);
                templateCopied = true;
                for (int columnIndex = Math.Max(0, (int)copied.FirstCellNum);
                     columnIndex < copied.LastCellNum; columnIndex++)
                {
                    ICell cell = copied.GetCell(columnIndex);
                    if (cell == null)
                    {
                        continue;
                    }
                    if (cell.CellType == CellType.Formula)
                    {
                        continue;
                    }
                    cell.SetCellType(CellType.Blank);
                }
                return copied;
            }

            return sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
        }

        private static void CopyDataValidations(ISheet sheet, int sourceRow, int targetRow)
        {
            if (sourceRow < 0 || targetRow < 0)
            {
                return;
            }

            List<IDataValidation> validations = sheet.GetDataValidations();
            if (validations == null || validations.Count == 0)
            {
                return;
            }

            IDataValidationHelper helper = sheet.GetDataValidationHelper();
            var sources = new List<IDataValidation>(validations);
            foreach (IDataValidation source in sources)
            {
                CellRangeAddress[] sourceRanges = source.Regions?.CellRangeAddresses;
                if (sourceRanges == null || source.ValidationConstraint == null)
                {
                    continue;
                }
                var ranges = new CellRangeAddressList();
                foreach (CellRangeAddress region in sourceRanges)
                {
                    if (sourceRow < region.FirstRow || sourceRow > region.LastRow ||
                        (targetRow >= region.FirstRow && targetRow <= region.LastRow))
                    {
                        continue;
                    }
                    ranges.AddCellRangeAddress(targetRow, targetRow,
                        region.FirstColumn, region.LastColumn);
                }

                if (ranges.CountRanges() == 0)
                {
                    continue;
                }

                IDataValidationConstraint constraint =
                    CloneValidationConstraint(helper, source.ValidationConstraint);
                if (constraint == null)
                {
                    continue;
                }

                IDataValidation clone = helper.CreateValidation(constraint, ranges);
                clone.ErrorStyle = source.ErrorStyle;
                clone.EmptyCellAllowed = source.EmptyCellAllowed;
                clone.SuppressDropDownArrow = source.SuppressDropDownArrow;
                clone.ShowPromptBox = source.ShowPromptBox;
                clone.ShowErrorBox = source.ShowErrorBox;
                if (source.PromptBoxTitle != null || source.PromptBoxText != null)
                {
                    clone.CreatePromptBox(source.PromptBoxTitle ?? string.Empty,
                        source.PromptBoxText ?? string.Empty);
                }
                if (source.ErrorBoxTitle != null || source.ErrorBoxText != null)
                {
                    clone.CreateErrorBox(source.ErrorBoxTitle ?? string.Empty,
                        source.ErrorBoxText ?? string.Empty);
                }
                sheet.AddValidationData(clone);
            }
        }

        private static IDataValidationConstraint CloneValidationConstraint(
            IDataValidationHelper helper, IDataValidationConstraint source)
        {
            int validationType = source.GetValidationType();
            switch (validationType)
            {
                case ValidationType.LIST:
                    if (source.ExplicitListValues != null && source.ExplicitListValues.Length > 0)
                    {
                        return helper.CreateExplicitListConstraint(source.ExplicitListValues);
                    }
                    return string.IsNullOrEmpty(source.Formula1)
                        ? null
                        : helper.CreateFormulaListConstraint(source.Formula1);
                case ValidationType.INTEGER:
                    return helper.CreateintConstraint(source.Operator, source.Formula1, source.Formula2);
                case ValidationType.DECIMAL:
                    return helper.CreateDecimalConstraint(source.Operator, source.Formula1, source.Formula2);
                case ValidationType.DATE:
                    return helper.CreateDateConstraint(source.Operator, source.Formula1, source.Formula2, null);
                case ValidationType.TIME:
                    return helper.CreateTimeConstraint(source.Operator, source.Formula1, source.Formula2);
                case ValidationType.TEXT_LENGTH:
                    return helper.CreateTextLengthConstraint(source.Operator, source.Formula1, source.Formula2);
                case ValidationType.FORMULA:
                    return string.IsNullOrEmpty(source.Formula1)
                        ? null
                        : helper.CreateCustomConstraint(source.Formula1);
                default:
                    return null;
            }
        }

        private sealed class CellChange
        {
            public string Address;
            public int RowIndex;
            public int ColumnIndex;
            public JToken Value;
            public string Formula;
            public bool HasExpectedValue;
            public JToken ExpectedValue;
            public string ExpectedFormula;
        }

        private sealed class UpsertInput
        {
            public JObject Values;
            public JObject Key;
            public string CanonicalKey;
        }
    }
}

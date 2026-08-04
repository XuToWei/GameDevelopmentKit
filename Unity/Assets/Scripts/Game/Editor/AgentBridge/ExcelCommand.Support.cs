using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AgentBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

namespace Game.Editor
{
    public sealed partial class ExcelCommand
    {
        private const int XlsxMaxRows = 1048576;
        private const int XlsxMaxColumns = 16384;
        private const int XlsMaxRows = 65536;
        private const int XlsMaxColumns = 256;
        private const int MaxQueryResponseCharacters = 500000;

        private static string GetString(JObject @params, string name, string defaultValue)
        {
            return @params?[name]?.Value<string>() ?? defaultValue;
        }

        private static int GetInt(JObject @params, string name, int defaultValue)
        {
            return @params?[name]?.Value<int?>() ?? defaultValue;
        }

        private static bool GetBool(JObject @params, string name, bool defaultValue)
        {
            return @params?[name]?.Value<bool?>() ?? defaultValue;
        }

        private static bool HasParam(JObject @params, string name)
        {
            return @params?.Property(name, StringComparison.Ordinal) != null;
        }

        private static string RequireString(JObject @params, string name)
        {
            string value = GetString(@params, name, null);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new CommandException(ErrorCodes.InvalidParams, $"Missing '{name}'.");
            }
            return value;
        }

        private static WorkbookPathInfo ResolveWorkbookPath(string requestedPath)
        {
            if (string.IsNullOrWhiteSpace(requestedPath))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'filePath'.");
            }

            DirectoryInfo unityProject = Directory.GetParent(Path.GetFullPath(Application.dataPath));
            DirectoryInfo repository = unityProject?.Parent;
            if (repository == null)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot resolve repository root from Unity data path '{Application.dataPath}'.");
            }

            string repositoryRoot = repository.FullName.TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(Path.IsPathRooted(requestedPath)
                    ? requestedPath
                    : Path.Combine(repositoryRoot, requestedPath));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException ||
                                       ex is PathTooLongException)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Invalid filePath '{requestedPath}': {ex.Message}");
            }

            string rootPrefix = repositoryRoot + Path.DirectorySeparatorChar;
            StringComparison comparison = Path.DirectorySeparatorChar == '\\'
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            if (!fullPath.StartsWith(rootPrefix, comparison))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"filePath must be inside repository root '{repositoryRoot}'.");
            }

            string extension = Path.GetExtension(fullPath);
            bool isXls = string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase);
            if (!isXls && !string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"filePath must end with .xlsx or .xls: {requestedPath}");
            }
            if (!File.Exists(fullPath))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Excel file not found: {requestedPath}");
            }

            return new WorkbookPathInfo
            {
                FullPath = fullPath,
                RelativePath = fullPath.Substring(rootPrefix.Length).Replace('\\', '/'),
                IsXls = isXls,
                MaxRows = isXls ? XlsMaxRows : XlsxMaxRows,
                MaxColumns = isXls ? XlsMaxColumns : XlsxMaxColumns
            };
        }

        private static IWorkbook LoadWorkbook(WorkbookPathInfo path)
        {
            using (var stream = new FileStream(path.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return path.IsXls
                    ? (IWorkbook)new HSSFWorkbook(stream)
                    : new XSSFWorkbook(stream);
            }
        }

        private static ISheet RequireSheet(IWorkbook workbook, string sheetName, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'sheet'.");
            }

            ISheet sheet = workbook.GetSheet(sheetName);
            if (sheet == null)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Sheet '{sheetName}' not found in '{relativePath}'.");
            }
            return sheet;
        }

        private static string GetFileVersion(string fullPath)
        {
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(stream);
                var result = new StringBuilder(hash.Length * 2);
                foreach (byte value in hash)
                {
                    result.Append(value.ToString("x2"));
                }
                return result.ToString();
            }
        }

        private static void ValidateExpectedVersion(JObject @params, string actualVersion)
        {
            string expectedVersion = GetString(@params, "expectedVersion", null);
            if (expectedVersion != null &&
                !string.Equals(expectedVersion, actualVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new CommandException(ConflictErrorCode,
                    $"Workbook version changed. Expected {expectedVersion}, actual {actualVersion}. Query it again before writing.");
            }
        }

        private static void EnsureFileUnchanged(string fullPath, string expectedVersion)
        {
            string actualVersion = GetFileVersion(fullPath);
            if (!string.Equals(expectedVersion, actualVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new CommandException(ConflictErrorCode,
                    $"Workbook changed while the command was running. Expected {expectedVersion}, actual {actualVersion}.");
            }
        }

        private static void SaveWorkbook(IWorkbook workbook, string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);
            string fileName = Path.GetFileName(fullPath);
            string token = Guid.NewGuid().ToString("N");
            string temporaryPath = Path.Combine(directory, $".{fileName}.{token}.tmp");
            string backupPath = Path.Combine(directory, $".{fileName}.{token}.bak");
            bool replaced = false;

            try
            {
                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    workbook.Write(stream);
                    stream.Flush();
                }

                File.Replace(temporaryPath, fullPath, backupPath);
                replaced = true;
            }
            finally
            {
                TryDelete(temporaryPath);
                if (replaced)
                {
                    TryDelete(backupPath);
                }
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AgentBridge] Failed to delete temporary Excel file '{path}': {ex.Message}");
            }
        }

        private static void ParseAddress(string requestedAddress, WorkbookPathInfo path,
            out string normalizedAddress, out int rowIndex, out int columnIndex)
        {
            if (string.IsNullOrEmpty(requestedAddress))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "An A1 cell address is required.");
            }

            int split = 0;
            while (split < requestedAddress.Length &&
                   ((requestedAddress[split] >= 'A' && requestedAddress[split] <= 'Z') ||
                    (requestedAddress[split] >= 'a' && requestedAddress[split] <= 'z')))
            {
                split++;
            }

            if (split == 0 || split == requestedAddress.Length || split > 3 ||
                !int.TryParse(requestedAddress.Substring(split), NumberStyles.None,
                    CultureInfo.InvariantCulture, out int row) || row < 1 || row > path.MaxRows)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Invalid or out-of-range Excel cell address '{requestedAddress}'.");
            }

            int column = 0;
            for (int i = 0; i < split; i++)
            {
                char letter = char.ToUpperInvariant(requestedAddress[i]);
                column = column * 26 + letter - 'A' + 1;
            }
            if (column < 1 || column > path.MaxColumns)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Excel cell address '{requestedAddress}' exceeds the workbook column limit.");
            }

            normalizedAddress = GetCellAddress(row - 1, column - 1);
            rowIndex = row - 1;
            columnIndex = column - 1;
        }

        private static CellRangeInfo ParseRange(string requestedRange, WorkbookPathInfo path, int maxCells)
        {
            if (string.IsNullOrWhiteSpace(requestedRange))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'range' for read_range.");
            }

            string[] parts = requestedRange.Split(':');
            if (parts.Length < 1 || parts.Length > 2)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Invalid Excel range '{requestedRange}'. Use A1 or A1:H30.");
            }

            ParseAddress(parts[0], path, out _, out int firstRow, out int firstColumn);
            ParseAddress(parts.Length == 2 ? parts[1] : parts[0], path,
                out _, out int lastRow, out int lastColumn);
            if (lastRow < firstRow || lastColumn < firstColumn)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Range end must not precede range start: {requestedRange}");
            }

            long cellCount = (long)(lastRow - firstRow + 1) * (lastColumn - firstColumn + 1);
            if (cellCount > maxCells)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Range '{requestedRange}' contains {cellCount} cells, exceeding maxCells={maxCells}.");
            }

            return new CellRangeInfo
            {
                FirstRow = firstRow,
                LastRow = lastRow,
                FirstColumn = firstColumn,
                LastColumn = lastColumn,
                CellCount = (int)cellCount,
                Address = $"{GetCellAddress(firstRow, firstColumn)}:{GetCellAddress(lastRow, lastColumn)}"
            };
        }

        private static string GetCellAddress(int rowIndex, int columnIndex)
        {
            int value = columnIndex + 1;
            var column = new StringBuilder();
            while (value > 0)
            {
                value--;
                column.Insert(0, (char)('A' + value % 26));
                value /= 26;
            }
            return column.ToString() + (rowIndex + 1).ToString(CultureInfo.InvariantCulture);
        }

        private static string NormalizeFormula(string formula, string address)
        {
            string normalized = formula?.Trim();
            if (!string.IsNullOrEmpty(normalized) && normalized[0] == '=')
            {
                normalized = normalized.Substring(1).TrimStart();
            }
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Formula for {address} must not be empty.");
            }
            return normalized;
        }

        private static object SetCellValue(ICell cell, JToken value)
        {
            if (value == null || value.Type == JTokenType.Null)
            {
                cell.SetCellType(CellType.Blank);
                return null;
            }

            switch (value.Type)
            {
                case JTokenType.String:
                {
                    string result = value.Value<string>();
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(result);
                    return result;
                }
                case JTokenType.Integer:
                case JTokenType.Float:
                {
                    double result = value.Value<double>();
                    cell.SetCellType(CellType.Numeric);
                    cell.SetCellValue(result);
                    return value.ToObject<object>();
                }
                case JTokenType.Boolean:
                {
                    bool result = value.Value<bool>();
                    cell.SetCellType(CellType.Boolean);
                    cell.SetCellValue(result);
                    return result;
                }
                default:
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Unsupported Excel cell value type: {value.Type}.");
            }
        }

        private static CellSnapshot ReadCell(ICell cell)
        {
            if (cell == null || cell.CellType == CellType.Blank)
            {
                return new CellSnapshot { Type = "blank", Value = null, Formula = null, Text = null };
            }

            if (cell.CellType == CellType.Formula)
            {
                CellType cachedType;
                try
                {
                    cachedType = cell.CachedFormulaResultType;
                }
                catch
                {
                    cachedType = CellType.Unknown;
                }

                return new CellSnapshot
                {
                    Type = "formula",
                    Value = ReadCellValue(cell, cachedType),
                    Formula = $"={cell.CellFormula}",
                    Text = SafeCellText(cell)
                };
            }

            return new CellSnapshot
            {
                Type = GetCellTypeName(cell.CellType),
                Value = ReadCellValue(cell, cell.CellType),
                Formula = null,
                Text = SafeCellText(cell)
            };
        }

        private static object ReadCellValue(ICell cell, CellType type)
        {
            try
            {
                switch (type)
                {
                    case CellType.String:
                        return cell.StringCellValue;
                    case CellType.Numeric:
                        return cell.NumericCellValue;
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    case CellType.Error:
                        return (int)cell.ErrorCellValue;
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GetCellTypeName(CellType type)
        {
            switch (type)
            {
                case CellType.String: return "string";
                case CellType.Numeric: return "number";
                case CellType.Boolean: return "boolean";
                case CellType.Error: return "error";
                case CellType.Formula: return "formula";
                default: return "blank";
            }
        }

        private static string SafeCellText(ICell cell)
        {
            try
            {
                return cell?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static bool CellMatchesToken(ICell cell, JToken expected, bool ignoreCase = false)
        {
            CellSnapshot snapshot = ReadCell(cell);
            if (expected == null || expected.Type == JTokenType.Null)
            {
                return snapshot.Type == "blank" || snapshot.Value == null;
            }

            switch (expected.Type)
            {
                case JTokenType.String:
                    return snapshot.Value is string text &&
                           string.Equals(text, expected.Value<string>(),
                               ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                case JTokenType.Integer:
                case JTokenType.Float:
                    return snapshot.Value != null &&
                           TryGetDouble(snapshot.Value, out double actual) &&
                           actual.Equals(expected.Value<double>());
                case JTokenType.Boolean:
                    return snapshot.Value is bool value && value == expected.Value<bool>();
                default:
                    return false;
            }
        }

        private static bool CellContainsToken(ICell cell, JToken expected, bool ignoreCase)
        {
            CellSnapshot snapshot = ReadCell(cell);
            if (expected == null || expected.Type == JTokenType.Null)
            {
                return snapshot.Type == "blank";
            }

            string actual = snapshot.Text ?? Convert.ToString(snapshot.Value, CultureInfo.InvariantCulture) ?? string.Empty;
            string requested = expected.Type == JTokenType.String
                ? expected.Value<string>()
                : expected.ToString(Formatting.None);
            return actual.IndexOf(requested,
                ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
        }

        private static bool TryGetDouble(object value, out double result)
        {
            try
            {
                result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return !double.IsNaN(result) && !double.IsInfinity(result);
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        private static bool CellHasLiteralValue(ICell cell, JToken expected)
        {
            return (cell == null || cell.CellType != CellType.Formula) &&
                   CellMatchesToken(cell, expected);
        }

        private static bool CellHasFormula(ICell cell, string normalizedFormula)
        {
            return cell != null && cell.CellType == CellType.Formula &&
                   string.Equals(cell.CellFormula, normalizedFormula, StringComparison.Ordinal);
        }

        private static HeaderInfo ReadHeaders(ISheet sheet, int headerRowNumber, int maxRows)
        {
            if (headerRowNumber < 1 || headerRowNumber > maxRows)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"headerRow must be in 1..{maxRows}, got {headerRowNumber}.");
            }

            int rowIndex = headerRowNumber - 1;
            IRow row = sheet.GetRow(rowIndex);
            if (row == null || row.LastCellNum <= 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Header row {headerRowNumber} is empty in sheet '{sheet.SheetName}'.");
            }

            var result = new HeaderInfo { RowIndex = rowIndex };
            for (int columnIndex = Math.Max(0, row.FirstCellNum);
                 columnIndex < row.LastCellNum; columnIndex++)
            {
                string name = SafeCellText(row.GetCell(columnIndex))?.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                if (result.ByName.ContainsKey(name))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Header '{name}' appears more than once in row {headerRowNumber}.");
                }

                result.ByName.Add(name, columnIndex);
                result.Columns.Add(new HeaderColumn { Name = name, ColumnIndex = columnIndex });
            }

            if (result.Columns.Count == 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Header row {headerRowNumber} has no named columns.");
            }
            if (result.Columns.Count > 512)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Header row {headerRowNumber} has too many columns ({result.Columns.Count}, maximum 512).");
            }
            return result;
        }

        private static int ResolveDataStartRow(JObject @params, ISheet sheet, HeaderInfo headers, int maxRows)
        {
            int requested = GetInt(@params, "dataStartRow", 0);
            int result;
            if (requested > 0)
            {
                result = requested - 1;
            }
            else
            {
                result = headers.RowIndex + 1;
                string marker = SafeCellText(sheet.GetRow(headers.RowIndex)?.GetCell(0))?.Trim();
                string typeMarker = SafeCellText(sheet.GetRow(headers.RowIndex + 1)?.GetCell(0))?.Trim();
                string commentMarker = SafeCellText(sheet.GetRow(headers.RowIndex + 2)?.GetCell(0))?.Trim();
                if (string.Equals(marker, "##var", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(typeMarker, "##type", StringComparison.OrdinalIgnoreCase) &&
                    commentMarker != null && commentMarker.StartsWith("##", StringComparison.Ordinal))
                {
                    result = headers.RowIndex + 3;
                }
            }

            if (result <= headers.RowIndex || result >= maxRows)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"dataStartRow must be after headerRow and within 1..{maxRows}, got {result + 1}.");
            }
            return result;
        }

        private static int FindLastMeaningfulRow(ISheet sheet, HeaderInfo headers, int dataStartRow)
        {
            for (int rowIndex = sheet.LastRowNum; rowIndex >= dataStartRow; rowIndex--)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row == null)
                {
                    continue;
                }
                foreach (HeaderColumn header in headers.Columns)
                {
                    if (ReadCell(row.GetCell(header.ColumnIndex)).Type != "blank")
                    {
                        return rowIndex;
                    }
                }
            }
            return dataStartRow - 1;
        }

        private static JObject CellResult(string address, int rowIndex, int columnIndex, CellSnapshot snapshot)
        {
            return new JObject
            {
                ["cell"] = address,
                ["row"] = rowIndex + 1,
                ["column"] = columnIndex + 1,
                ["type"] = snapshot.Type,
                ["value"] = snapshot.Value == null ? JValue.CreateNull() : JToken.FromObject(snapshot.Value),
                ["formula"] = snapshot.Formula == null ? JValue.CreateNull() : new JValue(snapshot.Formula),
                ["text"] = snapshot.Text == null ? JValue.CreateNull() : new JValue(snapshot.Text)
            };
        }

        private static JObject SnapshotResult(CellSnapshot snapshot)
        {
            return new JObject
            {
                ["type"] = snapshot.Type,
                ["value"] = snapshot.Value == null ? JValue.CreateNull() : JToken.FromObject(snapshot.Value),
                ["formula"] = snapshot.Formula == null ? JValue.CreateNull() : new JValue(snapshot.Formula),
                ["text"] = snapshot.Text == null ? JValue.CreateNull() : new JValue(snapshot.Text)
            };
        }

        private static int EstimateSnapshotCharacters(CellSnapshot snapshot)
        {
            int result = 96;
            if (snapshot.Value is string value)
            {
                result += value.Length;
            }
            else if (snapshot.Value != null)
            {
                result += 32;
            }
            result += snapshot.Formula?.Length ?? 0;
            result += snapshot.Text?.Length ?? 0;
            return result;
        }

        private sealed class WorkbookPathInfo
        {
            public string FullPath;
            public string RelativePath;
            public bool IsXls;
            public int MaxRows;
            public int MaxColumns;
        }

        private sealed class CellRangeInfo
        {
            public int FirstRow;
            public int LastRow;
            public int FirstColumn;
            public int LastColumn;
            public int CellCount;
            public string Address;
        }

        private sealed class CellSnapshot
        {
            public string Type;
            public object Value;
            public string Formula;
            public string Text;
        }

        private sealed class HeaderInfo
        {
            public int RowIndex;
            public readonly Dictionary<string, int> ByName =
                new Dictionary<string, int>(StringComparer.Ordinal);
            public readonly List<HeaderColumn> Columns = new List<HeaderColumn>();
        }

        private sealed class HeaderColumn
        {
            public string Name;
            public int ColumnIndex;
        }
    }
}

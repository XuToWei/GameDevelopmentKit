using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AgentBridge;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;

namespace Game.Editor
{
    public sealed partial class ExcelCommand
    {
        private static object Inspect(JObject @params)
        {
            WorkbookPathInfo path = ResolveWorkbookPath(GetString(@params, "filePath", null));
            IWorkbook workbook = null;
            try
            {
                string version = GetFileVersion(path.FullPath);
                workbook = LoadWorkbook(path);
                var sheets = new JArray();

                for (int index = 0; index < workbook.NumberOfSheets; index++)
                {
                    ISheet sheet = workbook.GetSheetAt(index);
                    UsedRangeInfo used = GetUsedRange(sheet);
                    int? suggestedHeaderRow = FindSuggestedHeaderRow(sheet, used);
                    int? suggestedDataStartRow = suggestedHeaderRow.HasValue
                        ? FindSuggestedDataStartRow(sheet, suggestedHeaderRow.Value)
                        : null;

                    int validationCount;
                    try
                    {
                        validationCount = sheet.GetDataValidations()?.Count ?? 0;
                    }
                    catch
                    {
                        validationCount = -1;
                    }

                    sheets.Add(new JObject
                    {
                        ["index"] = index,
                        ["name"] = sheet.SheetName,
                        ["hidden"] = workbook.IsSheetHidden(index),
                        ["usedRange"] = used == null ? JValue.CreateNull() : new JValue(used.Address),
                        ["firstRow"] = used == null ? JValue.CreateNull() : new JValue(used.FirstRow + 1),
                        ["lastRow"] = used == null ? JValue.CreateNull() : new JValue(used.LastRow + 1),
                        ["firstColumn"] = used == null ? JValue.CreateNull() : new JValue(used.FirstColumn + 1),
                        ["lastColumn"] = used == null ? JValue.CreateNull() : new JValue(used.LastColumn + 1),
                        ["mergedRegionCount"] = sheet.NumMergedRegions,
                        ["dataValidationCount"] = validationCount,
                        ["suggestedHeaderRow"] = suggestedHeaderRow.HasValue
                            ? new JValue(suggestedHeaderRow.Value)
                            : JValue.CreateNull(),
                        ["suggestedDataStartRow"] = suggestedDataStartRow.HasValue
                            ? new JValue(suggestedDataStartRow.Value)
                            : JValue.CreateNull()
                    });
                }

                EnsureFileUnchanged(path.FullPath, version);
                return new
                {
                    action = "inspect",
                    filePath = path.RelativePath,
                    format = path.IsXls ? "xls" : "xlsx",
                    version,
                    sheetCount = workbook.NumberOfSheets,
                    sheets
                };
            }
            catch (CommandException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot inspect '{path.RelativePath}'. Close the workbook in Excel and try again. {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode,
                    $"Failed to inspect '{path.RelativePath}': {ex.Message}");
            }
            finally
            {
                (workbook as IDisposable)?.Dispose();
            }
        }

        private static object ReadRange(JObject @params)
        {
            WorkbookPathInfo path = ResolveWorkbookPath(GetString(@params, "filePath", null));
            int maxCells = GetInt(@params, "maxCells", 500);
            if (maxCells < 1 || maxCells > 2000)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"maxCells must be in 1..2000, got {maxCells}.");
            }

            CellRangeInfo range = ParseRange(GetString(@params, "range", null), path, maxCells);
            bool includeEmpty = GetBool(@params, "includeEmpty", true);
            string sheetName = RequireString(@params, "sheet");
            IWorkbook workbook = null;

            try
            {
                string version = GetFileVersion(path.FullPath);
                workbook = LoadWorkbook(path);
                ISheet sheet = RequireSheet(workbook, sheetName, path.RelativePath);
                var cells = new JArray();
                int estimatedCharacters = 0;

                for (int rowIndex = range.FirstRow; rowIndex <= range.LastRow; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    for (int columnIndex = range.FirstColumn;
                         columnIndex <= range.LastColumn; columnIndex++)
                    {
                        CellSnapshot snapshot = ReadCell(row?.GetCell(columnIndex));
                        if (!includeEmpty && snapshot.Type == "blank")
                        {
                            continue;
                        }
                        estimatedCharacters += EstimateSnapshotCharacters(snapshot);
                        if (estimatedCharacters > MaxQueryResponseCharacters)
                        {
                            throw new CommandException(ErrorCodes.InvalidParams,
                                "read_range result is too large. Request a smaller range or set includeEmpty=false.");
                        }
                        cells.Add(CellResult(GetCellAddress(rowIndex, columnIndex),
                            rowIndex, columnIndex, snapshot));
                    }
                }

                EnsureFileUnchanged(path.FullPath, version);
                return new
                {
                    action = "read_range",
                    filePath = path.RelativePath,
                    sheet = sheetName,
                    version,
                    range = range.Address,
                    requestedCellCount = range.CellCount,
                    returnedCellCount = cells.Count,
                    includeEmpty,
                    cells
                };
            }
            catch (CommandException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot read '{path.RelativePath}'. Close the workbook in Excel and try again. {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode,
                    $"Failed to read '{path.RelativePath}': {ex.Message}");
            }
            finally
            {
                (workbook as IDisposable)?.Dispose();
            }
        }

        private static object FindRows(JObject @params)
        {
            WorkbookPathInfo path = ResolveWorkbookPath(GetString(@params, "filePath", null));
            string sheetName = RequireString(@params, "sheet");
            JObject where = @params?["where"] as JObject;
            if (where == null || !where.HasValues)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "find_rows requires a non-empty 'where' object.");
            }

            string match = GetString(@params, "match", "exact").ToLowerInvariant();
            if (match != "exact" && match != "contains")
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Unknown match mode '{match}'. Supported: exact, contains.");
            }
            bool ignoreCase = GetBool(@params, "ignoreCase", false);
            int offset = GetInt(@params, "offset", 0);
            int limit = GetInt(@params, "limit", 50);
            if (offset < 0 || limit < 1 || limit > 200)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"offset must be >= 0 and limit must be in 1..200, got offset={offset}, limit={limit}.");
            }

            IWorkbook workbook = null;
            try
            {
                string version = GetFileVersion(path.FullPath);
                workbook = LoadWorkbook(path);
                ISheet sheet = RequireSheet(workbook, sheetName, path.RelativePath);
                HeaderInfo headers = ReadHeaders(sheet, GetInt(@params, "headerRow", 1), path.MaxRows);
                int dataStartRow = ResolveDataStartRow(@params, sheet, headers, path.MaxRows);
                int defaultEndRow = Math.Max(sheet.LastRowNum, dataStartRow) + 1;
                int endRow = GetInt(@params, "endRow", defaultEndRow) - 1;
                if (endRow < dataStartRow || endRow >= path.MaxRows)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"endRow must be in {dataStartRow + 1}..{path.MaxRows}, got {endRow + 1}.");
                }
                if (endRow - dataStartRow + 1 > 100000)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        "find_rows scans at most 100000 rows per request. Narrow the range with endRow.");
                }

                foreach (JProperty condition in where.Properties())
                {
                    if (!headers.ByName.ContainsKey(condition.Name))
                    {
                        throw new CommandException(ErrorCodes.InvalidParams,
                            $"where column '{condition.Name}' was not found in header row {headers.RowIndex + 1}.");
                    }
                }

                List<HeaderColumn> selected = ResolveSelectedColumns(@params?["select"] as JArray, headers);
                var rows = new JArray();
                int matchedCount = 0;
                int scannedCount = 0;
                int estimatedCharacters = 0;

                for (int rowIndex = dataStartRow; rowIndex <= endRow; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    scannedCount++;
                    if (!RowMatches(row, where, headers, match, ignoreCase))
                    {
                        continue;
                    }

                    int currentMatch = matchedCount++;
                    if (currentMatch < offset || rows.Count >= limit)
                    {
                        continue;
                    }

                    var values = new JObject();
                    var formulas = new JObject();
                    foreach (HeaderColumn header in selected)
                    {
                        CellSnapshot snapshot = ReadCell(row?.GetCell(header.ColumnIndex));
                        estimatedCharacters += header.Name.Length + EstimateSnapshotCharacters(snapshot);
                        if (estimatedCharacters > MaxQueryResponseCharacters)
                        {
                            throw new CommandException(ErrorCodes.InvalidParams,
                                "find_rows result is too large. Reduce limit or select fewer columns.");
                        }
                        values[header.Name] = snapshot.Value == null
                            ? JValue.CreateNull()
                            : JToken.FromObject(snapshot.Value);
                        if (snapshot.Formula != null)
                        {
                            formulas[header.Name] = snapshot.Formula;
                        }
                    }

                    rows.Add(new JObject
                    {
                        ["row"] = rowIndex + 1,
                        ["values"] = values,
                        ["formulas"] = formulas
                    });
                }

                EnsureFileUnchanged(path.FullPath, version);
                return new
                {
                    action = "find_rows",
                    filePath = path.RelativePath,
                    sheet = sheetName,
                    version,
                    headerRow = headers.RowIndex + 1,
                    dataStartRow = dataStartRow + 1,
                    endRow = endRow + 1,
                    match,
                    ignoreCase,
                    offset,
                    limit,
                    scannedCount,
                    matchedCount,
                    returnedCount = rows.Count,
                    hasMore = offset + rows.Count < matchedCount,
                    rows
                };
            }
            catch (CommandException)
            {
                throw;
            }
            catch (IOException ex)
            {
                throw new CommandException(ErrorCode,
                    $"Cannot query '{path.RelativePath}'. Close the workbook in Excel and try again. {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new CommandException(ErrorCode,
                    $"Failed to query '{path.RelativePath}': {ex.Message}");
            }
            finally
            {
                (workbook as IDisposable)?.Dispose();
            }
        }

        private static bool RowMatches(IRow row, JObject where, HeaderInfo headers,
            string match, bool ignoreCase)
        {
            foreach (JProperty condition in where.Properties())
            {
                ICell cell = row?.GetCell(headers.ByName[condition.Name]);
                bool matches = match == "contains"
                    ? CellContainsToken(cell, condition.Value, ignoreCase)
                    : CellMatchesToken(cell, condition.Value, ignoreCase);
                if (!matches)
                {
                    return false;
                }
            }
            return true;
        }

        private static List<HeaderColumn> ResolveSelectedColumns(JArray select, HeaderInfo headers)
        {
            if (select == null)
            {
                var defaults = new List<HeaderColumn>();
                foreach (HeaderColumn header in headers.Columns)
                {
                    if (!header.Name.StartsWith("##", StringComparison.Ordinal))
                    {
                        defaults.Add(header);
                    }
                }
                return defaults.Count > 0 ? defaults : new List<HeaderColumn>(headers.Columns);
            }

            var result = new List<HeaderColumn>(select.Count);
            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (JToken item in select)
            {
                string name = item.Value<string>();
                if (!unique.Add(name))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"select contains duplicate column '{name}'.");
                }
                if (!headers.ByName.TryGetValue(name, out int columnIndex))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"select column '{name}' was not found in header row {headers.RowIndex + 1}.");
                }
                result.Add(new HeaderColumn { Name = name, ColumnIndex = columnIndex });
            }
            return result;
        }

        private static UsedRangeInfo GetUsedRange(ISheet sheet)
        {
            int firstRow = int.MaxValue;
            int lastRow = -1;
            int firstColumn = int.MaxValue;
            int lastColumn = -1;
            IEnumerator rows = sheet.GetRowEnumerator();

            while (rows.MoveNext())
            {
                IRow row = rows.Current as IRow;
                if (row == null || row.LastCellNum <= 0)
                {
                    continue;
                }

                for (int columnIndex = Math.Max(0, row.FirstCellNum);
                     columnIndex < row.LastCellNum; columnIndex++)
                {
                    if (ReadCell(row.GetCell(columnIndex)).Type == "blank")
                    {
                        continue;
                    }

                    firstRow = Math.Min(firstRow, row.RowNum);
                    lastRow = Math.Max(lastRow, row.RowNum);
                    firstColumn = Math.Min(firstColumn, columnIndex);
                    lastColumn = Math.Max(lastColumn, columnIndex);
                }
            }

            if (lastRow < 0)
            {
                return null;
            }

            return new UsedRangeInfo
            {
                FirstRow = firstRow,
                LastRow = lastRow,
                FirstColumn = firstColumn,
                LastColumn = lastColumn,
                Address = $"{GetCellAddress(firstRow, firstColumn)}:{GetCellAddress(lastRow, lastColumn)}"
            };
        }

        private static int? FindSuggestedHeaderRow(ISheet sheet, UsedRangeInfo used)
        {
            if (used == null)
            {
                return null;
            }

            int lastProbe = Math.Min(used.LastRow, used.FirstRow + 19);
            for (int rowIndex = used.FirstRow; rowIndex <= lastProbe; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);
                for (int columnIndex = used.FirstColumn;
                     columnIndex <= Math.Min(used.LastColumn, used.FirstColumn + 4); columnIndex++)
                {
                    string marker = SafeCellText(row?.GetCell(columnIndex))?.Trim();
                    if (string.Equals(marker, "##var", StringComparison.OrdinalIgnoreCase))
                    {
                        return rowIndex + 1;
                    }
                }
            }
            return used.FirstRow + 1;
        }

        private static int FindSuggestedDataStartRow(ISheet sheet, int headerRowNumber)
        {
            int headerIndex = headerRowNumber - 1;
            string marker = SafeCellText(sheet.GetRow(headerIndex)?.GetCell(0))?.Trim();
            string typeMarker = SafeCellText(sheet.GetRow(headerIndex + 1)?.GetCell(0))?.Trim();
            string commentMarker = SafeCellText(sheet.GetRow(headerIndex + 2)?.GetCell(0))?.Trim();
            if (string.Equals(marker, "##var", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(typeMarker, "##type", StringComparison.OrdinalIgnoreCase) &&
                commentMarker != null && commentMarker.StartsWith("##", StringComparison.Ordinal))
            {
                return headerRowNumber + 3;
            }
            return headerRowNumber + 1;
        }

        private sealed class UsedRangeInfo
        {
            public int FirstRow;
            public int LastRow;
            public int FirstColumn;
            public int LastColumn;
            public string Address;
        }
    }
}

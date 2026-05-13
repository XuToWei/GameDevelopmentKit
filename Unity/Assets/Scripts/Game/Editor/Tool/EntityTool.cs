using System.Collections.Generic;
using System.IO;
using GameFramework;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class EntityTool
    {
#if UNITY_ET
        private const string COMMON_ENTITY_XLSX = "../Design/Excel/ET/Datas/Game/Entity.xlsx";
        private const string UI_ENTITY_XLSX = "../Design/Excel/ET/Datas/Game/UIEntity.xlsx";
#elif UNITY_GAMEHOT
        private const string COMMON_ENTITY_XLSX = "../Design/Excel/GameHot/Datas/Game/Entity.xlsx";
        private const string UI_ENTITY_XLSX = "../Design/Excel/GameHot/Datas/Game/UIEntity.xlsx";
#else
         private const string COMMON_ENTITY_XLSX = "../Design/Excel/Game/Datas/Entity.xlsx";
        private const string UI_ENTITY_XLSX = "../Design/Excel/Game/Datas/UIEntity.xlsx";
#endif
        private const string COMMON_ENTITY_ASSET_PATH = "Assets/Res/Entity";
        private const string UI_ENTITY_ASSET_PATH = "Assets/Res/UI/UIEntity";
        private const string NO_ADD_ENTITY_SHEET = "~未添加的实体";

        [MenuItem("Game/Entity Tool/Write Entity To Tables")]
        public static void WriteCommonEntityToTables()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[]{ COMMON_ENTITY_ASSET_PATH });
            if (guids.Length < 1)
                return;
            List<string> entityAssetNames = new List<string>();
            int maxEntityId = 0;
            using (var fs = new FileStream(COMMON_ENTITY_XLSX, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var workbook = new XSSFWorkbook(fs);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var ws = workbook.GetSheetAt(i);
                    if (ws.SheetName.StartsWith("~"))
                        continue;
                    if (ws.LastRowNum < 3)
                        continue;
                    for (int row = 3; row <= ws.LastRowNum; row++)
                    {
                        var dataRow = ws.GetRow(row);
                        if (dataRow == null)
                            continue;
                        string assetName = dataRow.GetCell(4)?.ToString();
                        if (!string.IsNullOrEmpty(assetName))
                        {
                            entityAssetNames.Add(assetName);
                        }
                        int entityId = (int)(dataRow.GetCell(1)?.NumericCellValue ?? 0);
                        maxEntityId = Mathf.Max(maxEntityId, entityId);
                    }
                }
            }
            List<object[]> insertList = new List<object[]>();
            foreach (var guid in guids)
            {
                string entityAsset = AssetDatabase.GUIDToAssetPath(guid);
                string entityAssetName = Utility.Path.GetRegularPath(entityAsset).Replace(COMMON_ENTITY_ASSET_PATH, "");
                if (entityAssetNames.Contains(entityAssetName))
                    continue;
                insertList.Add(new object[]
                {
                    ++maxEntityId,
                    Path.GetFileNameWithoutExtension(entityAsset),
                    "",
                    entityAssetName,
                    "Default",
                    0,
                });
            }
            if (insertList.Count > 0)
            {
                string[] headers = { "Id", "CSName", "Desc", "AssetName", "EntityGroupName", "Priority" };
                InsertToSheet(COMMON_ENTITY_XLSX, NO_ADD_ENTITY_SHEET, headers, insertList);
                Debug.Log($"未添加的实体写入：{COMMON_ENTITY_XLSX}@{NO_ADD_ENTITY_SHEET}！");
            }
            else
            {
                ClearSheet(COMMON_ENTITY_XLSX, NO_ADD_ENTITY_SHEET);
                Debug.Log("没有添加的实体！");
            }
        }

        [MenuItem("Game/Entity Tool/Write UIEntity To Tables")]
        public static void WriteUIEntityToTables()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[]{ UI_ENTITY_ASSET_PATH });
            if (guids.Length < 1)
                return;
            List<string> entityAssetNames = new List<string>();
            int maxEntityId = 0;
            using (var fs = new FileStream(UI_ENTITY_XLSX, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var workbook = new XSSFWorkbook(fs);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var ws = workbook.GetSheetAt(i);
                    if (ws.SheetName.StartsWith("~"))
                        continue;
                    if (ws.LastRowNum < 3)
                        continue;
                    for (int row = 3; row <= ws.LastRowNum; row++)
                    {
                        var dataRow = ws.GetRow(row);
                        if (dataRow == null)
                            continue;
                        string assetName = dataRow.GetCell(4)?.ToString();
                        if (!string.IsNullOrEmpty(assetName))
                        {
                            entityAssetNames.Add(assetName);
                        }
                        int entityId = (int)(dataRow.GetCell(1)?.NumericCellValue ?? 0);
                        maxEntityId = Mathf.Max(maxEntityId, entityId);
                    }
                }
            }
            List<object[]> insertList = new List<object[]>();
            foreach (var guid in guids)
            {
                string entityAsset = AssetDatabase.GUIDToAssetPath(guid);
                string entityAssetName = Utility.Path.GetRegularPath(entityAsset).Replace(UI_ENTITY_ASSET_PATH, "");
                if (entityAssetNames.Contains(entityAssetName))
                    continue;
                insertList.Add(new object[]
                {
                    ++maxEntityId,
                    Path.GetFileNameWithoutExtension(entityAsset),
                    "",
                    entityAssetName,
                    "UI",
                    0,
                });
            }
            if (insertList.Count > 0)
            {
                string[] headers = { "Id", "CSName", "Desc", "AssetName", "EntityGroupName", "Priority" };
                InsertToSheet(UI_ENTITY_XLSX, NO_ADD_ENTITY_SHEET, headers, insertList);
                Debug.Log($"未添加的实体写入：{UI_ENTITY_XLSX}@{NO_ADD_ENTITY_SHEET}！");
            }
            else
            {
                ClearSheet(UI_ENTITY_XLSX, NO_ADD_ENTITY_SHEET);
                Debug.Log("没有添加的实体！");
            }
        }

        private static void ClearSheet(string filePath, string sheetName)
        {
            IWorkbook workbook;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new XSSFWorkbook(fs);
            }
            int idx = workbook.GetSheetIndex(sheetName);
            if (idx >= 0)
                workbook.RemoveSheetAt(idx);
            workbook.CreateSheet(sheetName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }

        private static void InsertToSheet(string filePath, string sheetName, string[] headers, List<object[]> data)
        {
            IWorkbook workbook;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = new XSSFWorkbook(fs);
            }
            int idx = workbook.GetSheetIndex(sheetName);
            if (idx >= 0)
                workbook.RemoveSheetAt(idx);
            var ws = workbook.CreateSheet(sheetName);
            var headerRow = ws.CreateRow(0);
            for (int col = 0; col < headers.Length; col++)
            {
                headerRow.CreateCell(col).SetCellValue(headers[col]);
            }
            for (int i = 0; i < data.Count; i++)
            {
                var dataRow = ws.CreateRow(i + 1);
                for (int col = 0; col < data[i].Length; col++)
                {
                    var cell = dataRow.CreateCell(col);
                    if (data[i][col] is int intVal)
                        cell.SetCellValue(intVal);
                    else if (data[i][col] is double doubleVal)
                        cell.SetCellValue(doubleVal);
                    else
                        cell.SetCellValue(data[i][col]?.ToString() ?? "");
                }
            }
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }
    }
}

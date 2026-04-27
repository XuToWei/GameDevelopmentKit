using System.Collections.Generic;
using System.IO;
using GameFramework;
using OfficeOpenXml;
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
            using (var package = new ExcelPackage(new FileInfo(COMMON_ENTITY_XLSX)))
            {
                foreach (var ws in package.Workbook.Worksheets)
                {
                    if (ws.Name.StartsWith("~"))
                        continue;
                    if (ws.Dimension == null)
                        continue;
                    for (int row = 4; row <= ws.Dimension.End.Row; row++)
                    {
                        string assetName = ws.Cells[row, 5].GetValue<string>();
                        if (!string.IsNullOrEmpty(assetName))
                        {
                            entityAssetNames.Add(assetName);
                        }
                        int entityId = ws.Cells[row, 2].GetValue<int>();
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
            using (var package = new ExcelPackage(new FileInfo(UI_ENTITY_XLSX)))
            {
                foreach (var ws in package.Workbook.Worksheets)
                {
                    if (ws.Name.StartsWith("~"))
                        continue;
                    if (ws.Dimension == null)
                        continue;
                    for (int row = 4; row <= ws.Dimension.End.Row; row++)
                    {
                        string assetName = ws.Cells[row, 5].GetValue<string>();
                        if (!string.IsNullOrEmpty(assetName))
                        {
                            entityAssetNames.Add(assetName);
                        }
                        int entityId = ws.Cells[row, 2].GetValue<int>();
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
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var ws = package.Workbook.Worksheets[sheetName];
                if (ws != null)
                    package.Workbook.Worksheets.Delete(ws);
                package.Workbook.Worksheets.Add(sheetName);
                package.Save();
            }
        }

        private static void InsertToSheet(string filePath, string sheetName, string[] headers, List<object[]> data)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var ws = package.Workbook.Worksheets[sheetName];
                if (ws != null)
                    package.Workbook.Worksheets.Delete(ws);
                ws = package.Workbook.Worksheets.Add(sheetName);
                for (int col = 0; col < headers.Length; col++)
                {
                    ws.Cells[1, col + 1].Value = headers[col];
                }
                for (int i = 0; i < data.Count; i++)
                {
                    for (int col = 0; col < data[i].Length; col++)
                    {
                        ws.Cells[i + 2, col + 1].Value = data[i][col];
                    }
                }
                package.Save();
            }
        }
    }
}

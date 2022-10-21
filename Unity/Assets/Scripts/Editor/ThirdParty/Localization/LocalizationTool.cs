using System;
using System.Collections.Generic;
using System.IO;
using UGF;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;

namespace I2.Loc
{
    /// <summary>
    /// 自动从Localization.xlsx中更新本地化数据
    /// </summary>
    public static class LocalizationTool
    {
        private static string LocalizationFileDirectory { get; } = $"{Application.dataPath}/../../Develop/";
        private static string LocalizationFileName { get; } = "Localization.xlsx";
        private static string LocalizationFilePath { get; set; }
        private static FileSystemWatcher m_Watcher;
        private const string LoadTimeSaveKey = "Editor.LocalizationLoadTime";

        [InitializeOnLoadMethod]
        public static void OnInitialize()
        {
            LocalizationFilePath = Path.Combine(LocalizationFileDirectory, LocalizationFileName);
            
            m_Watcher = new FileSystemWatcher();
            m_Watcher.Filter = LocalizationFileName;
            m_Watcher.Path = LocalizationFileDirectory;
            m_Watcher.IncludeSubdirectories = false;
            m_Watcher.EnableRaisingEvents = true;

            m_Watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            m_Watcher.Changed += OnCSVFileChanged;

            if (PlayerPrefs.HasKey(LoadTimeSaveKey))//放置代码修改时反复加载
            {
                FileInfo excelFileInfo = new FileInfo(LocalizationFilePath);
                if (PlayerPrefs.GetString(LoadTimeSaveKey) != excelFileInfo.LastWriteTime.ToString())
                {
                    Loom.Post(ImportFromCSV);
                }
            }
            else
            {
                Loom.Post(ImportFromCSV);
            }
            
            Loom.Post(LoadLanguageSource);
        }

        [MenuItem("Tools/更新本地化")]
        public static void ImportFromCSV()
        {
            LanguageSourceAsset languageSourceAsset = AssetDatabase.LoadAssetAtPath<LanguageSourceAsset>(AssetUtility.GetLocalizationAsset());
            languageSourceAsset.SourceData.Import_CSV(string.Empty, ExcelToCSV(LocalizationFilePath), eSpreadsheetUpdateMode.Replace);
            EditorUtility.SetDirty(languageSourceAsset);
            AssetDatabase.Refresh();
            FileInfo excelFileInfo = new FileInfo(LocalizationFilePath);
            PlayerPrefs.SetString(LoadTimeSaveKey, excelFileInfo.LastWriteTime.ToString());
            Debug.Log("本地化xlsx已加载！");
        }

        [MenuItem("Tools/重载本地化")]
        public static void LoadLanguageSource()
        {
            LanguageSourceAsset languageSourceAsset = AssetDatabase.LoadAssetAtPath<LanguageSourceAsset>(AssetUtility.GetLocalizationAsset());
            LocalizationManager.UpdateByOneSource(languageSourceAsset);
        }

        private static void OnCSVFileChanged(object sender, FileSystemEventArgs e)
        {
            Loom.Post(ImportFromCSV);
        }

        private static List<string[]> ExcelToCSV(string excelFilePath)
        {
            List<string[]> csv = new List<string[]>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using FileStream fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using ExcelPackage excelPackage = new ExcelPackage(fileStream);
            for (int i = 0; i < excelPackage.Workbook.Worksheets.Count; i++)
            {
                ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[i];
                if (sheet.Name.Contains("~"))
                    continue;
                if(sheet.Dimension.End.Row < 2)
                    continue;
                int rawColumnCount = 0;
                bool hasEmpty = false;
                for (int j = 1; j <= sheet.Dimension.End.Column; j++)
                {
                    if (!hasEmpty)
                    {
                        if (sheet.Cells[1, j].Value == null)
                        {
                            hasEmpty = true;
                            continue;
                        }
                        rawColumnCount++;
                    }
                    else
                    {
                        if (sheet.Cells[1, j].Value != null)
                        {
                            throw new Exception($"{excelFilePath}：第1行有为空的列！");
                        }
                    }
                }
                
                for (int row = 1; row <= sheet.Dimension.End.Row; row++)
                {
                    if(row == 2)//第二行为注释，跳过
                        continue;
                    string[] strArray = new string[rawColumnCount];
                    for (int clo = 1; clo <= rawColumnCount; clo++)
                    {
                        object value = sheet.Cells[row, clo].Value;
                        if (value == null)
                        {
                            strArray[clo - 1] = string.Empty;
                        }
                        else
                        {
                            strArray[clo - 1] = value.ToString();
                        }
                    }
                    csv.Add(strArray);
                }
            }

            return csv;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Luban;
using SimpleJSON;

namespace ThunderFireUITool
{
    [UXInitialize(9999)]
    public static partial class EditorLocalizationTool
    {
        static EditorLocalizationTool()
        {
            LocalizationHelper.SetEditorGetStringFunc(GetString);
        }

        private static string[] m_AllKeys;
        public static string[] AllKeys
        {
            get
            {
                TryRefreshData();
                return m_AllKeys;
            }
        }

        private static readonly Dictionary<string, Dictionary<LocalizationHelper.LanguageType, string>> s_Dictionary = new Dictionary<string, Dictionary<LocalizationHelper.LanguageType, string>>();
        private static readonly List<FileInfo> s_AssetFileInfos = new List<FileInfo>();

        private static void TryRefreshData()
        {
            if (s_Dictionary.Count < 1)
            {
                RefreshData();
            }
            else
            {
                bool needRefresh = false;
                foreach (var fileInfo in s_AssetFileInfos)
                {
                    FileInfo fi = new FileInfo(fileInfo.FullName);
                    if (fi.CreationTime != fileInfo.CreationTime || fi.LastWriteTime != fileInfo.LastWriteTime)
                    {
                        needRefresh = true;
                        break;
                    }
                }
                if (needRefresh)
                {
                    RefreshData();
                }
            }
        }

        private static void RefreshData()
        {
            s_Dictionary.Clear();
            s_AssetFileInfos.Clear();
            m_AllKeys = Array.Empty<string>();
            foreach (LocalizationHelper.LanguageType languageType in ReadyLanguageTypes)
            {
                string asset = GetLocalizationAsset(languageType);
                s_AssetFileInfos.Add(new FileInfo(asset));
                if (asset.EndsWith("bytes"))
                {
                    ByteBuf byteBuf = new ByteBuf(File.ReadAllBytes(asset));
                    while (byteBuf.NotEmpty)
                    {
                        string dictionaryKey = byteBuf.ReadString();
                        string dictionaryValue = byteBuf.ReadString();
                        if (!s_Dictionary.TryGetValue(dictionaryKey, out Dictionary<LocalizationHelper.LanguageType, string> lDict))
                        {
                            lDict = new Dictionary<LocalizationHelper.LanguageType, string>();
                            s_Dictionary.Add(dictionaryKey, lDict);
                        }
                        lDict.Add(languageType, dictionaryValue);
                    }
                }
                else
                {
                    JSONObject jsonObject = (JSONObject)JSONNode.Parse(File.ReadAllText(asset));
                    foreach (KeyValuePair<string, JSONNode> pair in jsonObject)
                    {
                        string dictionaryKey = pair.Key;
                        string dictionaryValue = pair.Value;
                        if (!s_Dictionary.TryGetValue(dictionaryKey, out Dictionary<LocalizationHelper.LanguageType, string> lDict))
                        {
                            lDict = new Dictionary<LocalizationHelper.LanguageType, string>();
                            s_Dictionary.Add(dictionaryKey, lDict);
                        }
                        lDict.Add(languageType, dictionaryValue);
                    }
                }
            }
            m_AllKeys = s_Dictionary.Keys.ToArray();
        }

        public static string GetString(LocalizationHelper.LanguageType languageType, string key, string defaultString = "<UNKNOWN>")
        {
            TryRefreshData();
            if (!s_Dictionary.TryGetValue(key, out Dictionary<LocalizationHelper.LanguageType, string> lDict))
            {
                return defaultString;
            }
            return lDict.GetValueOrDefault(languageType, defaultString);
        }

        public static Dictionary<LocalizationHelper.LanguageType, string> GetStringLanguageMap(string key)
        {
            TryRefreshData();
            return s_Dictionary.GetValueOrDefault(key);
        }

        public static void Clear()
        {
            s_Dictionary.Clear();
            s_AssetFileInfos.Clear();
            m_AllKeys = Array.Empty<string>();
        }
    }
}

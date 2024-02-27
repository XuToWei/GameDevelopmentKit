using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework.Localization;
using Luban;
using SimpleJSON;
using UnityEditor;

namespace Game.Editor
{
    public static class LocalizationTool
    {
        private static string[] m_AllKeys;
        public static string[] AllKeys
        {
            get
            {
                TryRefreshData();
                return m_AllKeys;
            }
        }

        private static readonly Dictionary<string, Dictionary<Language, string>> s_Dictionary = new Dictionary<string, Dictionary<Language, string>>();
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
            foreach (Language language in LocalizationReadyLanguage.Languages)
            {
                string asset = AssetUtility.GetLocalizationAsset(language);
                s_AssetFileInfos.Add(new FileInfo(asset));
                if (asset.EndsWith("bytes"))
                {
                    ByteBuf byteBuf = new ByteBuf(File.ReadAllBytes(asset));
                    while (byteBuf.NotEmpty)
                    {
                        string dictionaryKey = byteBuf.ReadString();
                        string dictionaryValue = byteBuf.ReadString();
                        if (!s_Dictionary.TryGetValue(dictionaryKey, out Dictionary<Language, string> lDict))
                        {
                            lDict = new Dictionary<Language, string>();
                            s_Dictionary.Add(dictionaryKey, lDict);
                        }
                        lDict.Add(language, dictionaryValue);
                    }
                }
                else
                {
                    JSONObject jsonObject = (JSONObject)JSONNode.Parse(File.ReadAllText(asset));
                    foreach (KeyValuePair<string, JSONNode> pair in jsonObject)
                    {
                        string dictionaryKey = pair.Key;
                        string dictionaryValue = pair.Value;
                        if (!s_Dictionary.TryGetValue(dictionaryKey, out Dictionary<Language, string> lDict))
                        {
                            lDict = new Dictionary<Language, string>();
                            s_Dictionary.Add(dictionaryKey, lDict);
                        }
                        lDict.Add(language, dictionaryValue);
                    }
                }
            }
            m_AllKeys = s_Dictionary.Keys.ToArray();
            Localize.EditorLocalizationAllKey = m_AllKeys;
            Localize.EditorLocalizationReadyLanguage = LocalizationReadyLanguage.Languages;
            Selection.activeGameObject = null;
        }

        public static string GetString(Language language, string key, string defaultString = "<UNKNOWN>")
        {
            TryRefreshData();
            if (!s_Dictionary.TryGetValue(key, out Dictionary<Language, string> lDict))
            {
                return defaultString;
            }
            return lDict.GetValueOrDefault(language, defaultString);
        }
    }
}

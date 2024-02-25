using System;
using System.Collections.Generic;
using System.Linq;
using Luban;
using SimpleJSON;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public static partial class EditorLocalizationTool
    {
        public static string[] AllKeys
        {
            get;
            private set;
        }
        
        private static readonly Dictionary<string, Dictionary<LocalizationHelper.LanguageType, string>> s_Dictionary = new Dictionary<string, Dictionary<LocalizationHelper.LanguageType, string>>();

        public static void TryRefreshData()
        {
            if (s_Dictionary.Count < 1)
            {
                RefreshData();
            }
        }
        
        public static void RefreshData()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            s_Dictionary.Clear();
            AllKeys = null;
            foreach (LocalizationHelper.LanguageType languageType in ReadyLanguageTypes)
            {
                string asset = GetLocalizationAsset(languageType);
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(asset);
                if (textAsset == null)
                {
                    throw new Exception($"Localization Language asset : {asset} is not exist!");
                }
                if (asset.EndsWith("bytes"))
                {
                    ByteBuf byteBuf = new ByteBuf(textAsset.bytes);
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
                    JSONObject jsonObject = (JSONObject)JSONNode.Parse(textAsset.text);
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
            AllKeys = s_Dictionary.Keys.ToArray();
        }

        public static string GetString(LocalizationHelper.LanguageType languageType, string key, string defaultString = "<UNKNOWN>")
        {
            if (!s_Dictionary.TryGetValue(key, out Dictionary<LocalizationHelper.LanguageType, string> lDict))
            {
                return defaultString;
            }
            return lDict.GetValueOrDefault(languageType, defaultString);
        }

        public static Dictionary<LocalizationHelper.LanguageType, string> GetStringLanguageMap(string key)
        {
            return s_Dictionary.GetValueOrDefault(key);
        }
    }
}

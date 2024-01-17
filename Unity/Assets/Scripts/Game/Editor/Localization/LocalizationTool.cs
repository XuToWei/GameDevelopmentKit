using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Localization;
using Luban;
using SimpleJSON;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class LocalizationTool
    {
        public static string[] AllKeys
        {
            get;
            private set;
        }
        
        private static readonly Dictionary<string, Dictionary<Language, string>> s_Dictionary = new Dictionary<string, Dictionary<Language, string>>();

        public static void TryRefreshData()
        {
            if (s_Dictionary.Count < 1)
            {
                RefreshData();
            }
        }
        
        public static void RefreshData()
        {
            s_Dictionary.Clear();
            AllKeys = null;
            foreach (Language language in LocalizationReadyLanguage.Languages)
            {
                string asset = AssetUtility.GetLocalizationAsset(language);
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
                    JSONObject jsonObject = (JSONObject)JSONNode.Parse(textAsset.text);
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
            AllKeys = s_Dictionary.Keys.ToArray();
            Localize.EditorLocalizationAllKey = AllKeys;
            Localize.EditorLocalizationReadyLanguage = LocalizationReadyLanguage.Languages;
        }

        public static string GetString(Language language, string key)
        {
            if (!s_Dictionary.TryGetValue(key, out Dictionary<Language, string> lDict))
            {
                return "<UNKNOWN>";
            }
        
            if (!lDict.TryGetValue(language, out var value))
            {
                return "<UNKNOWN>";
            }
        
            return value;
        }
    }
}

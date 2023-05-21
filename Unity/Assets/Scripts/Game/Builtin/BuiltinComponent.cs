using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

#if UNITY_EDITOR
using DG.DemiEditor;
using UnityEditor;
#endif

namespace Game
{
    public class BuiltinComponent : GameFrameworkComponent
    {
        [SerializeField]
        private BuiltinBuildInfo m_BuildInfoRelease;
        
        [SerializeField]
        private BuiltinBuildInfo m_BuildInfoDebug;

        [SerializeField]
        private BuiltinLocalizationDictionary[] m_LocalizationDictionaries;
        
        [SerializeField]
        private BuiltinUpdateResourceForm m_UpdateResourceFormTemplate;
        
        [SerializeField]
        private BuiltinDialogForm m_DialogFormTemplate;

        public BuiltinBuildInfo BuildInfo => GameEntry.Debugger.ActiveWindow ? m_BuildInfoDebug : m_BuildInfoRelease;
        public BuiltinUpdateResourceForm UpdateResourceFormTemplate => m_UpdateResourceFormTemplate;
        public BuiltinDialogForm DialogFormTemplate => m_DialogFormTemplate;

        public void InitDefaultDictionary()
        {
            foreach (var dictionary in m_LocalizationDictionaries)
            {
                if (dictionary.Language == GameEntry.Localization.Language)
                {
                    foreach (var kv in dictionary.Dictionary)
                    {
                        if (!GameEntry.Localization.AddRawString(kv.Key, kv.Value))
                        {
                            Log.Warning("Can not add raw string with key '{0}' which may be invalid or duplicate.", kv.Key);
                            return;
                        }
                    }
                    return;
                }
            }
            Log.Warning("Can not find language '{0}'.", GameEntry.Localization.Language);
        }

        public BuiltinDialogForm OpenDialogForm(BuiltinDialogParams dialogParams)
        {
            BuiltinDialogForm dialogForm = Instantiate(m_DialogFormTemplate);
            dialogForm.Open(dialogParams);
            return dialogForm;
        }

#if UNITY_EDITOR
        [Button("Collect All Localization Dictionary")]
        public void CollectAllLocalizationDictionary()
        {
            string[] guids = AssetDatabase.FindAssets("t:BuiltinLocalizationDictionary");
            m_LocalizationDictionaries = new BuiltinLocalizationDictionary[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                m_LocalizationDictionaries[i] = AssetDatabase.LoadAssetAtPath<BuiltinLocalizationDictionary>(assetPath);
            }

            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }

            HashSet<string> allKeys = new HashSet<string>();
            foreach (var dictionary in m_LocalizationDictionaries)
            {
                foreach (var key in dictionary.Dictionary.Keys)
                {
                    if (!allKeys.Contains(key))
                    {
                        allKeys.Add(key);
                    }
                }
            }
            foreach (var dictionary in m_LocalizationDictionaries)
            {
                foreach (var key in dictionary.Dictionary.Keys)
                {
                    if (!allKeys.Contains(key))
                    {
                        Log.Error("Dictionary({0}) key {1} is missing!", dictionary.name, key);
                    }
                }
            }
        }
#endif
    }
}

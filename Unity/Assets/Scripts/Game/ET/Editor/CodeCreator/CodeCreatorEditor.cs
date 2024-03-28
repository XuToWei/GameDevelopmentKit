using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class CodeCreatorEditor : EditorWindow
    {
        private const string CurCodeCreatorIndexSaveKey = "ETCodeCreator_CurCodeCreatorIndex";

        private readonly List<ICodeCreator> m_CodeCreatorInstances = new List<ICodeCreator>();
        private string[] m_CodeCreatorTypeNames;
        private int m_CurCodeCreatorIndex;
        private string m_CodeName;

        [MenuItem("ET/Code Creator")]
        public static void ShowWindow()
        {
            CodeCreatorEditor window = GetWindow<CodeCreatorEditor>("ET Code Creator", DockDefine.Types);
            window.minSize = new Vector2(600f, 400f);
        }

        private void OnEnable()
        {
            this.m_CodeCreatorInstances.Clear();
            this.m_CodeCreatorTypeNames = null;
            
            var types = this.GetType().Assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof (ICodeCreator).IsAssignableFrom(x));
            List<string> typeNames = new List<string>();
            foreach (Type cType in types)
            {
                typeNames.Add(cType.Name);
                ICodeCreator codeCreator = (ICodeCreator)Activator.CreateInstance(cType);
                this.m_CodeCreatorInstances.Add(codeCreator);
                codeCreator.OnEnable();
            }
            this.m_CodeCreatorTypeNames = typeNames.ToArray();

            this.m_CurCodeCreatorIndex = EditorPrefs.GetInt(CurCodeCreatorIndexSaveKey, 0);
            if (this.m_CurCodeCreatorIndex >= this.m_CodeCreatorTypeNames.Length)
            {
                this.m_CurCodeCreatorIndex = 0;
            }
        }

        private void OnGUI()
        {
            if (this.m_CodeCreatorTypeNames == null || this.m_CodeCreatorTypeNames.Length < 1)
            {
                EditorGUILayout.LabelField("No Available Code Creator!");
                return;
            }

            int index = this.m_CurCodeCreatorIndex;
            this.m_CurCodeCreatorIndex = EditorGUILayout.Popup("Code Creator Type", index, this.m_CodeCreatorTypeNames);
            if (index != this.m_CurCodeCreatorIndex)
            {
                EditorPrefs.SetInt(CurCodeCreatorIndexSaveKey, this.m_CurCodeCreatorIndex);
            }

            this.m_CodeCreatorInstances[this.m_CurCodeCreatorIndex].OnGUI();
            
            this.m_CodeName = EditorGUILayout.TextField("Code Name", this.m_CodeName);
            
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.m_CodeName) || this.m_CodeName.Contains(" "));
            {
                if (GUILayout.Button("Generate Code"))
                {
                    this.m_CodeCreatorInstances[this.m_CurCodeCreatorIndex].GenerateCode(this.m_CodeName);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class UIFormCodeCreator : ICodeCreator
    {
        private const string UIFormCodeTemplateFile           = "Assets/Res/Editor/ET/Config/UIFormCodeTemplate.txt";
        private const string UIFormSystemCodeTemplateFile     = "Assets/Res/Editor/ET/Config/UIFormSystemCodeTemplate.txt";
        private const string MonoUIFormCodeTemplateFile       = "Assets/Res/Editor/ET/Config/MonoUIFormCodeTemplate.txt";

        private const string UIFormModelViewCodePath          = "Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI";
        private const string UIFormHotfixViewCodePath         = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI";

        private const string UIFormPrefabPath                 = "Assets/Res/UI/UIForm";
        private const string AddUIFormNameKey                 = "NeedAddComponentUIFormName";

        public void OnEnable()
        {
            
        }

        public void OnGUI()
        {
            
        }

        public void GenerateCode(string codeName)
        {
            GenerateUICode(codeName);
            GenerateUIFormPrefab(codeName);
            Debug.Log("生成完毕！");
        }

        private void GenerateUICode(string uiFormName)
        {
            void GenerateCS(string templateFile, string codeFile)
            {
                if (File.Exists(codeFile))
                {
                    throw new Exception($"{codeFile} already exist!");
                }
                if (!File.Exists(templateFile))
                {
                    throw new Exception($"{templateFile} does not exist!");
                }
                string dirName = Path.GetDirectoryName(codeFile);
                if (string.IsNullOrEmpty(dirName))
                {
                    throw new Exception($"{dirName} does not exist!");
                }
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                string template = File.ReadAllText(templateFile);
                string codeStr = template.Replace("#NAME#", uiFormName);
                File.WriteAllText(codeFile, codeStr);
                Debug.Log($"Generate cs:{codeFile}!");
            }
            
            GenerateCS(UIFormCodeTemplateFile,           $"{UIFormModelViewCodePath}/UI{uiFormName}/UIForm{uiFormName}.cs");
            GenerateCS(UIFormSystemCodeTemplateFile,     $"{UIFormHotfixViewCodePath}/UI{uiFormName}/UIForm{uiFormName}System.cs");
            GenerateCS(MonoUIFormCodeTemplateFile,       $"{UIFormModelViewCodePath}/UI{uiFormName}/MonoUIForm{uiFormName}.cs");
        }

        private void GenerateUIFormPrefab(string uiFormName)
        {
            string prefabPath = $"{UIFormPrefabPath}/UIForm{uiFormName}.prefab";
            if (File.Exists(prefabPath))
            {
                Debug.LogWarning($"{prefabPath} already exist!");
                return;
            }
            GameObject prefab = UGuiFormCreateTool.CreateUGuiFormPrefab($"UIForm{uiFormName}", $"{UIFormPrefabPath}/UIForm{uiFormName}.prefab");
            Selection.activeGameObject = prefab;
            EditorPrefs.SetString(AddUIFormNameKey, uiFormName);
            Debug.Log($"Generate prefab:{prefabPath}!");
        }

        [DidReloadScripts]
        private static void AddUIFormPrefabComponent()
        {
            if (!EditorPrefs.HasKey(AddUIFormNameKey))
                return;
            string uiFormName = EditorPrefs.GetString(AddUIFormNameKey);
            EditorPrefs.DeleteKey(AddUIFormNameKey);
            string prefabPath = $"{UIFormPrefabPath}/UIForm{uiFormName}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
                return;
            MonoScript classScript = AssetDatabase.LoadAssetAtPath<MonoScript>($"{UIFormModelViewCodePath}/UI{uiFormName}/MonoUIForm{uiFormName}.cs");
            if (classScript == null)
                return;
            Type componentType = classScript.GetClass();
            if (prefab.GetComponent(componentType) != null)
                return;
            prefab.AddComponent(componentType);
            Debug.Log($"Add component {componentType} to {prefab.name}!");
        }
    }
}
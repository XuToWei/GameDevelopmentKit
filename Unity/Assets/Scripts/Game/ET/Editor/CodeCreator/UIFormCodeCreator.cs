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

        private string m_AddComponentName;
        
        public void OnEnable()
        {
            
        }

        public void OnGUI()
        {
            
        }

        public void GenerateCode(string codeName)
        {
            GenerateUICode(codeName);
            GenerateUIPrefab(codeName);
            Debug.Log("生成完毕！");
        }

        private void GenerateUICode(string uiName)
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
                string codeStr = template.Replace("#NAME#", uiName);
                File.WriteAllText(codeFile, codeStr);
                Debug.Log($"Generate cs:{codeFile}!");
            }
            
            GenerateCS(UIFormCodeTemplateFile,           $"{UIFormModelViewCodePath}/UI{uiName}/UIForm{uiName}.cs");
            GenerateCS(UIFormSystemCodeTemplateFile,     $"{UIFormHotfixViewCodePath}/UI{uiName}/UIForm{uiName}System.cs");
            GenerateCS(MonoUIFormCodeTemplateFile,       $"{UIFormModelViewCodePath}/UI{uiName}/MonoUIForm{uiName}.cs");
        }

        private void GenerateUIPrefab(string uiName)
        {
            string prefabPath = $"{UIFormPrefabPath}/UIForm{uiName}.prefab";
            if (File.Exists(prefabPath))
            {
                throw new Exception($"{prefabPath} already exist!");
            }
            GameObject prefab = UGuiFormCreateTool.CreateUGuiFormPrefab($"UIForm{uiName}", $"{UIFormPrefabPath}/UIForm{uiName}.prefab");
            Selection.activeGameObject = prefab;
            EditorPrefs.SetString("NeedAddComponentUIName", uiName);
            Debug.Log($"Generate prefab:{prefabPath}!");
        }

        [DidReloadScripts]
        private static void AddUIPrefabComponent()
        {
            if (!EditorPrefs.HasKey("NeedAddComponentUIName"))
                return;
            string uiName = EditorPrefs.GetString("NeedAddComponentUIName");
            EditorPrefs.DeleteKey("NeedAddComponentUIName");
            string prefabPath = $"{UIFormPrefabPath}/UIForm{uiName}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
                return;
            MonoScript classScript = AssetDatabase.LoadAssetAtPath<MonoScript>($"{UIFormModelViewCodePath}/UI{uiName}/MonoUIForm{uiName}.cs");
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
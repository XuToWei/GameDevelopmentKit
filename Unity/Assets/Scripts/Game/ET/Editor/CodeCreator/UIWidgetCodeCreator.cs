using System;
using System.IO;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class UIWidgetCodeCreator : ICodeCreator
    {
        private const string UIWidgetComponentCodeTemplateFile      = "Assets/Res/Editor/ET/Config/UIWidgetCodeTemplate.txt";
        private const string UIWidgetSystemCodeTemplateFile         = "Assets/Res/Editor/ET/Config/UIWidgetSystemCodeTemplate.txt";
        private const string MonoUIWidgetCodeTemplateFile          = "Assets/Res/Editor/ET/Config/MonoUIWidgetCodeTemplate.txt";

        private const string UIWidgetModelViewCodePath              = "Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI";
        private const string UIWidgetHotfixViewCodePath             = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI";

        public void OnEnable()
        {
            
        }

        public void OnGUI()
        {
            
        }

        public void GenerateCode(string codeName)
        {
            GenerateUICode(codeName);
        }

        private static void GenerateUICode(string uiWidgetName)
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
                string codeStr = template.Replace("#NAME#", uiWidgetName);
                File.WriteAllText(codeFile, codeStr);
                Debug.Log($"Generate cs:{codeFile}!");
            }
            
            GenerateCS(UIWidgetComponentCodeTemplateFile,     $"{UIWidgetModelViewCodePath}/UI{uiWidgetName}/UIWidget{uiWidgetName}.cs");
            GenerateCS(UIWidgetSystemCodeTemplateFile,        $"{UIWidgetHotfixViewCodePath}/UI{uiWidgetName}/UIWidget{uiWidgetName}System.cs");
            GenerateCS(MonoUIWidgetCodeTemplateFile,         $"{UIWidgetModelViewCodePath}/UI{uiWidgetName}/MonoUIWidget{uiWidgetName}.cs");
            Debug.Log("生成完毕！");
        }
    }
}
using System;
using System.IO;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class UGFUIWidgetCodeCreator : ICodeCreator
    {
        private const string UGFUIWidgetComponentCodeTemplateFile      = "Assets/Res/Editor/ET/Config/UGFUIWidgetCodeTemplate.txt";
        private const string UGFUIWidgetSystemCodeTemplateFile         = "Assets/Res/Editor/ET/Config/UGFUIWidgetSystemCodeTemplate.txt";
        private const string UGFUIWidgetEventCodeTemplateFile          = "Assets/Res/Editor/ET/Config/UGFUIWidgetEventCodeTemplate.txt";

        private const string UGFUIWidgetModelViewCodePath              = "Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI/Widget";
        private const string UGFUIWidgetHotfixViewCodePath             = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI/Widget";

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
            
            GenerateCS(UGFUIWidgetComponentCodeTemplateFile,     $"{UGFUIWidgetModelViewCodePath}/UI{uiWidgetName}/UGFUIWidget{uiWidgetName}.cs");
            GenerateCS(UGFUIWidgetSystemCodeTemplateFile,        $"{UGFUIWidgetHotfixViewCodePath}/UI{uiWidgetName}/UGFUIWidget{uiWidgetName}System.cs");
            GenerateCS(UGFUIWidgetEventCodeTemplateFile,         $"{UGFUIWidgetHotfixViewCodePath}/UI{uiWidgetName}/UGFUIWidget{uiWidgetName}Event.cs");
            Debug.Log("生成完毕！");
        }
    }
}
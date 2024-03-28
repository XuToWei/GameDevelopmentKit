using System;
using System.IO;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class UGFUIFormCodeCreator : ICodeCreator
    {
        private const string UGFUIComponentCodeTemplateFile      = "Assets/Res/Editor/ET/Config/UGFUICodeTemplate.txt";
        private const string UGFUISystemCodeTemplateFile         = "Assets/Res/Editor/ET/Config/UGFUISystemCodeTemplate.txt";
        private const string UGFUIEventCodeTemplateFile          = "Assets/Res/Editor/ET/Config/UGFUIEventCodeTemplate.txt";

        private const string UGFUIModelViewCodePath              = "Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI";
        private const string UGFUIHotfixViewCodePath             = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI";

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

        private static void GenerateUICode(string uiName)
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
            
            GenerateCS(UGFUIComponentCodeTemplateFile,     $"{UGFUIModelViewCodePath}/UI{uiName}/UGFUI{uiName}.cs");
            GenerateCS(UGFUISystemCodeTemplateFile,        $"{UGFUIHotfixViewCodePath}/UI{uiName}/UGFUI{uiName}System.cs");
            GenerateCS(UGFUIEventCodeTemplateFile,         $"{UGFUIHotfixViewCodePath}/UI{uiName}/UGFUI{uiName}Event.cs");
            Debug.Log("生成完毕！");
        }
    }
}
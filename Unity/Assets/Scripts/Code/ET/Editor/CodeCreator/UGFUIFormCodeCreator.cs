using System;
using System.IO;

namespace ET.Editor
{
    internal sealed class UGFUIFormCodeCreator : ICodeCreator
    {
        private const string UGFUIViewCodeTemplateFile           = "Assets/Res/Editor/ET/Config/UGFUIViewCodeTemplate.txt";
        private const string UGFUIComponentCodeTemplateFile      = "Assets/Res/Editor/ET/Config/UGFUICodeTemplate.txt";
        private const string UGFUISystemCodeTemplateFile         = "Assets/Res/Editor/ET/Config/UGFUISystemCodeTemplate.txt";
        private const string UGFUIEventCodeTemplateFile          = "Assets/Res/Editor/ET/Config/UGFUIEventCodeTemplate.txt";

        private const string UGFUIModelViewCodePath              = "Assets/Scripts/Code/ET/Code/ModelView/Client/Game/UI";
        private const string UGFUIHotfixViewCodePath             = "Assets/Scripts/Code/ET/Code/HotfixView/Client/Game/UI";

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
                    return;
                if (!File.Exists(templateFile))
                {
                    throw new Exception($"{templateFile} is not exist!");
                }
                string dirName = Path.GetDirectoryName(codeFile);
                if (string.IsNullOrEmpty(dirName))
                {
                    throw new Exception($"{dirName} is not exist!");
                }
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                string template = File.ReadAllText(templateFile);
                string codeStr = template.Replace("#NAME#", uiName);
                File.WriteAllText(codeFile, codeStr);
            }

            GenerateCS(UGFUIViewCodeTemplateFile,          $"{UGFUIModelViewCodePath}/{uiName}/UGFUI{uiName}View.cs");
            GenerateCS(UGFUIComponentCodeTemplateFile,     $"{UGFUIModelViewCodePath}/{uiName}/UGFUI{uiName}.cs");
            GenerateCS(UGFUISystemCodeTemplateFile,        $"{UGFUIHotfixViewCodePath}/{uiName}/UGFUI{uiName}System.cs");
            GenerateCS(UGFUIEventCodeTemplateFile,         $"{UGFUIHotfixViewCodePath}/{uiName}/UGFUI{uiName}Event.cs");
        }
    }
}
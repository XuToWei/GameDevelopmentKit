using System;
using System.IO;

namespace ET.Editor
{
    internal sealed class UGFEntityCodeCreator : ICodeCreator
    {
        private const string UGFEntityComponentCodeTemplateFile      = "Assets/Res/Editor/ET/Config/UGFEntityCodeTemplate.txt";
        private const string UGFEntitySystemCodeTemplateFile         = "Assets/Res/Editor/ET/Config/UGFEntitySystemCodeTemplate.txt";
        private const string UGFEntityEventCodeTemplateFile          = "Assets/Res/Editor/ET/Config/UGFEntityEventCodeTemplate.txt";

        private const string UGFEntityModelViewCodePath              = "Assets/Scripts/Code/ET/Code/ModelView/Client/Game/Entity";
        private const string UGFEntityHotfixViewCodePath             = "Assets/Scripts/Code/ET/Code/HotfixView/Client/Game/Entity";

        public void OnEnable()
        {
            
        }

        public void OnGUI()
        {
            
        }

        public void GenerateCode(string codeName)
        {
            GenerateEntityCode(codeName);
        }

        private static void GenerateEntityCode(string entityName)
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
                string codeStr = template.Replace("#NAME#", entityName);
                File.WriteAllText(codeFile, codeStr);
            }
            
            GenerateCS(UGFEntityComponentCodeTemplateFile,     $"{UGFEntityModelViewCodePath}/{entityName}/UGFEntity{entityName}.cs");
            GenerateCS(UGFEntitySystemCodeTemplateFile,        $"{UGFEntityHotfixViewCodePath}/{entityName}/UGFEntity{entityName}System.cs");
            GenerateCS(UGFEntityEventCodeTemplateFile,         $"{UGFEntityHotfixViewCodePath}/{entityName}/UGFEntity{entityName}Event.cs");
        }
    }
}
using System;
using System.IO;
using UnityEngine;

namespace ET.Editor
{
    internal sealed class UGFEntityCodeCreator : ICodeCreator
    {
        private const string GFEntityComponentCodeTemplateFile      = "Assets/Res/Editor/ET/Config/GFEntityCodeTemplate.txt";
        private const string GFEntitySystemCodeTemplateFile         = "Assets/Res/Editor/ET/Config/GFEntitySystemCodeTemplate.txt";
        private const string MonoGFEntityCodeTemplateFile           = "Assets/Res/Editor/ET/Config/MonoGFEntityCodeTemplate.txt";

        private const string GFEntityModelViewCodePath              = "Assets/Scripts/Game/ET/Code/ModelView/Client/Game/GFEntity";
        private const string GFEntityHotfixViewCodePath             = "Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/GFEntity";

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
                string codeStr = template.Replace("#NAME#", entityName);
                File.WriteAllText(codeFile, codeStr);
                Debug.Log($"Generate cs:{codeFile}!");
            }
            
            GenerateCS(GFEntityComponentCodeTemplateFile,     $"{GFEntityModelViewCodePath}/{entityName}/GFEntity{entityName}.cs");
            GenerateCS(GFEntitySystemCodeTemplateFile,        $"{GFEntityHotfixViewCodePath}/{entityName}/GFEntity{entityName}System.cs");
            GenerateCS(MonoGFEntityCodeTemplateFile,         $"{GFEntityModelViewCodePath}/{entityName}/MonoGFEntity{entityName}.cs");
            Debug.Log("生成完毕！");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Editor;
using SimpleJSON;

namespace ET
{
    public static class GenerateUGFEntityId
    {
        private static readonly string s_LubanEntityAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtentity.json");
        private static readonly string s_LubanUIEntityAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtuientity.json");

        public static void GenerateCode()
        {
            if (ExcelExporter.ExcelExporter_Luban.IsEnableET)
            {
                GenerateCS(s_LubanEntityAsset, "ET.Client", "UGFEntityId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFEntityId.cs"));
                
                GenerateCS(s_LubanUIEntityAsset, "ET.Client", "UGFUIEntityId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFUIEntityId.cs"));
            }

            if (ExcelExporter.ExcelExporter_Luban.IsEnableGameHot)
            {
                GenerateCS(s_LubanEntityAsset, "Game.Hot", "EntityId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Generate/UGF/EntityId.cs"));
                
                GenerateCS(s_LubanUIEntityAsset, "Game.Hot", "UIEntityId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Generate/UGF/UIEntityId.cs"));
            }
        }
        
        private static void GenerateCS(string entityAsset, string nameSpaceName, string className, string codeFile)
        {
            if (string.IsNullOrEmpty(nameSpaceName))
            {
                throw new Exception($"Generate {className} code fail, namespace is empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"Generate {className} code fail, class name is empty.");
            }
            if (string.IsNullOrEmpty(codeFile))
            {
                throw new Exception($"Generate {className} code fail, code file is empty.");
            }
            
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(entityAsset));
            List<DREntity> drEntities = new List<DREntity>();
            foreach (var childNode in jsonNode.Children)
            {
                DREntity drEntity = DREntity.LoadJsonDREntity(childNode);
                drEntities.Add(drEntity);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("// This is an automatically generated class by Share.Tool. Please do not modify it.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"namespace {nameSpaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 实体编号");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine($"    public static class {className}");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DREntity drEntity in drEntities)
            {
                if (string.IsNullOrEmpty(drEntity.CSName))
                {
                    continue;
                }
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drEntity.Desc}");
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine($"        public const int {drEntity.CSName} = {drEntity.Id};");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            string codeContent = stringBuilder.ToString();
            string dir = Path.GetDirectoryName(codeFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(codeFile) || !string.Equals(codeContent, File.ReadAllText(codeFile)))
            {
                File.WriteAllText(codeFile, codeContent);
                Log.Info($"Generate code : {codeFile}!");
            }
        }
    }
}
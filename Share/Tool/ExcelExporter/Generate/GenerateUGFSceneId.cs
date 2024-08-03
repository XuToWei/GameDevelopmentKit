using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Editor;
using SimpleJSON;

namespace ET
{
    public static class GenerateUGFSceneId
    {
        private static readonly string s_LubanSceneAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtscene.json");

        public static void GenerateCode()
        {
            if (ExcelExporter.ExcelExporter_Luban.IsEnableET)
            {
                GenerateCS("ET.Client", "UGFSceneId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFSceneId.cs"));
            }

            if (ExcelExporter.ExcelExporter_Luban.IsEnableGameHot)
            {
                GenerateCS("Game.Hot", "SceneId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Runtime/Generate/UGF/SceneId.cs"));
            }
        }
        
        private static void GenerateCS(string nameSpaceName, string className, string codeFile)
        {
            if (string.IsNullOrEmpty(nameSpaceName))
            {
                throw new Exception($"Generate UGFSceneId code fail, namespace is empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"Generate UGFSceneId code fail, class name is empty.");
            }
            if (string.IsNullOrEmpty(codeFile))
            {
                throw new Exception($"Generate UGFSceneId code fail, code file is empty.");
            }
            
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(s_LubanSceneAsset));
            List<DRScene> drScenes = new List<DRScene>();
            foreach (var childNode in jsonNode.Children)
            {
                DRScene drScene = DRScene.LoadJsonDRScene(childNode);
                drScenes.Add(drScene);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("// This is an automatically generated class by Share.Tool. Please do not modify it.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"namespace {nameSpaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 场景编号");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine($"    public static class {className}");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DRScene drScene in drScenes)
            {
                if (string.IsNullOrEmpty(drScene.CSName))
                {
                    throw new Exception($"UGFSceneId {drScene.Id} CSName is empty!");
                }
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drScene.Desc}");
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine($"        public const int {drScene.CSName} = {drScene.Id};");
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Editor;
using SimpleJSON;

namespace ET
{
    public static class GenerateUGFUIFormId
    {
        private static readonly string s_LubanUIFormAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtuiform.json");

        public static void GenerateCode()
        {
            if (ExcelExporter.ExcelExporter_Luban.IsEnableET)
            {
                GenerateCS("ET.Client", "UGFUIFormId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFUIFormId.cs"));
            }

            if (ExcelExporter.ExcelExporter_Luban.IsEnableGameHot)
            {
                GenerateCS("Game.Hot", "UIFormId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Runtime/Generate/UGF/UIFormId.cs"));
            }
        }
        
        private static void GenerateCS(string nameSpaceName, string className, string codeFile)
        {
            if (string.IsNullOrEmpty(nameSpaceName))
            {
                throw new Exception($"Generate UGFUIFormId code fail, namespace is empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"Generate UGFUIFormId code fail, class name is empty.");
            }
            if (string.IsNullOrEmpty(codeFile))
            {
                throw new Exception($"Generate UGFUIFormId code fail, code file is empty.");
            }
            
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(s_LubanUIFormAsset));
            List<DRUIForm> drUIForms = new List<DRUIForm>();
            foreach (var childNode in jsonNode.Children)
            {
                DRUIForm drUIForm = DRUIForm.LoadJsonDRUIForm(childNode);
                drUIForms.Add(drUIForm);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("// This is an automatically generated class by Share.Tool. Please do not modify it.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"namespace {nameSpaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 界面编号");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine($"    public static class {className}");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DRUIForm drUIForm in drUIForms)
            {
                if (string.IsNullOrEmpty(drUIForm.CSName))
                {
                    throw new Exception($"UGFUIFormId {drUIForm.Id} CSName is empty!");
                }
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drUIForm.Desc}");
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine($"        public const int {drUIForm.CSName} = {drUIForm.Id};");
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using editor.cfg.UGF;
using SimpleJSON;
using UnityEditor;

namespace ET
{
    public static class UICreator
    {
        private const string ETUIFormCodeTemplateFile = "Assets/Res/Editor/ET/ETUIFormCodeTemplate.txt";
        private const string UIComponentCodeTemplateFile = "Assets/Res/Editor/ET/UIComponentCodeTemplate.txt";
        private const string UIComponentSystemCodeTemplateFile = "Assets/Res/Editor/ET/UIComponentSystemCodeTemplate.txt";
        private const string UIEventCodeTemplateFile = "Assets/Res/Editor/ET/UIEventCodeTemplate.txt";

        private const string UIModelViewCodePath = "Assets/Scripts/Codes/ModelView/Client/Game/UI";
        private const string UIHotfixViewCodePath = "Assets/Scripts/Codes/HotfixView/Client/Game/UI";

        private const string LubanUIFormAsset = "Assets/Res/Editor/Luban/ugf_dtuiform.json";
        private const string UIFormIdCodeFile = "Assets/Scripts/Codes/ModelView/Client/UGF/UI/UIFormId.cs";

        [MenuItem("ET/Create UICode By Config", false, 40)]
        public static void AutoCreate()
        {
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(LubanUIFormAsset));
            List<DRUIForm> drUIForms = new List<DRUIForm>();
            foreach (var childNode in jsonNode.Children)
            {
                DRUIForm drUIForm = DRUIForm.LoadJsonDRUIForm(childNode);
                drUIForms.Add(drUIForm);
                GenerateUICode(GetUIName(drUIForm));
            }
            GenerateUIFormIdCode(drUIForms);
            AssetDatabase.Refresh();
        }

        private static string GetUIName(DRUIForm drUIForm)
        {
            return drUIForm.AssetName.EndsWith("Form", StringComparison.OrdinalIgnoreCase)
                    ? drUIForm.AssetName.Substring(0, drUIForm.AssetName.Length - 4) : drUIForm.AssetName;
        }

        private static void GenerateUIFormIdCode(List<DRUIForm> drUIForms)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("namespace UGF");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 界面编号。");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine("    public enum UIFormId : byte");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        Undefined = 0,");
            foreach (DRUIForm drUIForm in drUIForms)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drUIForm.Desc}。");
                stringBuilder.AppendLine("        /// </summary>");
               
                stringBuilder.AppendLine($"        {GetUIName(drUIForm)} = {drUIForm.Id},");
            }
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            File.WriteAllText(UIFormIdCodeFile, stringBuilder.ToString());
        }

        private static void GenerateUICode(string uiName)
        {
            void GenerateCode(string templateFile, string codeFile)
            {
                if (File.Exists(codeFile))
                    return;
                string dirName = Path.GetDirectoryName(codeFile);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                string template = File.ReadAllText(templateFile);
                string codeStr = template.Replace("#NAME#", uiName);
                File.WriteAllText(codeFile, codeStr);
            }

            GenerateCode(ETUIFormCodeTemplateFile, $"{UIModelViewCodePath}/{uiName}/{uiName}Form.cs");
            GenerateCode(UIComponentCodeTemplateFile, $"{UIModelViewCodePath}/{uiName}/UI{uiName}Component.cs");
            GenerateCode(UIComponentSystemCodeTemplateFile, $"{UIHotfixViewCodePath}/{uiName}/UI{uiName}ComponentSystem.cs");
            GenerateCode(UIEventCodeTemplateFile, $"{UIHotfixViewCodePath}/{uiName}/UI{uiName}Event.cs");
        }
    }
}
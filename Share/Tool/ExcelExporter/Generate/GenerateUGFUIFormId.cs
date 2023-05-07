using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Editor;
using SimpleJSON;

namespace ET
{
    public class GenerateUGFUIFormId
    {
        private static readonly string UIFormIdCodeFile = Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI/UGFUIFormId.cs");
        private static readonly string LubanUIFormAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtuiform.json");

        public static void GenerateCode()
        {
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(LubanUIFormAsset));
            List<DRUIForm> drUIForms = new List<DRUIForm>();
            foreach (var childNode in jsonNode.Children)
            {
                DRUIForm drUIForm = DRUIForm.LoadJsonDRUIForm(childNode);
                drUIForms.Add(drUIForm);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("namespace ET.Client");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 界面编号。");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine("    public static class UGFUIFormId");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DRUIForm drUIForm in drUIForms)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drUIForm.Desc}。");
                stringBuilder.AppendLine("        /// </summary>");

                stringBuilder.AppendLine($"        public const int {GetUIName(drUIForm)} = {drUIForm.Id};");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            string codeContent = stringBuilder.ToString();
            if (!File.Exists(UIFormIdCodeFile) || !string.Equals(codeContent, File.ReadAllText(UIFormIdCodeFile)))
            {
                File.WriteAllText(UIFormIdCodeFile, codeContent);
                Console.WriteLine($"Generate code : {UIFormIdCodeFile}!");
            }
        }

        private static string GetUIName(DRUIForm drUIForm)
        {
            return drUIForm.AssetName.EndsWith("Form", StringComparison.OrdinalIgnoreCase)
                    ? drUIForm.AssetName.Substring(0, drUIForm.AssetName.Length - 4) : drUIForm.AssetName;
        }
    }
}
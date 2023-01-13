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
        private const string LubanEntityAsset = "../Unity/Assets/Res/Editor/Luban/dtentity.json";
        private const string EntityIdCodeFile = "../Unity/Assets/Scripts/Code/ET/Code/ModelView/Client/Game/Entity/UGFEntityId.cs";

        public static void GenerateCode()
        {
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(LubanEntityAsset));
            List<DREntity> drEntities = new List<DREntity>();
            foreach (var childNode in jsonNode.Children)
            {
                DREntity drEntity = DREntity.LoadJsonDREntity(childNode);
                drEntities.Add(drEntity);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("namespace ET.Client");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 实体编号。");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine("    public static class UGFEntityId");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DREntity drEntity in drEntities)
            {
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drEntity.Desc}。");
                stringBuilder.AppendLine("        /// </summary>");

                stringBuilder.AppendLine($"        public const int {GetEntityName(drEntity)} = {drEntity.Id};");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            File.WriteAllText(EntityIdCodeFile, stringBuilder.ToString());
        }

        private static string GetEntityName(DREntity drEntity)
        {
            return drEntity.AssetName.EndsWith("Entity", StringComparison.OrdinalIgnoreCase)
                    ? drEntity.AssetName.Substring(0, drEntity.AssetName.Length - 6) : drEntity.AssetName;
        }
    }
}
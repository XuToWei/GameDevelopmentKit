using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class DefaultCodeGeneratorRuleHelper : ICodeGeneratorRuleHelper
    {
        public string GetGeneratorCode(List<ReferenceBindComponent.BindObjectData> bindDataList, string  nameSpace,string className,object userData)
        {
            var repeatNameData =  bindDataList.Find(_ => _.IsRepeatName);

            if (repeatNameData != null)
            {
                throw new Exception($"绑定对象中存在同名{repeatNameData.FieldName},请修改后重新生成。");
            }

            var fieldNameInvalidData =  bindDataList.Find(_ => _.FieldNameIsInvalid);
            if (fieldNameInvalidData != null)
            {
                throw new Exception($"绑定数据中存在无效的字段命名{fieldNameInvalidData.FieldName},请修改后重新生成。");
            }

            StringBuilder stringBuilder = new StringBuilder(2048);

            List<string> usedNameSpaces = CodeGeneratorUtility.GetNameSpaces(bindDataList);
            foreach (var usedNameSpace in usedNameSpaces)
            {
                stringBuilder.AppendLine($"using {usedNameSpace};");
            }

            stringBuilder.AppendLine("");

            string indentation = string.Empty;
            if (!string.IsNullOrEmpty(nameSpace))
            {
                //命名空间
                stringBuilder.AppendLine("namespace " + nameSpace);
                stringBuilder.AppendLine("{");
                indentation = "\t";
            }

            //类名
            stringBuilder.AppendLine($"{indentation}public partial class {className}");
            stringBuilder.AppendLine($"{indentation}{{");
            stringBuilder.AppendLine("");
            
            //组件字段
            foreach (var data in bindDataList)
            {
                stringBuilder.AppendLine(
                    $"{indentation}\tprivate {data.BindObject.GetType().Name} m_{data.FieldName};");
            }

            stringBuilder.AppendLine("");

            stringBuilder.AppendLine($"{indentation}\tprivate void InitBindObjects(GameObject go)");
            stringBuilder.AppendLine($"{indentation}\t{{");


            stringBuilder.AppendLine(
                $"{indentation}\t\t{nameof(ReferenceBindComponent)} bindComponent = go.GetComponent<{nameof(ReferenceBindComponent)}>();");
            stringBuilder.AppendLine("");

            //根据索引获取

            for (int i = 0; i < bindDataList.Count; i++)
            {
                ReferenceBindComponent.BindObjectData data = bindDataList[i];
                stringBuilder.AppendLine(
                    $"{indentation}\t\tm_{data.FieldName} = bindComponent.GetBindObject<{data.BindObject.GetType().Name}>({i});");
            }

            stringBuilder.AppendLine($"{indentation}\t}}");

            stringBuilder.AppendLine($"{indentation}}}");

            if (!string.IsNullOrEmpty(nameSpace))
            {
                stringBuilder.AppendLine("}");
            }

            return stringBuilder.ToString();
        }

        public bool GeneratorCodeAndWriteToFile(List<ReferenceBindComponent.BindObjectData> bindDataList, string  nameSpace, string className, string folderPath,object userData)
        {
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"代码保存路径{folderPath}无效");
                return false;
            }

            string str = GetGeneratorCode(bindDataList, nameSpace,className,userData);
            string filePath = $"{folderPath}/{className}.BindComponents.cs";
            if (File.Exists(filePath) && str == File.ReadAllText(filePath))
            {
                Debug.Log("文件内容相同。不需要重新生成。");
                return true;
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(str);
            }

            AssetDatabase.Refresh();
            Debug.Log($"代码生成成功,生成路径: {folderPath}/{className}.BindComponents.cs");

            return true;
        }
    }
}
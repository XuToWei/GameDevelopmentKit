using System.Collections.Generic;
using System.Text;
using CodeBind.Editor;

namespace CodeBind
{
    /// <summary>
    /// 游戏工程的 CodeBind 代码生成 Customizer。
    /// </summary>
    sealed class GameCodeBindCustomizer : ICodeBindCustomizer
    {
        public int Priority => 1;

        public string GetFieldName(string name)
        {
            return $"m_{name}";
        }

        public string GetPropertyName(string name)
        {
            return name;
        }

        public string GenerateExtraCode(string nameSpace, string className, List<CodeBindData> bindDatas, SortedDictionary<string, List<CodeBindData>> bindArrayDataDict, string indentation)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (CodeBindData bindData in bindDatas)
            {
                if (bindData.BindType != typeof(StateController.StateControllerMono))
                {
                    continue;
                }
                StateController.StateControllerMono controller = bindData.BindTransform.GetComponent<StateController.StateControllerMono>();
                if (controller == null)
                {
                    continue;
                }
                string controllerPropertyName = GetPropertyName($"{bindData.BindName}{bindData.BindPrefix}");
                foreach (var data in controller.EditorDatas)
                {
                    string[] stateNames = controller.GetStateNames(data.Name);
                    if (stateNames == null)
                    {
                        continue;
                    }
                    string dataBindName = $"{bindData.BindName}{data.Name}";
                    string dataFieldName = GetFieldName($"{dataBindName}StateControllerData");
                    string dataPropertyName = GetPropertyName($"{dataBindName}StateControllerData");
                    string stateNameClassName = $"{dataBindName}StateName";
                    string stateIndexClassName = $"{dataBindName}StateIndex";
                    stringBuilder.AppendLine($"{indentation}private StateController.StateControllerData {dataFieldName};");
                    stringBuilder.AppendLine($"{indentation}public StateController.StateControllerData {dataPropertyName} => this.{dataFieldName} ??= this.{controllerPropertyName}.GetData(\"{data.Name}\");");
                    stringBuilder.AppendLine($"{indentation}public static class {stateNameClassName}");
                    stringBuilder.AppendLine($"{indentation}{{");
                    foreach (var stateName in stateNames)
                    {
                        stringBuilder.AppendLine($"{indentation}\tpublic const string {stateName} = \"{stateName}\";");
                    }
                    stringBuilder.AppendLine($"{indentation}}}");
                    stringBuilder.AppendLine($"{indentation}public static class {stateIndexClassName}");
                    stringBuilder.AppendLine($"{indentation}{{");
                    for (int index = 0; index < stateNames.Length; index++)
                    {
                        stringBuilder.AppendLine($"{indentation}\tpublic const int {stateNames[index]} = {index};");
                    }
                    stringBuilder.AppendLine($"{indentation}}}");
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
    }
}

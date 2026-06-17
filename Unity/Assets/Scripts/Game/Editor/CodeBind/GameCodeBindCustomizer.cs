using System.Collections.Generic;
using System.Text;
using CodeBind.Editor;

namespace CodeBind
{
    /// <summary>
    /// 游戏工程的 CodeBind 代码生成 Customizer。
    /// 自定义命名风格（字段首字母小写），并为 StateControllerMono 绑定生成额外的状态访问代码
    /// （原 CodeBind 包中 STATE_CONTROLLER_CODE_BIND 逻辑迁移而来）。
    /// 使用方式：放入一个同时引用 CodeBind.Editor 和 StateController 的 Editor 程序集即可生效。
    /// 备注：
    ///   1. 原逻辑使用绑定变量名(BindName)作为生成成员前缀，这里改用属性名(member.Name)，故生成的成员名会带上类型后缀。
    ///   2. 原 BaseBinder 中“根节点有 StateControllerMono 时自动生成 Root 绑定”的逻辑无法用 ICodeBindCustomizer 表达，
    ///      若仍需要该功能需在绑定流程中另行处理，原逻辑见文件末尾注释。
    /// </summary>
    sealed class GameCodeBindCustomizer : ICodeBindCustomizer
    {
        public int Priority => 1;

        public string GetFieldName(string bindName, string bindPrefix)
        {
            return $"m_{bindName}{bindPrefix}";
        }

        public string GetPropertyName(string bindName, string bindPrefix)
        {
            return $"{FirstCharToLower(bindName)}{bindPrefix}";
        }

        public string GetArrayFieldName(string bindName, string bindPrefix)
        {
            return $"m_{bindName}{bindPrefix}Array";
        }

        public string GetArrayPropertyName(string bindName, string bindPrefix)
        {
            return $"{FirstCharToLower(bindName)}{bindPrefix}Array";
        }

        private string FirstCharToLower(string value)
        {
            if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            {
                return value;
            }
            return $"{char.ToLowerInvariant(value[0])}{value.Substring(1)}";
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
                string controllerPropertyName = GetPropertyName(bindData.BindName, bindData.BindPrefix);
                foreach (var data in controller.EditorDatas)
                {
                    string[] stateNames = controller.GetStateNames(data.Name);
                    if (stateNames == null)
                    {
                        continue;
                    }
                    string dataBindName = $"{bindData.BindName}{data.Name}";
                    string dataFieldName = GetFieldName(dataBindName, "StateControllerData");
                    string dataPropertyName = GetPropertyName(dataBindName, "StateControllerData");
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
                stringBuilder.AppendLine("");
            }
            return stringBuilder.ToString();
        }
    }
}

using System.Collections.Generic;
using System.Text;
using CodeBind.Editor;

namespace CodeBind
{
    /// <summary>
    /// 游戏工程的 CodeBind 代码生成 Customizer。
    /// </summary>
    sealed class GameCodeBindCustomizer : IBindingCodeCustomizer
    {
        public int Priority => 1;

        public string GetSerializedFieldName(string memberName)
        {
            return $"m_{memberName}";
        }

        public string GetPublicPropertyName(string memberName)
        {
            return memberName;
        }

        public string BuildAdditionalSource(string namespaceName, string className,
            List<BindingDescriptor> singleBindings,
            SortedDictionary<string, List<BindingDescriptor>> arrayBindingsByMemberName,
            string indentation)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (BindingDescriptor binding in singleBindings)
            {
                if (binding.TargetType != typeof(StateController.StateController))
                {
                    continue;
                }
                StateController.StateController controller =
                    binding.SourceTransform.GetComponent<StateController.StateController>();
                if (controller == null)
                {
                    continue;
                }
                string controllerPropertyName =
                    GetPublicPropertyName($"{binding.VariableName}{binding.TargetToken}");
                foreach (StateController.StateGroup group in controller.EditorGroups)
                {
                    string[] stateNames = controller.GetStateNames(group.Name);
                    if (stateNames == null)
                    {
                        continue;
                    }
                    string groupBindingName = $"{binding.VariableName}{group.Name}";
                    string groupFieldName = GetSerializedFieldName($"{groupBindingName}StateGroup");
                    string groupPropertyName = GetPublicPropertyName($"{groupBindingName}StateGroup");
                    string stateNameClassName = $"{groupBindingName}StateName";
                    string stateIndexClassName = $"{groupBindingName}StateIndex";
                    stringBuilder.AppendLine($"{indentation}private StateController.StateGroup {groupFieldName};");
                    stringBuilder.AppendLine($"{indentation}public StateController.StateGroup {groupPropertyName} => this.{groupFieldName} ??= this.{controllerPropertyName}.GetGroup(\"{group.Name}\");");
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

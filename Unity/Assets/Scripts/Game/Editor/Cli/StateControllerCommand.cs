using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Pipeline.Commands;
using Newtonsoft.Json;
using StateController;
using Unity.Pipeline;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using StateControllerComponent = StateController.StateController;

namespace Game.Editor
{
    /// <summary>Inspect and edit StateController groups, states, and binding nodes.</summary>
    // Pipeline uses Task return types to await commands and exclude them from synchronous job batches.
#pragma warning disable ET0501
    public static class StateControllerCommand
    {
        private const string ErrorCode = "STATECONTROLLER_ERROR";
        private const string Group = "game.statecontroller";

        // Task-returning entry points keep these Prefab/file operations out of transactional job batches.
        [CliGroupCommand(Group, "列出目标下的 StateController 状态组与状态。")]
        public static Task<object> List(
            [CliArg("asset_path", "Prefab 资源路径，优先于 instance_id/path/当前选择。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID，优先于 path。")] ObjectId? instanceId = null)
        {
            return Task.FromResult(Execute(new Arguments { AssetPath = assetPath, Path = path, InstanceId = instanceId }, "list"));
        }

        [CliGroupCommand(Group, "列出 StateBinding 节点，可按状态组和节点路径过滤。")]
        public static Task<object> ListNodes(
            [CliArg("asset_path", "Prefab 资源路径。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID。")] ObjectId? instanceId = null,
            [CliArg("group_name", "只返回绑定到该状态组的节点。")] string groupName = null,
            [CliArg("node_path", "节点路径，支持含根节点路径、相对路径或唯一节点名。")] string nodePath = null)
        {
            return Task.FromResult(Execute(new Arguments { AssetPath = assetPath, Path = path, InstanceId = instanceId,
                GroupName = groupName, NodePath = nodePath }, "list_nodes"));
        }

        [CliGroupCommand(Group, "切换状态；state_name 与 state_index 必须且只能提供一个。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> SetState(
            [CliArg("group_name", "状态组名。", Required = true)] string groupName,
            [CliArg("state_name", "目标状态名，与 state_index 二选一。")] string stateName = null,
            [CliArg("state_index", "目标状态的非负索引，与 state_name 二选一。")] int? stateIndex = null,
            [CliArg("asset_path", "Prefab 资源路径。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID。")] ObjectId? instanceId = null,
            [CliArg("controller_path", "目标有多个控制器时，用控制器路径消歧。")] string controllerPath = null)
        {
            return Task.FromResult(Execute(new Arguments { GroupName = groupName, StateName = stateName, StateIndex = stateIndex,
                AssetPath = assetPath, Path = path, InstanceId = instanceId, ControllerPath = controllerPath }, "set_state"));
        }

        [CliGroupCommand(Group, "新增状态组及可选的初始状态。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> AddGroup(
            [CliArg("group_name", "新增状态组名。", Required = true)] string groupName,
            [CliArg("states", "初始状态名的 JSON 数组，例如 [\"Normal\",\"Selected\"]；不得重复。")] string[] states = null,
            [CliArg("asset_path", "Prefab 资源路径。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID。")] ObjectId? instanceId = null,
            [CliArg("controller_path", "目标有多个控制器时，用控制器路径消歧。")] string controllerPath = null)
        {
            return Task.FromResult(Execute(new Arguments { GroupName = groupName, States = states,
                AssetPath = assetPath, Path = path, InstanceId = instanceId, ControllerPath = controllerPath }, "add_group"));
        }

        [CliGroupCommand(Group, "向已有状态组添加状态并对齐绑定节点。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> AddState(
            [CliArg("group_name", "已有状态组名。", Required = true)] string groupName,
            [CliArg("state_name", "新增状态名。", Required = true)] string stateName,
            [CliArg("asset_path", "Prefab 资源路径。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID。")] ObjectId? instanceId = null,
            [CliArg("controller_path", "目标有多个控制器时，用控制器路径消歧。")] string controllerPath = null)
        {
            return Task.FromResult(Execute(new Arguments { GroupName = groupName, StateName = stateName,
                AssetPath = assetPath, Path = path, InstanceId = instanceId, ControllerPath = controllerPath }, "add_state"));
        }

        [CliGroupCommand(Group, "为目标节点添加 StateBinding 并绑定状态组。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> AddStateNode(
            [CliArg("group_name", "已有状态组名。", Required = true)] string groupName,
            [CliArg("state_type", "StateBinding 子类名或完整类型名。", Required = true)] string stateType,
            [CliArg("asset_path", "Prefab 资源路径。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID。")] ObjectId? instanceId = null,
            [CliArg("node_path", "根节点下的路径，省略时操作根节点。")] string nodePath = null)
        {
            return Task.FromResult(Execute(new Arguments { GroupName = groupName, StateType = stateType,
                AssetPath = assetPath, Path = path, InstanceId = instanceId, NodePath = nodePath }, "add_state_node"));
        }

        private static object Execute(Arguments args, string action)
        {
            if (!GameCommandTarget.TryResolveRoot(args.AssetPath, args.Path, args.InstanceId,
                    out GameObject root, out bool isPrefabContents, out string assetPath,
                    out string targetLabel, out string error))
            {
                throw new GameCommandException(ErrorCode, error);
            }
            try
            {
                if (action == "add_state_node")
                {
                    return AddStateNode(args, root, targetLabel, isPrefabContents, assetPath);
                }
                if (action == "list_nodes")
                {
                    return ListNodes(args, root, targetLabel);
                }
                StateControllerComponent[] controllers = root.GetComponentsInChildren<StateControllerComponent>(true);
                if (controllers.Length == 0)
                {
                    throw new GameCommandException(ErrorCode, $"No StateController found under '{targetLabel}'.");
                }
                switch (action)
                {
                    case "list": return List(root, targetLabel, controllers);
                    case "set_state": return SetState(args, root, targetLabel, controllers, isPrefabContents, assetPath);
                    case "add_group": return AddGroup(args, root, targetLabel, controllers, isPrefabContents, assetPath);
                    case "add_state": return AddState(args, root, targetLabel, controllers, isPrefabContents, assetPath);
                    default: throw new GameCommandException(ErrorCode, $"Unhandled action: {action}");
                }
            }
            finally
            {
                if (isPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }

        private static object List(GameObject root, string targetLabel, StateControllerComponent[] controllers)
        {
            var controllerInfos = new List<object>();
            foreach (StateControllerComponent controller in controllers)
            {
                var datas = new List<object>();
                foreach (StateGroup data in controller.EditorGroups)
                {
                    datas.Add(new
                    {
                        name = data.Name,
                        selectedName = controller.GetCurrentStateName(data.Name),
                        selectedIndex = controller.GetCurrentStateIndex(data.Name),
                        states = controller.GetStateNames(data.Name) ?? Array.Empty<string>()
                    });
                }

                controllerInfos.Add(new
                {
                    path = GetRelativePath(root.transform, controller.transform),
                    datas
                });
            }

            return new
            {
                action = "list",
                target = targetLabel,
                controllers = controllerInfos
            };
        }

        private static object ListNodes(Arguments args, GameObject root, string targetLabel)
        {
            string dataFilter = args.GroupName;
            string nodeFilter = args.NodePath;

            var nodes = new List<object>();
            foreach (StateBinding state in root.GetComponentsInChildren<StateBinding>(true))
            {
                if (state == null)
                {
                    continue;
                }

                string nodePath = GetRelativePath(root.transform, state.transform);
                if (!string.IsNullOrEmpty(nodeFilter) &&
                    !MatchesNodePath(root.transform, state.transform, nodeFilter))
                {
                    continue;
                }

                var so = new SerializedObject(state);
                StateControllerComponent controller = state.GetComponentInParent<StateControllerComponent>(true);

                string mode;
                string booleanLogic = null;
                var bindings = new List<BindingInfo>();
                SerializedProperty dn = so.FindProperty("m_GroupName");
                if (dn != null)
                {
                    mode = "selectable";
                    bindings.Add(BuildBinding(dn.stringValue, so.FindProperty("m_ValueMappings")));
                }
                else
                {
                    mode = "boolean";
                    SerializedProperty logic = so.FindProperty("m_BooleanMode");
                    if (logic != null && logic.propertyType == SerializedPropertyType.Enum)
                    {
                        booleanLogic = SafeEnumName(logic);
                    }

                    SerializedProperty dn1 = so.FindProperty("m_GroupName1");
                    if (dn1 != null)
                    {
                        bindings.Add(BuildBinding(dn1.stringValue, so.FindProperty("m_ValueMappings1")));
                    }
                    SerializedProperty dn2 = so.FindProperty("m_GroupName2");
                    if (dn2 != null && !string.IsNullOrEmpty(dn2.stringValue))
                    {
                        bindings.Add(BuildBinding(dn2.stringValue, so.FindProperty("m_ValueMappings2")));
                    }
                }

                if (!string.IsNullOrEmpty(dataFilter) && !BindingsContainData(bindings, dataFilter))
                {
                    continue;
                }

                nodes.Add(new
                {
                    nodePath,
                    stateType = state.GetType().Name,
                    controllerPath = controller != null ? GetRelativePath(root.transform, controller.transform) : null,
                    mode,
                    booleanLogic,
                    bindings
                });
            }

            return new
            {
                action = "list_nodes",
                target = targetLabel,
                count = nodes.Count,
                nodes
            };
        }

        private static BindingInfo BuildBinding(string groupName, SerializedProperty stateValuesProp)
        {
            var slots = new List<object>();
            if (stateValuesProp != null && stateValuesProp.isArray)
            {
                for (int i = 0; i < stateValuesProp.arraySize; i++)
                {
                    SerializedProperty el = stateValuesProp.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = el.FindPropertyRelative("m_StateName");
                    SerializedProperty valueProp = el.FindPropertyRelative("m_Value");
                    slots.Add(new
                    {
                        stateName = nameProp != null ? nameProp.stringValue : null,
                        value = ReadSerializedValue(valueProp)
                    });
                }
            }

            return new BindingInfo
            {
                GroupName = groupName,
                StateValues = slots
            };
        }

        private static bool BindingsContainData(List<BindingInfo> bindings, string groupName)
        {
            foreach (BindingInfo binding in bindings)
            {
                if (binding.GroupName == groupName)
                {
                    return true;
                }
            }
            return false;
        }

        private static object ReadSerializedValue(SerializedProperty p)
        {
            if (p == null)
            {
                return null;
            }

            switch (p.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return p.boolValue;
                case SerializedPropertyType.Integer:
                    return p.intValue;
                case SerializedPropertyType.Float:
                    return p.floatValue;
                case SerializedPropertyType.String:
                    return p.stringValue;
                case SerializedPropertyType.Enum:
                    return SafeEnumName(p);
                case SerializedPropertyType.Color:
                    return $"#{ColorUtility.ToHtmlStringRGBA(p.colorValue)}";
                case SerializedPropertyType.Vector2:
                    return new { x = p.vector2Value.x, y = p.vector2Value.y };
                case SerializedPropertyType.Vector3:
                    return new { x = p.vector3Value.x, y = p.vector3Value.y, z = p.vector3Value.z };
                case SerializedPropertyType.Vector4:
                    return new { x = p.vector4Value.x, y = p.vector4Value.y, z = p.vector4Value.z, w = p.vector4Value.w };
                case SerializedPropertyType.ObjectReference:
                    return p.objectReferenceValue != null ? p.objectReferenceValue.name : null;
                default:
                    return $"({p.propertyType})";
            }
        }

        private static string SafeEnumName(SerializedProperty p)
        {
            string[] names = p.enumDisplayNames;
            int index = p.enumValueIndex;
            return index >= 0 && index < names.Length ? names[index] : index.ToString();
        }

        private static object SetState(Arguments args, GameObject root, string targetLabel,
            StateControllerComponent[] controllers, bool isPrefabContents, string assetPath)
        {
            string groupName = args.GroupName;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Missing 'group_name' for set_state.");
            }

            bool hasStateName = args.StateName != null;
            string stateName = args.StateName;
            bool hasStateIndex = args.StateIndex.HasValue;
            int stateIndex = args.StateIndex ?? -1;
            if (!hasStateName && !hasStateIndex)
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Provide 'state_name' or 'state_index' for set_state.");
            }
            if (hasStateName && hasStateIndex)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    "Provide only one of 'state_name' or 'state_index' for set_state.");
            }
            if (hasStateName && string.IsNullOrWhiteSpace(stateName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    "'state_name' must not be empty or whitespace for set_state.");
            }

            string controllerPath = args.ControllerPath;
            StateControllerComponent controller = ResolveController(root, controllers, controllerPath, groupName, out string resolveError);
            if (controller == null)
            {
                throw new GameCommandException(ErrorCode, resolveError);
            }

            string[] states = controller.GetStateNames(groupName);
            if (states == null)
            {
                throw new GameCommandException(ErrorCode,
                    $"Data '{groupName}' not found on controller '{GetRelativePath(root.transform, controller.transform)}'.");
            }

            if (hasStateName)
            {
                if (Array.IndexOf(states, stateName) < 0)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"State '{stateName}' not in data '{groupName}'. Available: [{string.Join(", ", states)}]");
                }
            }
            else
            {
                if (stateIndex < 0 || stateIndex >= states.Length)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"stateIndex {stateIndex} out of range for data '{groupName}' (count {states.Length}).");
                }
                stateName = states[stateIndex];
            }

            controller.SetCurrentStateName(groupName, stateName);
            Persist(controller, isPrefabContents, root, assetPath);

            return new
            {
                action = "set_state",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                groupName,
                stateName,
                stateIndex = Array.IndexOf(states, stateName)
            };
        }

        private static object AddGroup(Arguments args, GameObject root, string targetLabel,
            StateControllerComponent[] controllers, bool isPrefabContents, string assetPath)
        {
            string groupName = args.GroupName;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Missing 'group_name' for add_group.");
            }

            string controllerPath = args.ControllerPath;
            StateControllerComponent controller = ResolveController(root, controllers, controllerPath, null, out string resolveError);
            if (controller == null)
            {
                throw new GameCommandException(ErrorCode, resolveError);
            }

            var initialStates = ValidateStates(args.States);
            var so = new SerializedObject(controller);
            SerializedProperty datas = so.FindProperty("m_Groups");
            if (datas == null)
            {
                throw new GameCommandException(ErrorCode, "StateController.m_Groups not found (package layout changed?).");
            }

            for (int i = 0; i < datas.arraySize; i++)
            {
                if (datas.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == groupName)
                {
                    throw new GameCommandException(ErrorCode, $"Data '{groupName}' already exists on controller.");
                }
            }

            if (!isPrefabContents)
            {
                Undo.RecordObject(controller, "CLI StateController Add Data");
            }

            int index = datas.arraySize;
            datas.InsertArrayElementAtIndex(index);
            SerializedProperty dataEl = datas.GetArrayElementAtIndex(index);
            dataEl.FindPropertyRelative("m_Name").stringValue = groupName;
            SerializedProperty statesProp = dataEl.FindPropertyRelative("m_States");
            statesProp.ClearArray();
            foreach (string state in initialStates)
            {
                AddStateElement(statesProp, state);
            }
            so.ApplyModifiedPropertiesWithoutUndo();

            Persist(controller, isPrefabContents, root, assetPath);

            return new
            {
                action = "add_group",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                groupName,
                states = initialStates
            };
        }

        private static object AddState(Arguments args, GameObject root, string targetLabel,
            StateControllerComponent[] controllers, bool isPrefabContents, string assetPath)
        {
            string groupName = args.GroupName;
            string stateName = args.StateName;
            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(stateName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Provide both 'group_name' and 'state_name' for add_state.");
            }

            string controllerPath = args.ControllerPath;
            StateControllerComponent controller = ResolveController(root, controllers, controllerPath, groupName, out string resolveError);
            if (controller == null)
            {
                throw new GameCommandException(ErrorCode, resolveError);
            }

            var so = new SerializedObject(controller);
            SerializedProperty datas = so.FindProperty("m_Groups");
            SerializedProperty dataEl = null;
            for (int i = 0; i < datas.arraySize; i++)
            {
                SerializedProperty el = datas.GetArrayElementAtIndex(i);
                if (el.FindPropertyRelative("m_Name").stringValue == groupName)
                {
                    dataEl = el;
                    break;
                }
            }
            if (dataEl == null)
            {
                throw new GameCommandException(ErrorCode, $"Data '{groupName}' not found on controller.");
            }

            SerializedProperty statesProp = dataEl.FindPropertyRelative("m_States");
            for (int i = 0; i < statesProp.arraySize; i++)
            {
                if (statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == stateName)
                {
                    throw new GameCommandException(ErrorCode, $"State '{stateName}' already exists in data '{groupName}'.");
                }
            }

            if (!isPrefabContents)
            {
                Undo.RecordObject(controller, "CLI StateController Add State");
            }

            AddStateElement(statesProp, stateName);
            so.ApplyModifiedPropertiesWithoutUndo();

            string[] states = controller.GetStateNames(groupName) ?? Array.Empty<string>();
            int aligned = AlignEffectNodes(controller, groupName, states);
            Persist(controller, isPrefabContents, root, assetPath);

            return new
            {
                action = "add_state",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                groupName,
                stateName,
                states,
                alignedEffectNodes = aligned
            };
        }

        private static object AddStateNode(Arguments args, GameObject root, string targetLabel,
            bool isPrefabContents, string assetPath)
        {
            string stateTypeName = args.StateType;
            if (string.IsNullOrWhiteSpace(stateTypeName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Missing 'state_type' for add_state_node.");
            }
            string groupName = args.GroupName;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Missing 'group_name' for add_state_node.");
            }

            Type stateType = ResolveStateType(stateTypeName);
            if (stateType == null)
            {
                throw new GameCommandException(ErrorCode,
                    $"Binding type '{stateTypeName}' not found (must be a non-abstract StateBinding subclass).");
            }

            Transform node = root.transform;
            string nodePath = args.NodePath;
            if (!string.IsNullOrEmpty(nodePath))
            {
                node = FindNode(root.transform, nodePath);
                if (node == null)
                {
                    throw new GameCommandException(ErrorCode, $"Node '{nodePath}' not found under '{targetLabel}'.");
                }
            }

            StateControllerComponent controller = node.GetComponentInParent<StateControllerComponent>(true);
            if (controller == null)
            {
                throw new GameCommandException(ErrorCode,
                    $"Node '{GetRelativePath(root.transform, node)}' has no StateController ancestor.");
            }

            string[] states = controller.GetStateNames(groupName);
            if (states == null)
            {
                throw new GameCommandException(ErrorCode,
                    $"Data '{groupName}' not found on controller '{GetRelativePath(root.transform, controller.transform)}'.");
            }
            if (node.GetComponent(stateType) != null)
            {
                throw new GameCommandException(ErrorCode, $"Node already has a '{stateType.Name}' component.");
            }

            Component comp = node.gameObject.AddComponent(stateType);
            if (!isPrefabContents)
            {
                Undo.RegisterCreatedObjectUndo(comp, "CLI StateController Add State Node");
            }

            var so = new SerializedObject(comp);
            string boundField;
            if (so.FindProperty("m_GroupName") != null)
            {
                so.FindProperty("m_GroupName").stringValue = groupName;
                AlignStateValues(so.FindProperty("m_ValueMappings"), states);
                boundField = "m_GroupName";
            }
            else if (so.FindProperty("m_GroupName1") != null)
            {
                so.FindProperty("m_GroupName1").stringValue = groupName;
                AlignStateValues(so.FindProperty("m_ValueMappings1"), states);
                boundField = "m_GroupName1";
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(comp, true);
                throw new GameCommandException(ErrorCode,
                    $"'{stateType.Name}' has no recognized group field (m_GroupName / m_GroupName1).");
            }
            so.ApplyModifiedPropertiesWithoutUndo();

            Persist(comp, isPrefabContents, root, assetPath);

            return new
            {
                action = "add_state_node",
                target = targetLabel,
                nodePath = GetRelativePath(root.transform, node),
                stateType = stateType.Name,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                groupName,
                boundField,
                stateValueSlots = states
            };
        }

        private static void AddStateElement(SerializedProperty statesProp, string stateName)
        {
            int index = statesProp.arraySize;
            statesProp.InsertArrayElementAtIndex(index);
            SerializedProperty element = statesProp.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("m_Name").stringValue = stateName;
            SerializedProperty links = element.FindPropertyRelative("m_Links");
            if (links != null)
            {
                links.ClearArray();
            }
            SerializedProperty onSelectedEvent = element.FindPropertyRelative("m_OnSelectedEvent");
            if (onSelectedEvent != null &&
                onSelectedEvent.propertyType == SerializedPropertyType.ManagedReference)
            {
                onSelectedEvent.managedReferenceValue = null;
            }
        }

        private static void AlignStateValues(SerializedProperty listProp, string[] stateNames)
        {
            if (listProp == null)
            {
                return;
            }

            var wanted = new HashSet<string>(stateNames);
            for (int i = listProp.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty nameProp = listProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_StateName");
                if (nameProp == null || !wanted.Contains(nameProp.stringValue))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                }
            }

            var existing = new HashSet<string>();
            for (int i = 0; i < listProp.arraySize; i++)
            {
                SerializedProperty nameProp = listProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_StateName");
                if (nameProp != null)
                {
                    existing.Add(nameProp.stringValue);
                }
            }

            foreach (string stateName in stateNames)
            {
                if (existing.Contains(stateName))
                {
                    continue;
                }

                int index = listProp.arraySize;
                listProp.InsertArrayElementAtIndex(index);
                SerializedProperty nameProp = listProp.GetArrayElementAtIndex(index).FindPropertyRelative("m_StateName");
                if (nameProp != null)
                {
                    nameProp.stringValue = stateName;
                }
            }
        }

        private static int AlignEffectNodes(StateControllerComponent controller, string groupName, string[] states)
        {
            int count = 0;
            foreach (StateBinding state in controller.GetComponentsInChildren<StateBinding>(true))
            {
                if (state == null || state.GetComponentInParent<StateControllerComponent>(true) != controller)
                {
                    continue;
                }

                var so = new SerializedObject(state);
                bool changed = false;
                SerializedProperty dn = so.FindProperty("m_GroupName");
                if (dn != null)
                {
                    if (dn.stringValue == groupName)
                    {
                        AlignStateValues(so.FindProperty("m_ValueMappings"), states);
                        changed = true;
                    }
                }
                else
                {
                    SerializedProperty dn1 = so.FindProperty("m_GroupName1");
                    if (dn1 != null && dn1.stringValue == groupName)
                    {
                        AlignStateValues(so.FindProperty("m_ValueMappings1"), states);
                        changed = true;
                    }
                    SerializedProperty dn2 = so.FindProperty("m_GroupName2");
                    if (dn2 != null && dn2.stringValue == groupName)
                    {
                        AlignStateValues(so.FindProperty("m_ValueMappings2"), states);
                        changed = true;
                    }
                }

                if (changed)
                {
                    so.ApplyModifiedPropertiesWithoutUndo();
                    count++;
                }
            }
            return count;
        }

        private static Type ResolveStateType(string name)
        {
            foreach (Type type in TypeCache.GetTypesDerivedFrom<StateBinding>())
            {
                if (!type.IsAbstract && (type.Name == name || type.FullName == name))
                {
                    return type;
                }
            }
            return null;
        }

        private static StateControllerComponent ResolveController(GameObject root, StateControllerComponent[] controllers,
            string controllerPath, string groupName, out string error)
        {
            error = null;

            if (!string.IsNullOrEmpty(controllerPath))
            {
                var fullPathMatches = new List<StateControllerComponent>();
                foreach (StateControllerComponent controller in controllers)
                {
                    string fullPath = GetRelativePath(root.transform, controller.transform);
                    if (fullPath == controllerPath)
                    {
                        fullPathMatches.Add(controller);
                    }
                }

                if (fullPathMatches.Count == 1)
                {
                    return fullPathMatches[0];
                }
                if (fullPathMatches.Count > 1)
                {
                    error = $"Controller path '{controllerPath}' is ambiguous. Candidates: [{string.Join(", ", GetControllerPaths(root, fullPathMatches))}]";
                    return null;
                }

                var relativePathMatches = new List<StateControllerComponent>();
                foreach (StateControllerComponent controller in controllers)
                {
                    string relativePath = GetPathBelowRoot(root.transform, controller.transform);
                    if (relativePath == controllerPath)
                    {
                        relativePathMatches.Add(controller);
                    }
                }
                if (relativePathMatches.Count == 1)
                {
                    return relativePathMatches[0];
                }
                if (relativePathMatches.Count > 1)
                {
                    error = $"Controller path '{controllerPath}' is ambiguous. Candidates: [{string.Join(", ", GetControllerPaths(root, relativePathMatches))}]";
                    return null;
                }

                var nameMatches = new List<StateControllerComponent>();
                foreach (StateControllerComponent controller in controllers)
                {
                    if (controller.name == controllerPath)
                    {
                        nameMatches.Add(controller);
                    }
                }
                if (nameMatches.Count == 1)
                {
                    return nameMatches[0];
                }
                if (nameMatches.Count > 1)
                {
                    error = $"Controller name '{controllerPath}' is ambiguous. Use a path. Candidates: [{string.Join(", ", GetControllerPaths(root, nameMatches))}]";
                    return null;
                }

                error = $"Controller '{controllerPath}' not found under target.";
                return null;
            }

            if (string.IsNullOrEmpty(groupName))
            {
                if (controllers.Length == 1)
                {
                    return controllers[0];
                }
                error = $"Multiple StateController components under target. Disambiguate with controllerPath. Candidates: [{string.Join(", ", GetControllerPaths(root, controllers))}]";
                return null;
            }

            var owning = new List<StateControllerComponent>();
            foreach (StateControllerComponent controller in controllers)
            {
                if (controller.GetStateNames(groupName) != null)
                {
                    owning.Add(controller);
                }
            }

            if (owning.Count == 1)
            {
                return owning[0];
            }
            if (owning.Count == 0)
            {
                error = $"No controller under target has data '{groupName}'.";
                return null;
            }

            error = $"Multiple controllers have data '{groupName}'. Disambiguate with controllerPath. Candidates: [{string.Join(", ", GetControllerPaths(root, owning))}]";
            return null;
        }

        private static List<string> GetControllerPaths(GameObject root, IReadOnlyList<StateControllerComponent> controllers)
        {
            var paths = new List<string>();
            foreach (StateControllerComponent controller in controllers)
            {
                paths.Add(GetRelativePath(root.transform, controller.transform));
            }
            return paths;
        }

        private static void Persist(UnityEngine.Object target, bool isPrefabContents, GameObject root, string assetPath)
        {
            if (isPrefabContents)
            {
                PrefabUtility.SaveAsPrefabAsset(root, assetPath);
                return;
            }

            EditorUtility.SetDirty(target);
            if (target is Component component && component.gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }

        private static List<string> ValidateStates(string[] states)
        {
            var result = new List<string>();
            if (states == null)
            {
                return result;
            }

            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (string part in states)
            {
                string trimmed = part?.Trim() ?? string.Empty;
                if (trimmed.Length == 0)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        "states must not contain empty entries.");
                }
                if (!unique.Add(trimmed))
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"states contains duplicate entry '{trimmed}'.");
                }
                result.Add(trimmed);
            }
            return result;
        }

        private static Transform FindNode(Transform root, string path)
        {
            if (string.IsNullOrEmpty(path) || path == root.name)
            {
                return root;
            }

            string prefix = $"{root.name}/";
            string relativePath = path.StartsWith(prefix, StringComparison.Ordinal)
                ? path.Substring(prefix.Length)
                : path;
            return root.Find(relativePath);
        }

        private static bool MatchesNodePath(Transform root, Transform target, string path)
        {
            return GetRelativePath(root, target) == path ||
                   GetPathBelowRoot(root, target) == path ||
                   target.name == path;
        }

        private static string GetPathBelowRoot(Transform root, Transform target)
        {
            if (target == root)
            {
                return string.Empty;
            }

            string path = target.name;
            Transform current = target.parent;
            while (current != null && current != root)
            {
                path = $"{current.name}/{path}";
                current = current.parent;
            }
            return path;
        }

        private static string GetRelativePath(Transform root, Transform target)
        {
            string relativePath = GetPathBelowRoot(root, target);
            return string.IsNullOrEmpty(relativePath) ? root.name : $"{root.name}/{relativePath}";
        }

        private sealed class Arguments
        {
            public string AssetPath;
            public string Path;
            public ObjectId? InstanceId;
            public string NodePath;
            public string ControllerPath;
            public string GroupName;
            public string StateName;
            public int? StateIndex;
            public string[] States;
            public string StateType;
        }

        private sealed class BindingInfo
        {
            [JsonProperty("groupName")]
            public string GroupName;

            [JsonProperty("stateValues")]
            public List<object> StateValues;
        }
    }
#pragma warning restore ET0501
}

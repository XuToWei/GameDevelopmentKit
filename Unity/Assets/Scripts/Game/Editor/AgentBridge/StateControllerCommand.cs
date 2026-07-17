using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StateController;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Unity Agent Bridge command for StateController inspection, state switching, and simple authoring.
    /// </summary>
    public sealed class StateControllerCommand : ICommandHandler
    {
        private const string ErrorCode = "STATECONTROLLER_ERROR";

        private static readonly HashSet<string> s_Actions = new HashSet<string>
        {
            "list", "list_nodes", "set_state", "add_data", "add_state", "add_state_node"
        };

        public string Command => "statecontroller";
        public string Description => "StateController 编辑工具：列出控制器数据/状态、列出 BaseState 节点、切换状态、新增 data/state、给节点添加 BaseState 组件。action=list/list_nodes/set_state/add_data/add_state/add_state_node，默认 list；目标支持 assetPath(prefab)/path(scene)/instanceId/当前选择。写操作会自动保存 Prefab 或标记 Scene dirty。";
        public string Group => "Game";
        public bool CanDisable => true;
        public CommandBatchMode BatchMode => CommandBatchMode.NotAllowed;

        // AgentBridge's Task<object> contract requires an async method builder, while ET0501
        // normally forbids non-UniTask async methods in project code.
#pragma warning disable ET0501, CS1998
        public async Task<object> ExecuteAsync(JObject @params)
        {
            string action = GetString(@params, "action", "list").ToLowerInvariant();
            if (!s_Actions.Contains(action))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"Unknown action: {action}. Supported: {string.Join(", ", s_Actions)}");
            }

            if (!TryResolveRoot(@params, out GameObject root, out bool isPrefabContents, out string assetPath,
                    out string targetLabel, out string error))
            {
                throw new CommandException(ErrorCode, error);
            }

            try
            {
                if (action == "add_state_node")
                {
                    return AddStateNode(@params, root, targetLabel, isPrefabContents, assetPath);
                }
                if (action == "list_nodes")
                {
                    return ListNodes(@params, root, targetLabel);
                }

                StateControllerMono[] controllers = root.GetComponentsInChildren<StateControllerMono>(true);
                if (controllers.Length == 0)
                {
                    throw new CommandException(ErrorCode, $"No StateControllerMono found under '{targetLabel}'.");
                }

                switch (action)
                {
                    case "list":
                        return List(root, targetLabel, controllers);
                    case "set_state":
                        return SetState(@params, root, targetLabel, controllers, isPrefabContents, assetPath);
                    case "add_data":
                        return AddData(@params, root, targetLabel, controllers, isPrefabContents, assetPath);
                    case "add_state":
                        return AddState(@params, root, targetLabel, controllers, isPrefabContents, assetPath);
                    default:
                        throw new CommandException(ErrorCode, $"Unhandled action: {action}");
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
#pragma warning restore ET0501, CS1998

        public JObject ParamsSchema { get; } = JObject.Parse(@"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""action"": {
      ""type"": ""string"",
      ""default"": ""list"",
      ""enum"": [""list"", ""list_nodes"", ""set_state"", ""add_data"", ""add_state"", ""add_state_node""],
      ""description"": ""默认 list。list/list_nodes 只读；set_state/add_data/add_state/add_state_node 会修改目标，Prefab 自动保存，Scene 标记 dirty。""
    },
    ""assetPath"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""Prefab 资源路径。传入后通过 PrefabUtility.LoadPrefabContents 编辑并保存 prefab；优先级高于 instanceId/path/selection。""
    },
    ""path"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""Scene 中 GameObject 的层级路径，包含根节点名；未传 assetPath/instanceId 时使用。""
    },
    ""instanceId"": {
      ""type"": ""integer"",
      ""description"": ""Scene 对象 instanceId；未传 assetPath 时优先于 path。""
    },
    ""nodePath"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""目标根节点下的节点路径；可直接回传 list_nodes 返回的含根节点路径，也兼容不含根节点的相对路径。add_state_node 用它定位节点，list_nodes 用它过滤。""
    },
    ""controllerPath"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""当目标下有多个 StateControllerMono 时用于消歧；可用 list 返回的含根节点路径、不含根节点的相对路径或唯一节点名。""
    },
    ""dataName"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""set_state/add_data/add_state/add_state_node 使用的数据名；list_nodes 可用作过滤条件。""
    },
    ""stateName"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""set_state 要切换到的状态名，或 add_state 要新增的状态名。set_state 可用 stateIndex 替代。""
    },
    ""stateIndex"": {
      ""type"": ""integer"",
      ""minimum"": 0,
      ""description"": ""set_state 可选。按状态索引切换，未传 stateName 时使用。""
    },
    ""states"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""add_data 可选。逗号分隔且不可重复的初始状态名，例如 Normal,Selected,Disabled。""
    },
    ""stateType"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""add_state_node 必填。BaseState 子类名或完整类型名，例如 StateGameObjectForActive。""
    }
  }
}");

        private static object List(GameObject root, string targetLabel, StateControllerMono[] controllers)
        {
            var controllerInfos = new List<object>();
            foreach (StateControllerMono controller in controllers)
            {
                var datas = new List<object>();
                foreach (StateControllerData data in controller.EditorDatas)
                {
                    datas.Add(new
                    {
                        name = data.Name,
                        selectedName = controller.GetSelectedName(data.Name),
                        selectedIndex = controller.GetSelectedIndex(data.Name),
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

        private static object ListNodes(JObject @params, GameObject root, string targetLabel)
        {
            string dataFilter = GetString(@params, "dataName", null);
            string nodeFilter = GetString(@params, "nodePath", null);

            var nodes = new List<object>();
            foreach (BaseState state in root.GetComponentsInChildren<BaseState>(true))
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
                StateControllerMono controller = state.GetComponentInParent<StateControllerMono>(true);

                string mode;
                string booleanLogic = null;
                var bindings = new List<BindingInfo>();
                SerializedProperty dn = so.FindProperty("m_DataName");
                if (dn != null)
                {
                    mode = "selectable";
                    bindings.Add(BuildBinding(dn.stringValue, so.FindProperty("m_StateValues")));
                }
                else
                {
                    mode = "boolean";
                    SerializedProperty logic = so.FindProperty("m_BooleanLogicType");
                    if (logic != null && logic.propertyType == SerializedPropertyType.Enum)
                    {
                        booleanLogic = SafeEnumName(logic);
                    }

                    SerializedProperty dn1 = so.FindProperty("m_DataName1");
                    if (dn1 != null)
                    {
                        bindings.Add(BuildBinding(dn1.stringValue, so.FindProperty("m_StateValues1")));
                    }
                    SerializedProperty dn2 = so.FindProperty("m_DataName2");
                    if (dn2 != null && !string.IsNullOrEmpty(dn2.stringValue))
                    {
                        bindings.Add(BuildBinding(dn2.stringValue, so.FindProperty("m_StateValues2")));
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

        private static BindingInfo BuildBinding(string dataName, SerializedProperty stateValuesProp)
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
                DataName = dataName,
                StateValues = slots
            };
        }

        private static bool BindingsContainData(List<BindingInfo> bindings, string dataName)
        {
            foreach (BindingInfo binding in bindings)
            {
                if (binding.DataName == dataName)
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

        private static object SetState(JObject @params, GameObject root, string targetLabel,
            StateControllerMono[] controllers, bool isPrefabContents, string assetPath)
        {
            string dataName = GetString(@params, "dataName", null);
            if (string.IsNullOrWhiteSpace(dataName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'dataName' for set_state.");
            }

            bool hasStateName = HasParam(@params, "stateName");
            string stateName = GetString(@params, "stateName", null);
            bool hasStateIndex = HasParam(@params, "stateIndex");
            int stateIndex = GetInt(@params, "stateIndex", -1);
            if (!hasStateName && !hasStateIndex)
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Provide 'stateName' or 'stateIndex' for set_state.");
            }
            if (hasStateName && hasStateIndex)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "Provide only one of 'stateName' or 'stateIndex' for set_state.");
            }
            if (hasStateName && string.IsNullOrWhiteSpace(stateName))
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    "'stateName' must not be empty or whitespace for set_state.");
            }

            string controllerPath = GetString(@params, "controllerPath", null);
            StateControllerMono controller = ResolveController(root, controllers, controllerPath, dataName, out string resolveError);
            if (controller == null)
            {
                throw new CommandException(ErrorCode, resolveError);
            }

            string[] states = controller.GetStateNames(dataName);
            if (states == null)
            {
                throw new CommandException(ErrorCode,
                    $"Data '{dataName}' not found on controller '{GetRelativePath(root.transform, controller.transform)}'.");
            }

            if (hasStateName)
            {
                if (Array.IndexOf(states, stateName) < 0)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"State '{stateName}' not in data '{dataName}'. Available: [{string.Join(", ", states)}]");
                }
            }
            else
            {
                if (stateIndex < 0 || stateIndex >= states.Length)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"stateIndex {stateIndex} out of range for data '{dataName}' (count {states.Length}).");
                }
                stateName = states[stateIndex];
            }

            controller.SetSelectedName(dataName, stateName);
            Persist(controller, isPrefabContents, root, assetPath);

            return new
            {
                action = "set_state",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                stateName,
                stateIndex = Array.IndexOf(states, stateName)
            };
        }

        private static object AddData(JObject @params, GameObject root, string targetLabel,
            StateControllerMono[] controllers, bool isPrefabContents, string assetPath)
        {
            string dataName = GetString(@params, "dataName", null);
            if (string.IsNullOrWhiteSpace(dataName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'dataName' for add_data.");
            }

            string controllerPath = GetString(@params, "controllerPath", null);
            StateControllerMono controller = ResolveController(root, controllers, controllerPath, null, out string resolveError);
            if (controller == null)
            {
                throw new CommandException(ErrorCode, resolveError);
            }

            var initialStates = SplitCsv(GetString(@params, "states", null));
            var so = new SerializedObject(controller);
            SerializedProperty datas = so.FindProperty("m_Datas");
            if (datas == null)
            {
                throw new CommandException(ErrorCode, "StateControllerMono.m_Datas not found (package layout changed?).");
            }

            for (int i = 0; i < datas.arraySize; i++)
            {
                if (datas.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == dataName)
                {
                    throw new CommandException(ErrorCode, $"Data '{dataName}' already exists on controller.");
                }
            }

            if (!isPrefabContents)
            {
                Undo.RecordObject(controller, "AgentBridge StateController Add Data");
            }

            int index = datas.arraySize;
            datas.InsertArrayElementAtIndex(index);
            SerializedProperty dataEl = datas.GetArrayElementAtIndex(index);
            dataEl.FindPropertyRelative("m_Name").stringValue = dataName;
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
                action = "add_data",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                states = initialStates
            };
        }

        private static object AddState(JObject @params, GameObject root, string targetLabel,
            StateControllerMono[] controllers, bool isPrefabContents, string assetPath)
        {
            string dataName = GetString(@params, "dataName", null);
            string stateName = GetString(@params, "stateName", null);
            if (string.IsNullOrWhiteSpace(dataName) || string.IsNullOrWhiteSpace(stateName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Provide both 'dataName' and 'stateName' for add_state.");
            }

            string controllerPath = GetString(@params, "controllerPath", null);
            StateControllerMono controller = ResolveController(root, controllers, controllerPath, dataName, out string resolveError);
            if (controller == null)
            {
                throw new CommandException(ErrorCode, resolveError);
            }

            var so = new SerializedObject(controller);
            SerializedProperty datas = so.FindProperty("m_Datas");
            SerializedProperty dataEl = null;
            for (int i = 0; i < datas.arraySize; i++)
            {
                SerializedProperty el = datas.GetArrayElementAtIndex(i);
                if (el.FindPropertyRelative("m_Name").stringValue == dataName)
                {
                    dataEl = el;
                    break;
                }
            }
            if (dataEl == null)
            {
                throw new CommandException(ErrorCode, $"Data '{dataName}' not found on controller.");
            }

            SerializedProperty statesProp = dataEl.FindPropertyRelative("m_States");
            for (int i = 0; i < statesProp.arraySize; i++)
            {
                if (statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == stateName)
                {
                    throw new CommandException(ErrorCode, $"State '{stateName}' already exists in data '{dataName}'.");
                }
            }

            if (!isPrefabContents)
            {
                Undo.RecordObject(controller, "AgentBridge StateController Add State");
            }

            AddStateElement(statesProp, stateName);
            so.ApplyModifiedPropertiesWithoutUndo();

            string[] states = controller.GetStateNames(dataName) ?? Array.Empty<string>();
            int aligned = AlignEffectNodes(controller, dataName, states);
            Persist(controller, isPrefabContents, root, assetPath);

            return new
            {
                action = "add_state",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                stateName,
                states,
                alignedEffectNodes = aligned
            };
        }

        private static object AddStateNode(JObject @params, GameObject root, string targetLabel,
            bool isPrefabContents, string assetPath)
        {
            string stateTypeName = GetString(@params, "stateType", null);
            if (string.IsNullOrWhiteSpace(stateTypeName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'stateType' for add_state_node.");
            }
            string dataName = GetString(@params, "dataName", null);
            if (string.IsNullOrWhiteSpace(dataName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'dataName' for add_state_node.");
            }

            Type stateType = ResolveStateType(stateTypeName);
            if (stateType == null)
            {
                throw new CommandException(ErrorCode,
                    $"State type '{stateTypeName}' not found (must be a non-abstract BaseState subclass).");
            }

            Transform node = root.transform;
            string nodePath = GetString(@params, "nodePath", null);
            if (!string.IsNullOrEmpty(nodePath))
            {
                node = FindNode(root.transform, nodePath);
                if (node == null)
                {
                    throw new CommandException(ErrorCode, $"Node '{nodePath}' not found under '{targetLabel}'.");
                }
            }

            StateControllerMono controller = node.GetComponentInParent<StateControllerMono>(true);
            if (controller == null)
            {
                throw new CommandException(ErrorCode,
                    $"Node '{GetRelativePath(root.transform, node)}' has no StateControllerMono ancestor.");
            }

            string[] states = controller.GetStateNames(dataName);
            if (states == null)
            {
                throw new CommandException(ErrorCode,
                    $"Data '{dataName}' not found on controller '{GetRelativePath(root.transform, controller.transform)}'.");
            }
            if (node.GetComponent(stateType) != null)
            {
                throw new CommandException(ErrorCode, $"Node already has a '{stateType.Name}' component.");
            }

            Component comp = node.gameObject.AddComponent(stateType);
            if (!isPrefabContents)
            {
                Undo.RegisterCreatedObjectUndo(comp, "AgentBridge StateController Add State Node");
            }

            var so = new SerializedObject(comp);
            string boundField;
            if (so.FindProperty("m_DataName") != null)
            {
                so.FindProperty("m_DataName").stringValue = dataName;
                AlignStateValues(so.FindProperty("m_StateValues"), states);
                boundField = "m_DataName";
            }
            else if (so.FindProperty("m_DataName1") != null)
            {
                so.FindProperty("m_DataName1").stringValue = dataName;
                AlignStateValues(so.FindProperty("m_StateValues1"), states);
                boundField = "m_DataName1";
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(comp, true);
                throw new CommandException(ErrorCode,
                    $"'{stateType.Name}' has no recognized data field (m_DataName / m_DataName1).");
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
                dataName,
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

        private static int AlignEffectNodes(StateControllerMono controller, string dataName, string[] states)
        {
            int count = 0;
            foreach (BaseState state in controller.GetComponentsInChildren<BaseState>(true))
            {
                if (state == null || state.GetComponentInParent<StateControllerMono>(true) != controller)
                {
                    continue;
                }

                var so = new SerializedObject(state);
                bool changed = false;
                SerializedProperty dn = so.FindProperty("m_DataName");
                if (dn != null)
                {
                    if (dn.stringValue == dataName)
                    {
                        AlignStateValues(so.FindProperty("m_StateValues"), states);
                        changed = true;
                    }
                }
                else
                {
                    SerializedProperty dn1 = so.FindProperty("m_DataName1");
                    if (dn1 != null && dn1.stringValue == dataName)
                    {
                        AlignStateValues(so.FindProperty("m_StateValues1"), states);
                        changed = true;
                    }
                    SerializedProperty dn2 = so.FindProperty("m_DataName2");
                    if (dn2 != null && dn2.stringValue == dataName)
                    {
                        AlignStateValues(so.FindProperty("m_StateValues2"), states);
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
            foreach (Type type in TypeCache.GetTypesDerivedFrom<BaseState>())
            {
                if (!type.IsAbstract && (type.Name == name || type.FullName == name))
                {
                    return type;
                }
            }
            return null;
        }

        private static bool TryResolveRoot(JObject @params, out GameObject root, out bool isPrefabContents,
            out string assetPath, out string targetLabel, out string error)
        {
            root = null;
            isPrefabContents = false;
            error = null;
            assetPath = GetString(@params, "assetPath", null);
            string path = GetString(@params, "path", null);
            int instanceId = GetInt(@params, "instanceId", 0);

            if (!string.IsNullOrEmpty(assetPath))
            {
                if (!assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                {
                    targetLabel = assetPath;
                    error = $"assetPath must be a .prefab: {assetPath}";
                    return false;
                }
                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null)
                {
                    targetLabel = assetPath;
                    error = $"Prefab not found: {assetPath}";
                    return false;
                }
                root = PrefabUtility.LoadPrefabContents(assetPath);
                isPrefabContents = true;
                targetLabel = assetPath;
                return true;
            }

            if (instanceId != 0)
            {
#if UNITY_6000_0_OR_NEWER
                root = EditorUtility.EntityIdToObject(instanceId) as GameObject;
#else
                root = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
#endif
                targetLabel = path ?? $"instanceId:{instanceId}";
            }
            else if (!string.IsNullOrEmpty(path))
            {
                root = SceneObjectResolver.FindByPath(path);
                targetLabel = path;
            }
            else
            {
                root = Selection.activeGameObject;
                targetLabel = root != null ? root.name : null;
            }

            if (root == null)
            {
                error = "Target not found. Provide 'assetPath' (prefab), 'path'/'instanceId' (scene), or select a GameObject.";
                return false;
            }
            return true;
        }

        private static StateControllerMono ResolveController(GameObject root, StateControllerMono[] controllers,
            string controllerPath, string dataName, out string error)
        {
            error = null;

            if (!string.IsNullOrEmpty(controllerPath))
            {
                var fullPathMatches = new List<StateControllerMono>();
                foreach (StateControllerMono controller in controllers)
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

                var relativePathMatches = new List<StateControllerMono>();
                foreach (StateControllerMono controller in controllers)
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

                var nameMatches = new List<StateControllerMono>();
                foreach (StateControllerMono controller in controllers)
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

            if (string.IsNullOrEmpty(dataName))
            {
                if (controllers.Length == 1)
                {
                    return controllers[0];
                }
                error = $"Multiple StateControllerMono under target. Disambiguate with controllerPath. Candidates: [{string.Join(", ", GetControllerPaths(root, controllers))}]";
                return null;
            }

            var owning = new List<StateControllerMono>();
            foreach (StateControllerMono controller in controllers)
            {
                if (controller.GetStateNames(dataName) != null)
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
                error = $"No controller under target has data '{dataName}'.";
                return null;
            }

            error = $"Multiple controllers have data '{dataName}'. Disambiguate with controllerPath. Candidates: [{string.Join(", ", GetControllerPaths(root, owning))}]";
            return null;
        }

        private static List<string> GetControllerPaths(GameObject root, IReadOnlyList<StateControllerMono> controllers)
        {
            var paths = new List<string>();
            foreach (StateControllerMono controller in controllers)
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

        private static List<string> SplitCsv(string csv)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(csv))
            {
                return result;
            }

            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (string part in csv.Split(','))
            {
                string trimmed = part.Trim();
                if (trimmed.Length == 0)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        "states must not contain empty entries.");
                }
                if (!unique.Add(trimmed))
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
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

        private static string GetString(JObject @params, string name, string defaultValue)
        {
            return @params?[name]?.Value<string>() ?? defaultValue;
        }

        private static int GetInt(JObject @params, string name, int defaultValue)
        {
            return @params?[name]?.Value<int?>() ?? defaultValue;
        }

        private static bool HasParam(JObject @params, string name)
        {
            return @params?[name] != null;
        }

        private sealed class BindingInfo
        {
            [JsonProperty("dataName")]
            public string DataName;

            [JsonProperty("stateValues")]
            public List<object> StateValues;
        }
    }
}

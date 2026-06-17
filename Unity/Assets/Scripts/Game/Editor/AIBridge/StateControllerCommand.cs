using System;
using System.Collections.Generic;
using AIBridge.Editor;
using StateController;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// AIBridge extension command for StateController: inspect controllers/datas/states, switch a data's
    /// selected state, and author the configuration (add data, add state, attach state-effect components).
    /// Auto-discovered by CommandRegistry via reflection.
    ///
    /// 读取/切换用 StateControllerMono 的 public API；新增数据/状态/效果节点用 SerializedObject 按序列化字段名编辑
    /// （m_Datas / m_Name / m_States 以及效果节点的 m_DataName(1)/m_StateValues(1)），不引用包内 internal 类型。
    ///
    /// CLI usage (custom command type, dispatched via batch script 'call' line):
    ///   AIBridgeCLI multi --cmd "statecontroller --action list --assetPath Assets/.../X.prefab"
    /// </summary>
    public class StateControllerCommand : ICommand
    {
        public string Type => "statecontroller";
        public bool RequiresRefresh => true;

        public string SkillDescription => @"### `statecontroller` - StateController Inspect / Switch / Author

Inspect StateController controllers/datas/states, switch a data's selected state, or author configuration:
add a data, add a state, and attach a state-effect component (a `BaseState` subclass) to a node.

**Important**: `statecontroller` is a custom command type - it is NOT a native CLI subcommand. Invoke it through the batch script runner with a `call` line.

```bash
# Inspect controllers/datas/states
$CLI multi --cmd ""statecontroller --action list --assetPath Assets/.../X.prefab""

# Inspect state-effect nodes (BaseState components): binding data + per-state values (optional --dataName/--nodePath filter)
$CLI multi --cmd ""statecontroller --action list_nodes --assetPath Assets/.../X.prefab""
$CLI multi --cmd ""statecontroller --action list_nodes --assetPath Assets/.../X.prefab --dataName Tab""

# Switch a data's state (by name or index)
$CLI batch from_text --text ""call statecontroller --action set_state --assetPath Assets/.../X.prefab --dataName Tab --stateName Selected""

# Add a data (optionally with initial states)
$CLI batch from_text --text ""call statecontroller --action add_data --assetPath Assets/.../X.prefab --dataName Tab --states Normal,Selected""

# Add a state to a data (existing effect-nodes bound to that data get a matching value slot)
$CLI batch from_text --text ""call statecontroller --action add_state --assetPath Assets/.../X.prefab --dataName Tab --stateName Disabled""

# Attach a state-effect component to a node and bind it to a data (value slots aligned to the data's states)
$CLI batch from_text --text ""call statecontroller --action add_state_node --assetPath Assets/.../X.prefab --nodePath Tabs/Icon --stateType StateGameObjectForActive --dataName Tab""
```

**Actions & parameters:**

| action | params | description |
|--------|--------|-------------|
| `list` (default) | target | list controllers, datas, states, current selection |
| `list_nodes` | target, `--dataName`/`--nodePath` (optional filter) | list BaseState effect nodes: path, type, controller, bound data(s) + per-state values |
| `set_state` | target, `--dataName`, `--stateName`\|`--stateIndex`, `--controllerPath` | apply a data's state onto components |
| `add_data` | target, `--dataName`, `--states` (csv, optional), `--controllerPath` | add a new data to a controller |
| `add_state` | target, `--dataName`, `--stateName`, `--controllerPath` | add a state to a data; realigns effect-nodes bound to that data |
| `add_state_node` | node, `--stateType`, `--dataName`, `--nodePath` (prefab) | add a `BaseState` component to the node, bind it to a data, align value slots |

**Target**: `--assetPath` (prefab; node ops add `--nodePath` relative to the prefab root) / `--path` (scene) / `--instanceId` / current selection. `--controllerPath` disambiguates when the subtree has multiple controllers.

`--stateType` is the C# class name of a `BaseState` subclass, e.g. `StateGameObjectForActive`, `StateImageForSprite`, `StateTextForText`, `StateRectTransformForAnchoredPosition`. Newly created value slots use default values - set actual values in the editor afterwards.

**Response (list):**
```json
{""success"":true,""data"":{""action"":""list"",""target"":""...prefab"",""controllers"":[{""path"":""Form/Tabs"",""datas"":[{""name"":""Tab"",""selectedName"":""Selected"",""selectedIndex"":1,""states"":[""Normal"",""Selected""]}]}]}}
```";

        private static readonly HashSet<string> s_Actions = new HashSet<string>
        {
            "list", "list_nodes", "set_state", "add_data", "add_state", "add_state_node"
        };

        public CommandResult Execute(CommandRequest request)
        {
            string action = request.GetParam("action", "list").ToLowerInvariant();
            if (!s_Actions.Contains(action))
            {
                return CommandResult.Failure(request.id,
                    $"Unknown action: {action}. Supported: {string.Join(", ", s_Actions)}");
            }

            if (!TryResolveRoot(request, out GameObject root, out bool isPrefabContents, out string assetPath,
                    out string targetLabel, out string error))
            {
                return CommandResult.Failure(request.id, error);
            }

            try
            {
                switch (action)
                {
                    case "add_state_node":
                        return AddStateNode(request, root, targetLabel, isPrefabContents, assetPath);
                    case "list_nodes":
                        return ListNodes(request, root, targetLabel);
                    default:
                        break;
                }

                StateControllerMono[] controllers = root.GetComponentsInChildren<StateControllerMono>(true);
                if (controllers.Length == 0)
                {
                    return CommandResult.Failure(request.id, $"No StateControllerMono found under '{targetLabel}'.");
                }

                switch (action)
                {
                    case "list":
                        return List(request, root, targetLabel, controllers);
                    case "set_state":
                        return SetState(request, root, targetLabel, controllers, isPrefabContents, assetPath);
                    case "add_data":
                        return AddData(request, root, targetLabel, controllers, isPrefabContents, assetPath);
                    case "add_state":
                        return AddState(request, root, targetLabel, controllers, isPrefabContents, assetPath);
                    default:
                        return CommandResult.Failure(request.id, $"Unhandled action: {action}");
                }
            }
            catch (Exception ex)
            {
                return CommandResult.FromException(request.id, ex);
            }
            finally
            {
                if (isPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }

        // ---------------------------------------------------------------------
        // list / set_state
        // ---------------------------------------------------------------------

        private CommandResult List(CommandRequest request, GameObject root, string targetLabel, StateControllerMono[] controllers)
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

            return CommandResult.Success(request.id, new
            {
                action = "list",
                target = targetLabel,
                controllers = controllerInfos
            });
        }

        /// <summary>
        /// 列出目标下所有 BaseState 效果节点的信息：路径、类型、所属控制器、绑定的 data 及每个状态的值。
        /// 可选 --dataName / --nodePath 过滤。
        /// </summary>
        private CommandResult ListNodes(CommandRequest request, GameObject root, string targetLabel)
        {
            string dataFilter = request.GetParam<string>("dataName", null);
            string nodeFilter = request.GetParam<string>("nodePath", null);

            var nodes = new List<object>();
            foreach (BaseState state in root.GetComponentsInChildren<BaseState>(true))
            {
                if (state == null)
                {
                    continue;
                }
                string nodePath = GetRelativePath(root.transform, state.transform);
                if (!string.IsNullOrEmpty(nodeFilter) && nodePath != nodeFilter && state.name != nodeFilter)
                {
                    continue;
                }

                var so = new SerializedObject(state);
                StateControllerMono controller = state.GetComponentInParent<StateControllerMono>(true);

                string mode;
                string booleanLogic = null;
                var bindings = new List<object>();
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

            return CommandResult.Success(request.id, new
            {
                action = "list_nodes",
                target = targetLabel,
                count = nodes.Count,
                nodes
            });
        }

        private static object BuildBinding(string dataName, SerializedProperty stateValuesProp)
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
            return new { dataName, stateValues = slots };
        }

        private static bool BindingsContainData(List<object> bindings, string dataName)
        {
            foreach (object b in bindings)
            {
                string dn = b.GetType().GetProperty("dataName")?.GetValue(b) as string;
                if (dn == dataName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>把 StateValue.m_Value 的 SerializedProperty 读成可序列化的可读值。</summary>
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
            int idx = p.enumValueIndex;
            return idx >= 0 && idx < names.Length ? names[idx] : idx.ToString();
        }

        private CommandResult SetState(CommandRequest request, GameObject root, string targetLabel,
            StateControllerMono[] controllers, bool isPrefabContents, string assetPath)
        {
            string dataName = request.GetParam<string>("dataName", null);
            if (string.IsNullOrEmpty(dataName))
            {
                return CommandResult.Failure(request.id, "Missing 'dataName' for set_state.");
            }
            bool hasStateName = request.HasParam("stateName");
            string stateName = request.GetParam<string>("stateName", null);
            bool hasStateIndex = request.HasParam("stateIndex");
            int stateIndex = request.GetParam("stateIndex", -1);
            if (!hasStateName && !hasStateIndex)
            {
                return CommandResult.Failure(request.id, "Provide 'stateName' or 'stateIndex' for set_state.");
            }

            string controllerPath = request.GetParam<string>("controllerPath", null);
            StateControllerMono controller = ResolveController(root, controllers, controllerPath, dataName, out string resolveError);
            if (controller == null)
            {
                return CommandResult.Failure(request.id, resolveError);
            }

            string[] states = controller.GetStateNames(dataName);
            if (states == null)
            {
                return CommandResult.Failure(request.id,
                    $"Data '{dataName}' not found on controller '{GetRelativePath(root.transform, controller.transform)}'.");
            }

            if (hasStateName)
            {
                if (Array.IndexOf(states, stateName) < 0)
                {
                    return CommandResult.Failure(request.id,
                        $"State '{stateName}' not in data '{dataName}'. Available: [{string.Join(", ", states)}]");
                }
            }
            else
            {
                if (stateIndex < 0 || stateIndex >= states.Length)
                {
                    return CommandResult.Failure(request.id,
                        $"stateIndex {stateIndex} out of range for data '{dataName}' (count {states.Length}).");
                }
                stateName = states[stateIndex];
            }

            controller.SetSelectedName(dataName, stateName);
            Persist(controller, isPrefabContents, root, assetPath);

            return CommandResult.Success(request.id, new
            {
                action = "set_state",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                stateName,
                stateIndex = Array.IndexOf(states, stateName)
            });
        }

        // ---------------------------------------------------------------------
        // add_data / add_state
        // ---------------------------------------------------------------------

        private CommandResult AddData(CommandRequest request, GameObject root, string targetLabel,
            StateControllerMono[] controllers, bool isPrefabContents, string assetPath)
        {
            string dataName = request.GetParam<string>("dataName", null);
            if (string.IsNullOrEmpty(dataName))
            {
                return CommandResult.Failure(request.id, "Missing 'dataName' for add_data.");
            }

            string controllerPath = request.GetParam<string>("controllerPath", null);
            StateControllerMono controller = ResolveController(root, controllers, controllerPath, null, out string resolveError);
            if (controller == null)
            {
                return CommandResult.Failure(request.id, resolveError);
            }

            var initialStates = SplitCsv(request.GetParam<string>("states", null));

            var so = new SerializedObject(controller);
            SerializedProperty datas = so.FindProperty("m_Datas");
            if (datas == null)
            {
                return CommandResult.Failure(request.id, "StateControllerMono.m_Datas not found (package layout changed?).");
            }
            for (int i = 0; i < datas.arraySize; i++)
            {
                if (datas.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == dataName)
                {
                    return CommandResult.Failure(request.id, $"Data '{dataName}' already exists on controller.");
                }
            }

            int idx = datas.arraySize;
            datas.InsertArrayElementAtIndex(idx);
            SerializedProperty dataEl = datas.GetArrayElementAtIndex(idx);
            dataEl.FindPropertyRelative("m_Name").stringValue = dataName;
            SerializedProperty statesProp = dataEl.FindPropertyRelative("m_States");
            statesProp.ClearArray();
            foreach (string s in initialStates)
            {
                AddStateElement(statesProp, s);
            }
            so.ApplyModifiedPropertiesWithoutUndo();

            Persist(controller, isPrefabContents, root, assetPath);

            return CommandResult.Success(request.id, new
            {
                action = "add_data",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                states = initialStates
            });
        }

        private CommandResult AddState(CommandRequest request, GameObject root, string targetLabel,
            StateControllerMono[] controllers, bool isPrefabContents, string assetPath)
        {
            string dataName = request.GetParam<string>("dataName", null);
            string stateName = request.GetParam<string>("stateName", null);
            if (string.IsNullOrEmpty(dataName) || string.IsNullOrEmpty(stateName))
            {
                return CommandResult.Failure(request.id, "Provide both 'dataName' and 'stateName' for add_state.");
            }

            string controllerPath = request.GetParam<string>("controllerPath", null);
            StateControllerMono controller = ResolveController(root, controllers, controllerPath, dataName, out string resolveError);
            if (controller == null)
            {
                return CommandResult.Failure(request.id, resolveError);
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
                return CommandResult.Failure(request.id, $"Data '{dataName}' not found on controller.");
            }

            SerializedProperty statesProp = dataEl.FindPropertyRelative("m_States");
            for (int i = 0; i < statesProp.arraySize; i++)
            {
                if (statesProp.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue == stateName)
                {
                    return CommandResult.Failure(request.id, $"State '{stateName}' already exists in data '{dataName}'.");
                }
            }
            AddStateElement(statesProp, stateName);
            so.ApplyModifiedPropertiesWithoutUndo();

            // 同步：把绑定该 data 的效果节点的值槽对齐到新的状态集合
            string[] states = controller.GetStateNames(dataName) ?? Array.Empty<string>();
            int aligned = AlignEffectNodes(controller, dataName, states);

            Persist(controller, isPrefabContents, root, assetPath);

            return CommandResult.Success(request.id, new
            {
                action = "add_state",
                target = targetLabel,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                stateName,
                states,
                alignedEffectNodes = aligned
            });
        }

        // ---------------------------------------------------------------------
        // add_state_node：给节点挂一个 BaseState 效果组件并绑定到 data
        // ---------------------------------------------------------------------

        private CommandResult AddStateNode(CommandRequest request, GameObject root, string targetLabel,
            bool isPrefabContents, string assetPath)
        {
            string stateTypeName = request.GetParam<string>("stateType", null);
            if (string.IsNullOrEmpty(stateTypeName))
            {
                return CommandResult.Failure(request.id, "Missing 'stateType' for add_state_node.");
            }
            string dataName = request.GetParam<string>("dataName", null);
            if (string.IsNullOrEmpty(dataName))
            {
                return CommandResult.Failure(request.id, "Missing 'dataName' for add_state_node.");
            }

            Type stateType = ResolveStateType(stateTypeName);
            if (stateType == null)
            {
                return CommandResult.Failure(request.id,
                    $"State type '{stateTypeName}' not found (must be a non-abstract BaseState subclass).");
            }

            // 定位节点：prefab 用 --nodePath（相对根），否则 root 自身即节点
            Transform node = root.transform;
            string nodePath = request.GetParam<string>("nodePath", null);
            if (!string.IsNullOrEmpty(nodePath))
            {
                node = root.transform.Find(nodePath);
                if (node == null)
                {
                    return CommandResult.Failure(request.id, $"Node '{nodePath}' not found under '{targetLabel}'.");
                }
            }

            StateControllerMono controller = node.GetComponentInParent<StateControllerMono>(true);
            if (controller == null)
            {
                return CommandResult.Failure(request.id,
                    $"Node '{GetRelativePath(root.transform, node)}' has no StateControllerMono ancestor.");
            }
            string[] states = controller.GetStateNames(dataName);
            if (states == null)
            {
                return CommandResult.Failure(request.id,
                    $"Data '{dataName}' not found on controller '{GetRelativePath(root.transform, controller.transform)}'.");
            }
            if (node.GetComponent(stateType) != null)
            {
                return CommandResult.Failure(request.id, $"Node already has a '{stateType.Name}' component.");
            }

            var comp = node.gameObject.AddComponent(stateType);
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
                return CommandResult.Failure(request.id,
                    $"'{stateType.Name}' has no recognized data field (m_DataName / m_DataName1).");
            }
            so.ApplyModifiedPropertiesWithoutUndo();

            Persist(comp, isPrefabContents, root, assetPath);

            return CommandResult.Success(request.id, new
            {
                action = "add_state_node",
                target = targetLabel,
                nodePath = GetRelativePath(root.transform, node),
                stateType = stateType.Name,
                controllerPath = GetRelativePath(root.transform, controller.transform),
                dataName,
                boundField,
                stateValueSlots = states
            });
        }

        // ---------------------------------------------------------------------
        // SerializedProperty 辅助
        // ---------------------------------------------------------------------

        /// <summary>给 data 的 m_States 追加一个状态元素（清空其 m_Links）。</summary>
        private static void AddStateElement(SerializedProperty statesProp, string stateName)
        {
            int idx = statesProp.arraySize;
            statesProp.InsertArrayElementAtIndex(idx);
            SerializedProperty el = statesProp.GetArrayElementAtIndex(idx);
            el.FindPropertyRelative("m_Name").stringValue = stateName;
            SerializedProperty links = el.FindPropertyRelative("m_Links");
            if (links != null)
            {
                links.ClearArray();
            }
        }

        /// <summary>把一个值槽列表对齐到给定状态集合：删除多余、补齐缺失（保留已有值，新槽用默认值）。</summary>
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
            foreach (string sn in stateNames)
            {
                if (existing.Contains(sn))
                {
                    continue;
                }
                int idx = listProp.arraySize;
                listProp.InsertArrayElementAtIndex(idx);
                SerializedProperty nameProp = listProp.GetArrayElementAtIndex(idx).FindPropertyRelative("m_StateName");
                if (nameProp != null)
                {
                    nameProp.stringValue = sn;
                }
            }
        }

        /// <summary>把控制器下绑定了 dataName 的所有 BaseState 效果节点的值槽对齐到 states，返回对齐的节点数。</summary>
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
            foreach (Type t in TypeCache.GetTypesDerivedFrom<BaseState>())
            {
                if (!t.IsAbstract && t.Name == name)
                {
                    return t;
                }
            }
            return null;
        }

        // ---------------------------------------------------------------------
        // 目标 / 控制器解析、持久化、工具
        // ---------------------------------------------------------------------

        private static bool TryResolveRoot(CommandRequest request, out GameObject root, out bool isPrefabContents,
            out string assetPath, out string targetLabel, out string error)
        {
            root = null;
            isPrefabContents = false;
            error = null;
            assetPath = request.GetParam<string>("assetPath", null);
            string path = request.GetParam<string>("path", null);
            int instanceId = request.GetParam("instanceId", 0);

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
#if UNITY_6000_3_OR_NEWER
                root = EditorUtility.EntityIdToObject(instanceId) as GameObject;
#else
                root = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
#endif
                targetLabel = path ?? $"instanceId:{instanceId}";
            }
            else if (!string.IsNullOrEmpty(path))
            {
                root = GameObject.Find(path);
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

        /// <summary>
        /// 解析目标控制器：给了 controllerPath 按路径/名字匹配；否则给了 dataName 取唯一拥有该 data 的控制器；
        /// 都没有时取唯一控制器。
        /// </summary>
        private StateControllerMono ResolveController(GameObject root, StateControllerMono[] controllers,
            string controllerPath, string dataName, out string error)
        {
            error = null;

            if (!string.IsNullOrEmpty(controllerPath))
            {
                foreach (StateControllerMono c in controllers)
                {
                    if (GetRelativePath(root.transform, c.transform) == controllerPath || c.name == controllerPath)
                    {
                        return c;
                    }
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
                error = $"Multiple StateControllerMono under target. Disambiguate with --controllerPath. Candidates: [{string.Join(", ", GetControllerPaths(root, controllers))}]";
                return null;
            }

            var owning = new List<StateControllerMono>();
            foreach (StateControllerMono c in controllers)
            {
                if (c.GetStateNames(dataName) != null)
                {
                    owning.Add(c);
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
            error = $"Multiple controllers have data '{dataName}'. Disambiguate with --controllerPath. Candidates: [{string.Join(", ", GetControllerPaths(root, owning))}]";
            return null;
        }

        private static List<string> GetControllerPaths(GameObject root, IReadOnlyList<StateControllerMono> controllers)
        {
            var paths = new List<string>();
            foreach (StateControllerMono c in controllers)
            {
                paths.Add(GetRelativePath(root.transform, c.transform));
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
            Component component = target as Component;
            if (component != null && component.gameObject.scene.IsValid())
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
            foreach (string part in csv.Split(','))
            {
                string trimmed = part.Trim();
                if (trimmed.Length > 0)
                {
                    result.Add(trimmed);
                }
            }
            return result;
        }

        private static string GetRelativePath(Transform root, Transform target)
        {
            if (target == root)
            {
                return root.name;
            }
            string path = target.name;
            Transform t = target.parent;
            while (t != null && t != root)
            {
                path = $"{t.name}/{path}";
                t = t.parent;
            }
            return $"{root.name}/{path}";
        }
    }
}

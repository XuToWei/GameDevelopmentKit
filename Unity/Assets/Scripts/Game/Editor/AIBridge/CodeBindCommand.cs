using System;
using System.Collections.Generic;
using System.Reflection;
using AIBridge.Editor;
using CodeBind;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// AIBridge extension command for CodeBind: generate bind code (.Bind.cs), reference serialization,
    /// and rename a node into a wildcard bind name (`bindName<sep>*`) that binds all of its components.
    /// Auto-discovered by CommandRegistry via reflection.
    ///
    /// 绑定生成/序列化复用 CodeBind 包内 MonoCodeBinder / CSCodeBinder（internal，通过反射调用）。
    ///
    /// CLI usage (custom command type, dispatched via batch script 'call' line):
    ///   AIBridgeCLI multi --cmd "codebind --action all --assetPath Assets/Res/UI/UIForm/Hot/LoginForm.prefab"
    /// </summary>
    public class CodeBindCommand : ICommand
    {
        public string Type => "codebind";
        public bool RequiresRefresh => true;

        public string SkillDescription => @"### `codebind` - CodeBind Generate / Serialize / Rename

Generate CodeBind bind code (`*.Bind.cs`), assign reference serialization, or rename a node into a wildcard bind name (`bindName<sep>*`) that auto-binds all of the node's components.

**Important**: `codebind` is a custom command type - it is NOT a native CLI subcommand. Invoke it through the batch script runner with a `call` line.

```bash
# Generate bind code + serialization (default action = all)
$CLI multi --cmd ""codebind --action all --assetPath Assets/Res/UI/UIForm/Hot/LoginForm.prefab""

# Only (re)generate bind code / only serialize
$CLI batch from_text --text ""call codebind --action generate_code --assetPath Assets/.../X.prefab""
$CLI batch from_text --text ""call codebind --action set_serialization --path Canvas/LoginForm""

# Rename a node to '<bindName>_*' so it binds all components on that node
$CLI batch from_text --text ""call codebind --action rename_node --assetPath Assets/.../X.prefab --nodePath Panel/LoginButton --bindName Login""
$CLI batch from_text --text ""call codebind --action rename_node --path Canvas/Item --bindName Item""

# Array element: pass --arrayIndex to produce '<bindName>_* (index)'. Name multiple sibling nodes with the
# same bindName to form an array (indices are auto-renumbered on generate).
$CLI batch from_text --text ""call codebind --action rename_node --path Canvas/Slot0 --bindName Slot --arrayIndex 0""
$CLI batch from_text --text ""call codebind --action rename_node --path Canvas/Slot1 --bindName Slot --arrayIndex 1""
```

Works for both bind modes: `generate_code`/`set_serialization` process `[MonoCodeBind]` MonoBehaviours (mono mode) and `CSCodeBindMono` components (cs mode) anywhere under the target; `rename_node` is mode-agnostic (the `<bindName><sep>...` convention is shared).

**Recommended flow when bind nodes were added/removed** (new fields need a recompile before assignment). Run as one script:
```
call codebind --action rename_node --assetPath Assets/.../X.prefab --nodePath Panel/LoginButton --bindName Login
call codebind --action generate_code --assetPath Assets/.../X.prefab
wait_compile 120000
call codebind --action set_serialization --assetPath Assets/.../X.prefab
```

**Actions & parameters:**

| action | params | description |
|--------|--------|-------------|
| `all` (default) | target | generate_code then set_serialization |
| `generate_code` | target | write `<Class>.Bind.cs` for `[MonoCodeBind]` + `CSCodeBindMono` (root & children) |
| `set_serialization` | target | assign component references (needs bind code compiled) |
| `rename_node` | node, `--bindName`, `--arrayIndex`, `--separator` | rename the node to `bindName<sep>*` (binds all components); with `--arrayIndex` -> `bindName<sep>* (index)` array element |

**Target** (root for generate/serialize, single node for rename): `--assetPath` (prefab; rename adds `--nodePath` relative to the prefab root) / `--path` (scene) / `--instanceId` / current selection. `--separator` defaults to the project setting (`EditorPrefs CodeBind.SeparatorChar`, usually `_`).

**Response:**
```json
{""success"":true,""data"":{""action"":""all"",""target"":""Assets/.../LoginForm.prefab"",""codeGenerated"":[""MonoUIFormLogin""],""serialized"":[""MonoUIFormLogin""],""serializationDeferred"":false}}
```
When `serializationDeferred` is true the bind code changed and a `compile unity` is required before `set_serialization`.";

        // CodeBind 包内的 internal binder 类型，通过反射调用其 public 方法
        private static Type s_MonoCodeBinderType;
        private static Type s_CSCodeBinderType;

        public CommandResult Execute(CommandRequest request)
        {
            try
            {
                string action = request.GetParam("action", "all").ToLowerInvariant();
                switch (action)
                {
                    case "all":
                    case "generate_code":
                    case "set_serialization":
                        return RunBind(request, action);
                    case "rename_node":
                        return RenameNode(request);
                    default:
                        return CommandResult.Failure(request.id,
                            $"Unknown action: {action}. Supported: all, generate_code, set_serialization, rename_node");
                }
            }
            catch (Exception ex)
            {
                return CommandResult.FromException(request.id, ex);
            }
        }

        // ---------------------------------------------------------------------
        // 绑定代码生成 / 引用序列化
        // ---------------------------------------------------------------------

        private CommandResult RunBind(CommandRequest request, string action)
        {
            if (!TryResolveBinderTypes(out string typeError))
            {
                return CommandResult.Failure(request.id, typeError);
            }

            if (!TryResolveRoot(request, out GameObject root, out bool isPrefabContents, out string assetPath,
                    out string targetLabel, out string error))
            {
                return CommandResult.Failure(request.id, error);
            }

            bool genCode = action == "all" || action == "generate_code";
            bool serialize = action == "all" || action == "set_serialization";

            var codeGenerated = new List<string>();
            var serialized = new List<string>();
            bool serializationDeferred = false;
            string deferReason = null;

            try
            {
                var targets = CollectTargets(root);
                if (targets.Count == 0)
                {
                    return CommandResult.Failure(request.id,
                        $"No [MonoCodeBind] component or CSCodeBindMono found under '{targetLabel}'.");
                }

                if (genCode)
                {
                    foreach (BindTarget t in targets)
                    {
                        InvokeBinder(t, "TryGenerateBindCode");
                        codeGenerated.Add(t.Label);
                    }
                }

                if (serialize)
                {
                    foreach (BindTarget t in targets)
                    {
                        try
                        {
                            InvokeBinder(t, "TrySetSerialization");
                            serialized.Add(t.Label);
                        }
                        catch (Exception ex) when (action == "all")
                        {
                            // 'all' 容错：生成了新字段但尚未编译时序列化会失败，降级为需要先编译
                            serializationDeferred = true;
                            deferReason = ex.Message;
                            break;
                        }
                    }
                }

                if (isPrefabContents)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, assetPath);
                }
            }
            finally
            {
                if (isPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            return CommandResult.Success(request.id, new
            {
                action,
                target = targetLabel,
                codeGenerated,
                serialized,
                serializationDeferred,
                message = serializationDeferred
                    ? $"绑定代码已变更，需要先编译再序列化。请运行 `compile unity` 后执行 codebind action=set_serialization。({deferReason})"
                    : null
            });
        }

        // ---------------------------------------------------------------------
        // 节点改名：统一用通配符 * 绑定该节点的全部组件
        // ---------------------------------------------------------------------

        private CommandResult RenameNode(CommandRequest request)
        {
            char separator = GetSeparator(request);

            string bindName = request.GetParam<string>("bindName", null);
            if (string.IsNullOrEmpty(bindName))
            {
                return CommandResult.Failure(request.id, "Missing 'bindName' for rename_node.");
            }
            if (bindName.IndexOf(separator) >= 0)
            {
                return CommandResult.Failure(request.id, $"bindName '{bindName}' must not contain the separator '{separator}'.");
            }

            // 统一用通配符 * 绑定该节点全部组件；带 arrayIndex 时作为数组元素 "bindName_* (index)"
            string newName = $"{bindName}{separator}*";
            bool isArray = request.HasParam("arrayIndex");
            if (isArray)
            {
                int arrayIndex = request.GetParam("arrayIndex", 0);
                if (arrayIndex < 0)
                {
                    return CommandResult.Failure(request.id, $"arrayIndex must be >= 0, got {arrayIndex}.");
                }
                newName = $"{newName} ({arrayIndex})";
            }

            if (!TryResolveNode(request, out NodeContext ctx, out string error))
            {
                if (ctx.IsPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(ctx.PrefabRoot);
                }
                return CommandResult.Failure(request.id, error);
            }

            try
            {
                string oldName = ctx.Node.name;
                ctx.Node.name = newName;

                if (ctx.IsPrefabContents)
                {
                    PrefabUtility.SaveAsPrefabAsset(ctx.PrefabRoot, ctx.AssetPath);
                }
                else
                {
                    EditorUtility.SetDirty(ctx.Node.gameObject);
                    if (ctx.Node.gameObject.scene.IsValid())
                    {
                        EditorSceneManager.MarkSceneDirty(ctx.Node.gameObject.scene);
                    }
                }

                return CommandResult.Success(request.id, new
                {
                    action = "rename_node",
                    target = ctx.TargetLabel,
                    oldName,
                    newName,
                    isArray,
                    separator = separator.ToString()
                });
            }
            finally
            {
                if (ctx.IsPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(ctx.PrefabRoot);
                }
            }
        }

        // ---------------------------------------------------------------------
        // 目标解析
        // ---------------------------------------------------------------------

        /// <summary>
        /// 解析"绑定根"（generate_code / set_serialization 用），对整棵子树操作。
        /// </summary>
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
        /// 解析单个节点（rename_node 用）。prefab 时用 --nodePath 在预制体内定位。
        /// 即便失败也会回填 ctx 的 prefab 加载信息，便于调用方在 finally 中卸载。
        /// </summary>
        private static bool TryResolveNode(CommandRequest request, out NodeContext ctx, out string error)
        {
            ctx = default;
            error = null;
            string assetPath = request.GetParam<string>("assetPath", null);
            string path = request.GetParam<string>("path", null);
            int instanceId = request.GetParam("instanceId", 0);

            if (!string.IsNullOrEmpty(assetPath))
            {
                if (!assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                {
                    error = $"assetPath must be a .prefab: {assetPath}";
                    return false;
                }
                if (AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) == null)
                {
                    error = $"Prefab not found: {assetPath}";
                    return false;
                }
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                ctx.IsPrefabContents = true;
                ctx.PrefabRoot = prefabRoot;
                ctx.AssetPath = assetPath;

                string nodePath = request.GetParam<string>("nodePath", null);
                Transform node;
                if (string.IsNullOrEmpty(nodePath))
                {
                    node = prefabRoot.transform;
                }
                else
                {
                    node = prefabRoot.transform.Find(nodePath);
                    if (node == null)
                    {
                        error = $"Node '{nodePath}' not found in prefab '{assetPath}'.";
                        return false;
                    }
                }
                ctx.Node = node;
                ctx.TargetLabel = string.IsNullOrEmpty(nodePath) ? assetPath : $"{assetPath}:{nodePath}";
                return true;
            }

            GameObject go;
            if (instanceId != 0)
            {
#if UNITY_6000_3_OR_NEWER
                go = EditorUtility.EntityIdToObject(instanceId) as GameObject;
#else
                go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
#endif
                ctx.TargetLabel = path ?? $"instanceId:{instanceId}";
            }
            else if (!string.IsNullOrEmpty(path))
            {
                go = GameObject.Find(path);
                ctx.TargetLabel = path;
            }
            else
            {
                go = Selection.activeGameObject;
                ctx.TargetLabel = go != null ? go.name : null;
            }

            if (go == null)
            {
                error = "Node not found. Provide 'assetPath'+'nodePath' (prefab), 'path'/'instanceId' (scene), or select a GameObject.";
                return false;
            }
            ctx.Node = go.transform;
            return true;
        }

        private static char GetSeparator(CommandRequest request)
        {
            string custom = request.GetParam<string>("separator", null);
            if (!string.IsNullOrEmpty(custom))
            {
                return custom[0];
            }
            string saved = EditorPrefs.GetString("CodeBind.SeparatorChar", "_");
            return string.IsNullOrEmpty(saved) ? '_' : saved[0];
        }

        // ---------------------------------------------------------------------
        // 绑定器收集与反射调用
        // ---------------------------------------------------------------------

        /// <summary>
        /// 收集 root 及其子节点上所有需要绑定的目标（[MonoCodeBind] 组件与 CSCodeBindMono）。
        /// </summary>
        private static List<BindTarget> CollectTargets(GameObject root)
        {
            var targets = new List<BindTarget>();

            MonoBehaviour[] monos = root.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour mono in monos)
            {
                if (mono == null)
                {
                    continue;
                }
                object[] attrs = mono.GetType().GetCustomAttributes(typeof(MonoCodeBindAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }
                var attr = (MonoCodeBindAttribute)attrs[0];
                MonoScript script = MonoScript.FromMonoBehaviour(mono);
                targets.Add(new BindTarget
                {
                    BinderType = s_MonoCodeBinderType,
                    BinderArgs = new object[] { script, mono.transform, attr.SeparatorChar },
                    Label = mono.GetType().Name
                });
            }

            CSCodeBindMono[] csBinds = root.GetComponentsInChildren<CSCodeBindMono>(true);
            foreach (CSCodeBindMono bindMono in csBinds)
            {
                targets.Add(new BindTarget
                {
                    BinderType = s_CSCodeBinderType,
                    BinderArgs = new object[] { bindMono.BindScript, bindMono.transform, bindMono.SeparatorChar },
                    Label = $"{bindMono.name}(CSCodeBindMono)"
                });
            }

            return targets;
        }

        private static void InvokeBinder(BindTarget target, string methodName)
        {
            object binder = Activator.CreateInstance(target.BinderType, target.BinderArgs);
            MethodInfo method = target.BinderType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                throw new MissingMethodException($"{target.BinderType.FullName}.{methodName} not found (CodeBind API changed?).");
            }
            try
            {
                method.Invoke(binder, null);
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }
        }

        private static bool TryResolveBinderTypes(out string error)
        {
            if (s_MonoCodeBinderType == null)
            {
                s_MonoCodeBinderType = FindType("CodeBind.Editor.MonoCodeBinder");
            }
            if (s_CSCodeBinderType == null)
            {
                s_CSCodeBinderType = FindType("CodeBind.Editor.CSCodeBinder");
            }
            if (s_MonoCodeBinderType == null || s_CSCodeBinderType == null)
            {
                error = "CodeBind.Editor binder types not found. Is the me.xw.codebind package present?";
                return false;
            }
            error = null;
            return true;
        }

        private static Type FindType(string fullName)
        {
            Type t = System.Type.GetType($"{fullName}, CodeBind.Editor");
            if (t != null)
            {
                return t;
            }
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(fullName);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        private struct BindTarget
        {
            public Type BinderType;
            public object[] BinderArgs;
            public string Label;
        }

        private struct NodeContext
        {
            public Transform Node;
            public bool IsPrefabContents;
            public GameObject PrefabRoot;
            public string AssetPath;
            public string TargetLabel;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AgentBridge;
using CodeBind;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Unity Agent Bridge command for CodeBind: generate bind code, assign serialized references,
    /// and rename bind nodes.
    /// </summary>
    public sealed class CodeBindCommand : ICommandHandler
    {
        private const string ErrorCode = "CODEBIND_ERROR";

        private static Type s_MonoCodeBinderType;
        private static Type s_CSCodeBinderType;

        public string Command => "codebind";
        public string Description => "CodeBind 绑定工具：生成 *.Bind.cs、刷新序列化引用，或把节点改名为 bindName_*。action=all/generate_code/set_serialization/rename_node，默认 all；目标支持 assetPath(prefab)/path(scene)/instanceId/当前选择。Prefab 会自动保存，Scene 目标会标记 dirty；all 生成新代码后可能需要先编译再执行 set_serialization。";
        public string Group => "Game";
        public bool CanDisable => true;
        public CommandBatchMode BatchMode => CommandBatchMode.NotAllowed;

        // AgentBridge's Task<object> contract requires an async method builder, while ET0501
        // normally forbids non-UniTask async methods in project code.
        public Task<object> ExecuteAsync(JObject @params)
        {
            string action = GetString(@params, "action", "all").ToLowerInvariant();
            switch (action)
            {
                case "all":
                case "generate_code":
                case "set_serialization":
                    return Task.FromResult<object>(RunBind(@params, action));
                case "rename_node":
                    return Task.FromResult<object>(RenameNode(@params));
                default:
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"Unknown action: {action}. Supported: all, generate_code, set_serialization, rename_node");
            }
        }

        public JObject ParamsSchema { get; } = JObject.Parse(@"{
  ""type"": ""object"",
  ""additionalProperties"": false,
  ""properties"": {
    ""action"": {
      ""type"": ""string"",
      ""default"": ""all"",
      ""enum"": [""all"", ""generate_code"", ""set_serialization"", ""rename_node""],
      ""description"": ""默认 all。all = 先 generate_code 再尝试 set_serialization；如果新字段尚未编译，返回 serializationDeferred=true，需要编译后再执行 set_serialization。""
    },
    ""assetPath"": {
      ""type"": ""string"",
      ""description"": ""Prefab 资源路径。传入后通过 PrefabUtility.LoadPrefabContents 编辑并保存 prefab；优先级高于 instanceId/path/selection。""
    },
    ""path"": {
      ""type"": ""string"",
      ""description"": ""Scene 中 GameObject 的层级路径，包含根节点名，例如 Canvas/LoginForm；未传 assetPath/instanceId 时使用。""
    },
    ""instanceId"": {
      ""type"": ""integer"",
      ""description"": ""Scene 对象 instanceId；未传 assetPath 时优先于 path。""
    },
    ""nodePath"": {
      ""type"": ""string"",
      ""description"": ""Prefab/root 下的相对子节点路径，仅 rename_node 使用；省略时操作目标根节点。""
    },
    ""bindName"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""description"": ""rename_node 必填。节点会被改名为 bindName<separator>*；不要包含 separator。""
    },
    ""arrayIndex"": {
      ""type"": ""integer"",
      ""minimum"": 0,
      ""description"": ""rename_node 可选。传入后节点名为 bindName<separator>* (index)，用于数组元素绑定。""
    },
    ""separator"": {
      ""type"": ""string"",
      ""minLength"": 1,
      ""maxLength"": 1,
      ""description"": ""绑定分隔符，默认读取 EditorPrefs CodeBind.SeparatorChar，通常为 _。""
    }
  }
}");

        private static object RunBind(JObject @params, string action)
        {
            if (!TryResolveBinderTypes(out string typeError))
            {
                throw new CommandException(ErrorCode, typeError);
            }

            if (!TryResolveRoot(@params, out GameObject root, out bool isPrefabContents, out string assetPath,
                    out string targetLabel, out string error))
            {
                throw new CommandException(ErrorCode, error);
            }

            bool genCode = action == "all" || action == "generate_code";
            bool serialize = action == "all" || action == "set_serialization";

            var codeGenerated = new List<string>();
            var serialized = new List<string>();
            bool serializationDeferred = false;
            string deferReason = null;

            try
            {
                List<BindTarget> targets = CollectTargets(root);
                if (targets.Count == 0)
                {
                    throw new CommandException(ErrorCode,
                        $"No [MonoCodeBind] component or CSCodeBindMono found under '{targetLabel}'.");
                }

                if (!isPrefabContents)
                {
                    Undo.RegisterFullObjectHierarchyUndo(root, "AgentBridge CodeBind");
                }

                if (genCode)
                {
                    foreach (BindTarget target in targets)
                    {
                        InvokeBinder(target, "TryGenerateBindCode");
                        codeGenerated.Add(target.Label);
                    }
                }

                if (serialize)
                {
                    foreach (BindTarget target in targets)
                    {
                        try
                        {
                            InvokeBinder(target, "TrySetSerialization");
                            serialized.Add(target.Label);
                        }
                        catch (Exception ex) when (action == "all")
                        {
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
                else if (genCode || serialize)
                {
                    EditorUtility.SetDirty(root);
                    if (root.scene.IsValid())
                    {
                        EditorSceneManager.MarkSceneDirty(root.scene);
                    }
                }
            }
            finally
            {
                if (isPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            return new
            {
                action,
                target = targetLabel,
                codeGenerated,
                serialized,
                serializationDeferred,
                message = serializationDeferred
                    ? $"Bind code changed. Compile Unity, then run codebind action=set_serialization. ({deferReason})"
                    : null
            };
        }

        private static object RenameNode(JObject @params)
        {
            char separator = GetSeparator(@params);
            string bindName = GetString(@params, "bindName", null);
            if (string.IsNullOrEmpty(bindName))
            {
                throw new CommandException(ErrorCodes.InvalidParams, "Missing 'bindName' for rename_node.");
            }
            if (bindName.IndexOf(separator) >= 0)
            {
                throw new CommandException(ErrorCodes.InvalidParams,
                    $"bindName '{bindName}' must not contain the separator '{separator}'.");
            }

            string newName = $"{bindName}{separator}*";
            bool isArray = HasParam(@params, "arrayIndex");
            if (isArray)
            {
                int arrayIndex = GetInt(@params, "arrayIndex", 0);
                if (arrayIndex < 0)
                {
                    throw new CommandException(ErrorCodes.InvalidParams, $"arrayIndex must be >= 0, got {arrayIndex}.");
                }
                newName = $"{newName} ({arrayIndex})";
            }

            NodeContext ctx = default;
            try
            {
                if (!TryResolveNode(@params, out ctx, out string error))
                {
                    throw new CommandException(ErrorCode, error);
                }

                string oldName = ctx.Node.name;
                if (!ctx.IsPrefabContents)
                {
                    Undo.RecordObject(ctx.Node, "AgentBridge CodeBind Rename Node");
                }
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

                return new
                {
                    action = "rename_node",
                    target = ctx.TargetLabel,
                    oldName,
                    newName,
                    isArray,
                    separator = separator.ToString()
                };
            }
            finally
            {
                if (ctx.IsPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(ctx.PrefabRoot);
                }
            }
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

        private static bool TryResolveNode(JObject @params, out NodeContext ctx, out string error)
        {
            ctx = default;
            error = null;
            string assetPath = GetString(@params, "assetPath", null);
            string path = GetString(@params, "path", null);
            int instanceId = GetInt(@params, "instanceId", 0);

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

                string nodePath = GetString(@params, "nodePath", null);
                Transform node = string.IsNullOrEmpty(nodePath) ? prefabRoot.transform : prefabRoot.transform.Find(nodePath);
                if (node == null)
                {
                    error = $"Node '{nodePath}' not found in prefab '{assetPath}'.";
                    return false;
                }

                ctx.Node = node;
                ctx.TargetLabel = string.IsNullOrEmpty(nodePath) ? assetPath : $"{assetPath}:{nodePath}";
                return true;
            }

            GameObject go;
            if (instanceId != 0)
            {
#if UNITY_6000_0_OR_NEWER
                go = EditorUtility.EntityIdToObject(instanceId) as GameObject;
#else
                go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
#endif
                ctx.TargetLabel = path ?? $"instanceId:{instanceId}";
            }
            else if (!string.IsNullOrEmpty(path))
            {
                go = SceneObjectResolver.FindByPath(path);
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
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        private static bool TryResolveBinderTypes(out string error)
        {
            s_MonoCodeBinderType ??= FindType("CodeBind.Editor.MonoCodeBinder");
            s_CSCodeBinderType ??= FindType("CodeBind.Editor.CSCodeBinder");
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
            Type type = Type.GetType($"{fullName}, CodeBind.Editor");
            if (type != null)
            {
                return type;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        private static char GetSeparator(JObject @params)
        {
            string custom = GetString(@params, "separator", null);
            if (!string.IsNullOrEmpty(custom))
            {
                if (custom.Length != 1)
                {
                    throw new CommandException(ErrorCodes.InvalidParams,
                        $"separator must contain exactly one character, got '{custom}'.");
                }
                return custom[0];
            }

            string saved = EditorPrefs.GetString("CodeBind.SeparatorChar", "_");
            return string.IsNullOrEmpty(saved) ? '_' : saved[0];
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

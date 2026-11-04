using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CodeBind;
using Unity.Pipeline.Commands;
using Unity.Pipeline;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>Generate CodeBind sources, assign references, and rename bind nodes.</summary>
    // Pipeline uses Task return types to await commands and exclude them from synchronous job batches.
#pragma warning disable ET0501
    public static class CodeBindCommand
    {
        private const string ErrorCode = "CODEBIND_ERROR";
        private const string Group = "game.codebind";

        private static Type s_MonoBehaviourBinderType;
        private static Type s_PlainClassBinderType;

        // Task results preserve the standalone-only policy for code generation and Prefab saves.
        [CliGroupCommand(Group, "生成绑定代码并尝试设置引用；serializationDeferred=true 时，编译后执行 game.codebind set_serialization。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> All(
            [CliArg("asset_path", "Prefab 资源路径，优先于 instance_id/path/当前选择。")] string assetPath = null,
            [CliArg("path", "场景层级路径，例如 Canvas/LoginForm。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID，优先于 path。")] ObjectId? instanceId = null)
        {
            return Task.FromResult(RunBind(assetPath, path, instanceId, "all"));
        }

        [CliGroupCommand(Group, "生成 *.Bind.cs。新字段编译完成后，再执行 game.codebind set_serialization 设置引用。")]
        public static Task<object> GenerateCode(
            [CliArg("asset_path", "Prefab 资源路径，优先于 instance_id/path/当前选择。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID，优先于 path。")] ObjectId? instanceId = null)
        {
            return Task.FromResult(RunBind(assetPath, path, instanceId, "generate_code"));
        }

        [CliGroupCommand(Group, "设置已编译的 CodeBind 序列化引用。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> SetSerialization(
            [CliArg("asset_path", "Prefab 资源路径，优先于 instance_id/path/当前选择。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID，优先于 path。")] ObjectId? instanceId = null)
        {
            return Task.FromResult(RunBind(assetPath, path, instanceId, "set_serialization"));
        }

        private static object RunBind(string requestedAssetPath, string path, ObjectId? instanceId, string action)
        {
            if (!TryResolveBinderTypes(out string typeError))
            {
                throw new GameCommandException(ErrorCode, typeError);
            }

            if (!GameCommandTarget.TryResolveRoot(requestedAssetPath, path, instanceId,
                    out GameObject root, out bool isPrefabContents, out string assetPath,
                    out string targetLabel, out string error))
            {
                throw new GameCommandException(ErrorCode, error);
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
                    throw new GameCommandException(ErrorCode,
                        $"No [MonoBehaviourBinding] component or PlainClassBindingHost found under '{targetLabel}'.");
                }

                if (!isPrefabContents)
                {
                    Undo.RegisterFullObjectHierarchyUndo(root, "CLI CodeBind");
                }

                if (genCode)
                {
                    foreach (BindTarget target in targets)
                    {
                        InvokeBinder(target, "TryGenerateBindingSource");
                        codeGenerated.Add(target.Label);
                    }
                }

                if (serialize)
                {
                    foreach (BindTarget target in targets)
                    {
                        try
                        {
                            InvokeBinder(target, "TrySerializeBindings");
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
                    ? $"Bind code changed. Compile Unity, then run game.codebind set_serialization. ({deferReason})"
                    : null
            };
        }

        [CliGroupCommand(Group, "将节点改名为 bind_name<separator>*；array_index 可生成数组绑定名。Prefab 自动保存，Scene 标记 dirty。")]
        public static Task<object> RenameNode(
            [CliArg("bind_name", "绑定字段名，不得包含 separator。", Required = true)] string bindName,
            [CliArg("asset_path", "Prefab 资源路径，优先于 instance_id/path/当前选择。")] string assetPath = null,
            [CliArg("path", "场景层级路径。")] string path = null,
            [CliArg("instance_id", "场景对象实例 ID，优先于 path。")] ObjectId? instanceId = null,
            [CliArg("node_path", "目标根节点下的相对路径；省略时操作根节点。")] string nodePath = null,
            [CliArg("array_index", "可选的非负数组索引；省略时生成单字段绑定名。")] int? arrayIndex = null,
            [CliArg("separator", "单字符分隔符，默认读取 CodeBind 编辑器设置。")] string separator = null)
        {
            return Task.FromResult(RenameNodeCore(bindName, assetPath, path, instanceId, nodePath, arrayIndex, separator));
        }

        private static object RenameNodeCore(string bindName, string assetPath, string path, ObjectId? instanceId,
            string nodePath, int? arrayIndex, string separator)
        {
            char nameSeparator = GetSeparator(separator);
            if (string.IsNullOrEmpty(bindName))
            {
                throw new GameCommandException(GameCommandException.InvalidParams, "Missing 'bind_name' for rename_node.");
            }
            if (bindName.IndexOf(nameSeparator) >= 0)
            {
                throw new GameCommandException(GameCommandException.InvalidParams,
                    $"bind_name '{bindName}' must not contain the separator '{nameSeparator}'.");
            }

            string newName = $"{bindName}{nameSeparator}*";
            bool isArray = arrayIndex.HasValue;
            if (isArray)
            {
                if (arrayIndex.Value < 0)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams, $"array_index must be >= 0, got {arrayIndex}.");
                }
                newName = $"{newName} ({arrayIndex.Value})";
            }

            if (!GameCommandTarget.TryResolveRoot(assetPath, path, instanceId,
                    out GameObject root, out bool isPrefabContents, out string resolvedAssetPath,
                    out string targetLabel, out string error))
            {
                throw new GameCommandException(ErrorCode, error);
            }
            try
            {
                Transform node = string.IsNullOrEmpty(nodePath) ? root.transform : root.transform.Find(nodePath);
                if (node == null)
                {
                    throw new GameCommandException(ErrorCode, $"Node '{nodePath}' not found under '{targetLabel}'.");
                }
                string oldName = node.name;
                if (!isPrefabContents)
                {
                    Undo.RecordObject(node.gameObject, "CLI CodeBind Rename Node");
                }
                node.name = newName;

                if (isPrefabContents)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, resolvedAssetPath);
                }
                else
                {
                    EditorUtility.SetDirty(node.gameObject);
                    if (node.gameObject.scene.IsValid())
                    {
                        EditorSceneManager.MarkSceneDirty(node.gameObject.scene);
                    }
                }

                return new
                {
                    action = "rename_node",
                    target = string.IsNullOrEmpty(nodePath) ? targetLabel : $"{targetLabel}:{nodePath}",
                    oldName,
                    newName,
                    isArray,
                    separator = nameSeparator.ToString()
                };
            }
            finally
            {
                if (isPrefabContents)
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
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

                object[] attrs = mono.GetType().GetCustomAttributes(typeof(MonoBehaviourBindingAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                var attr = (MonoBehaviourBindingAttribute)attrs[0];
                MonoScript script = MonoScript.FromMonoBehaviour(mono);
                targets.Add(new BindTarget
                {
                    BinderType = s_MonoBehaviourBinderType,
                    BinderArgs = new object[] { script, mono.transform, attr.NameSeparator },
                    Label = mono.GetType().Name
                });
            }

            PlainClassBindingHost[] plainClassHosts = root.GetComponentsInChildren<PlainClassBindingHost>(true);
            foreach (PlainClassBindingHost host in plainClassHosts)
            {
                targets.Add(new BindTarget
                {
                    BinderType = s_PlainClassBinderType,
                    BinderArgs = new object[] { host.BindingClassScript, host.transform, host.NameSeparator },
                    Label = $"{host.name}(PlainClassBindingHost)"
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
            s_MonoBehaviourBinderType ??= FindType("CodeBind.Editor.MonoBehaviourBinder");
            s_PlainClassBinderType ??= FindType("CodeBind.Editor.PlainClassBinder");
            if (s_MonoBehaviourBinderType == null || s_PlainClassBinderType == null)
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

            foreach (Assembly assembly in UnityEngine.Assemblies.CurrentAssemblies.GetLoadedAssemblies())
            {
                type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        private static char GetSeparator(string custom)
        {
            if (custom != null)
            {
                if (custom.Length != 1)
                {
                    throw new GameCommandException(GameCommandException.InvalidParams,
                        $"separator must contain exactly one character, got '{custom}'.");
                }
                return custom[0];
            }

            string saved = EditorPrefs.GetString("CodeBind.NameSeparator", "_");
            return string.IsNullOrEmpty(saved) ? '_' : saved[0];
        }

        private struct BindTarget
        {
            public Type BinderType;
            public object[] BinderArgs;
            public string Label;
        }
    }
#pragma warning restore ET0501
}

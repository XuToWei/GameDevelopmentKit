using System;
using System.Collections.Generic;
using System.Text;
using Unity.Pipeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Editor
{
    /// <summary>Resolve Prefabs, loaded scene objects (including inactive objects), or the selection.</summary>
    internal static class GameCommandTarget
    {
        public static bool TryResolveRoot(string requestedAssetPath, string path, ObjectId? instanceId,
            out GameObject root, out bool isPrefabContents, out string assetPath,
            out string targetLabel, out string error)
        {
            root = null;
            isPrefabContents = false;
            assetPath = requestedAssetPath;
            targetLabel = requestedAssetPath ?? path ?? $"instance_id:{instanceId}";
            error = null;

            if (requestedAssetPath != null)
            {
                if (!requestedAssetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                {
                    error = $"asset_path must be a .prefab: {requestedAssetPath}";
                    return false;
                }
                if (AssetDatabase.LoadAssetAtPath<GameObject>(requestedAssetPath) == null)
                {
                    error = $"Prefab not found: {requestedAssetPath}";
                    return false;
                }
                root = PrefabUtility.LoadPrefabContents(requestedAssetPath);
                isPrefabContents = true;
                return true;
            }

            if (instanceId.HasValue)
            {
                root = PipelineUtils.IdToObject(instanceId.Value) as GameObject;
            }
            else if (path != null)
            {
                List<GameObject> matches = FindByPath(path);
                if (matches.Count > 1)
                {
                    error = $"Scene path '{path}' is ambiguous ({matches.Count} objects). Use instance_id.";
                    return false;
                }
                root = matches.Count == 1 ? matches[0] : null;
            }
            else
            {
                root = Selection.activeGameObject;
                targetLabel = root != null ? root.name : null;
            }

            if (root == null)
            {
                error = "Target not found. Provide asset_path (Prefab), path/instance_id (scene), or select a GameObject.";
                return false;
            }
            if (EditorUtility.IsPersistent(root) || !root.scene.IsValid())
            {
                error = "Target is not a scene object. Use asset_path to edit a Prefab asset.";
                root = null;
                return false;
            }
            return true;
        }

        private static List<GameObject> FindByPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new List<GameObject>();
            }

            string[] raw = path.Trim('/').Split('/');
            var decoded = new string[raw.Length];
            bool validEncoding = true;
            for (int i = 0; i < raw.Length; i++)
            {
                if (!TryDecodeSegment(raw[i], out decoded[i]))
                {
                    validEncoding = false;
                    break;
                }
            }

            List<GameObject> matches = validEncoding ? FindBySegments(decoded) : new List<GameObject>();
            // Accept both the former bridge's escaped paths and literal Unity hierarchy paths.
            if (!validEncoding || path.IndexOf('~') >= 0)
            {
                foreach (GameObject candidate in FindBySegments(raw))
                {
                    if (!matches.Contains(candidate))
                    {
                        matches.Add(candidate);
                    }
                }
            }
            return matches;
        }

        private static List<GameObject> FindBySegments(string[] segments)
        {
            var result = new List<GameObject>();
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.isLoaded)
                {
                    continue;
                }
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    if (root.name != segments[0])
                    {
                        continue;
                    }
                    var matches = new List<Transform> { root.transform };
                    for (int segmentIndex = 1; segmentIndex < segments.Length && matches.Count > 0; segmentIndex++)
                    {
                        var next = new List<Transform>();
                        foreach (Transform parent in matches)
                        {
                            for (int childIndex = 0; childIndex < parent.childCount; childIndex++)
                            {
                                Transform child = parent.GetChild(childIndex);
                                if (child.name == segments[segmentIndex])
                                {
                                    next.Add(child);
                                }
                            }
                        }
                        matches = next;
                    }
                    foreach (Transform match in matches)
                    {
                        result.Add(match.gameObject);
                    }
                }
            }
            return result;
        }

        private static bool TryDecodeSegment(string segment, out string decoded)
        {
            decoded = null;
            if (segment.Length == 0)
            {
                return false;
            }
            if (segment == "~2")
            {
                decoded = string.Empty;
                return true;
            }
            var builder = new StringBuilder(segment.Length);
            for (int i = 0; i < segment.Length; i++)
            {
                if (segment[i] != '~')
                {
                    builder.Append(segment[i]);
                    continue;
                }
                if (++i >= segment.Length || (segment[i] != '0' && segment[i] != '1'))
                {
                    return false;
                }
                builder.Append(segment[i] == '0' ? '~' : '/');
            }
            decoded = builder.ToString();
            return true;
        }
    }
}

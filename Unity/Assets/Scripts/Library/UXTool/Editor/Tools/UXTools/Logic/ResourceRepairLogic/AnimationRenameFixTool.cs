#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
#if !UNITY_2021_2_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace ThunderFireUITool
{
    public class AnimationRenameFixTool : EditorWindow
    {
        private GameObject target;
        private AnimationClip anim;
        private string nameBefore;
        private GameObject curObj;
        private List<string> previousNames;

        [MenuItem(ThunderFireUIToolConfig.Menu_ResourceCheck + "/UI动画重命名修复(UIAnimRenameFix)", false, 160)]
        private static void ShowWindow()
        {
            var window = GetWindow<AnimationRenameFixTool>();
            window.titleContent = new GUIContent("动画资源重命名修复");
            window.Show();
        }

        bool DoFix()
        {
            if (target == null)
                return false;
            if (anim == null)
                return false;
            if (nameBefore == null || nameBefore == string.Empty)
                return false;

            GameObject root = target;
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);
            string newPath = AnimationUtility.CalculateTransformPath(curObj.transform, root.transform);
            string parentPath = GetParentPath(newPath);
            string oldPath = parentPath + nameBefore;
            for (int i = 0; i < bindings.Length; i++)
            {
                var binding = bindings[i];
                if (isSubPath(binding.path, oldPath))
                {
                    string oldBindingPath = binding.path;
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(anim, binding);
                    AnimationUtility.SetEditorCurve(anim, binding, null);
                    binding.path = binding.path.Replace(oldPath, newPath);
                    AnimationUtility.SetEditorCurve(anim, binding, curve);
                    Debug.Log($"[AnimationFix] Convert {oldBindingPath} to {binding.path}");
                }
            }

            return true;
        }
        bool isSubPath(string path, string oldPath)
        {
            bool result = path.Contains(oldPath) && (path == oldPath || path[path.IndexOf(oldPath) + oldPath.Length] == '/');
            return result;
        }
        string GetParentPath(string path)
        {
            int lastSlashIndex = path.LastIndexOf('/');
            string result = lastSlashIndex > 0 ? path.Substring(0, lastSlashIndex + 1) : "";
            return result;
        }
        string GetNameByPath(string path)
        {
            string[] segments = path.Trim('/').Split('/');
            string lastSegment = segments[segments.Length - 1];
            return lastSegment;
        }

        private void OnGUI()
        {

            EditorGUILayout.LabelField("Root, 自动选中当前打开prefab");
            target = EditorGUILayout.ObjectField("", target, typeof(GameObject), false) as GameObject;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                target = prefabStage.prefabContentsRoot;
            }
            EditorGUILayout.LabelField("要修复的AnimationClip");
            anim = EditorGUILayout.ObjectField("", anim, typeof(AnimationClip), false) as AnimationClip;
            EditorGUILayout.LabelField("修改前节点名, 点检查自动设置");
            nameBefore = EditorGUILayout.TextField("", nameBefore);
            EditorGUILayout.LabelField("选中对应节点, 多个待修复时需优先父节点");
            curObj = EditorGUILayout.ObjectField("", curObj, typeof(GameObject), false) as GameObject;
            var selectedObj = Selection.activeGameObject;
            if (selectedObj != null)
            {
                curObj = selectedObj;
            }
            if (GUILayout.Button("检查节点曾用名", GUILayout.Width(200)))
            {
                if (selectedObj != null)
                {
                    curObj = selectedObj;
                }
                SetCurrentObject();
            }
            if (GUILayout.Button("点击修复(单节点)", GUILayout.Width(200)))
            {
                DoFix();
            }
            if (GUILayout.Button("一键修复", GUILayout.Width(200)))
            {
                AllFix();
            }
        }

        private void AllFix()
        {
            Transform[] allObjectsTrans = target.GetComponentsInChildren<Transform>(true);
            foreach (Transform trans in allObjectsTrans)
            {
                GameObject obj = trans.gameObject;
                if (obj == target)
                    continue;
                Debug.Log($"[AnimationFix] fix {obj.name}");
                curObj = obj;
                SetCurrentObject();
                DoFix();
            }
        }

        private void SetCurrentObject()
        {
            if (target == null)
                return;
            if (anim == null)
                return;
            if (curObj == null)
                return;

            previousNames = AnimationRenameListener.GetPreviousNames(curObj.name);
            GameObject root = target;
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);
            string newPath = AnimationUtility.CalculateTransformPath(curObj.transform, root.transform);
            string parentPath = GetParentPath(newPath);
            foreach (var oldName in previousNames)
            {
                string oldPath = parentPath + oldName;
                for (int i = 0; i < bindings.Length; i++)
                {
                    var binding = bindings[i];
                    if (isSubPath(binding.path, oldPath))
                    {
                        nameBefore = oldName;
                        Debug.Log($"[AnimationFix] Get previous name {oldName}");
                        return;
                    }
                }
            }

        }
    }
}
#endif
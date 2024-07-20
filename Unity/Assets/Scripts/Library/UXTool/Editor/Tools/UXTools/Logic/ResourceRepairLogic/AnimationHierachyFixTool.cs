#if UNITY_EDITOR 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ThunderFireUnityEx;
#if !UNITY_2021_2_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace ThunderFireUITool
{
    public class AnimationHierarchyFixTool : EditorWindow
    {
        private GameObject target;
        private AnimationClip anim;

        [MenuItem(ThunderFireUIToolConfig.Menu_ResourceCheck + "/UI动画层级修复(UIAnimHierarchyFix)", false, 160)]
        private static void ShowWindow()
        {
            var window = GetWindow<AnimationHierarchyFixTool>();
            window.titleContent = new GUIContent("动画资源层级修复");
            window.Show();
        }

        bool DoFix()
        {
            if (target != null && anim != null)
            {
                GameObject root = target;
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);

                for (int i = 0; i < bindings.Length; i++)
                {
                    var binding = bindings[i];
                    GameObject bindObj = AnimationUtility.GetAnimatedObject(root, binding) as GameObject;
                    //Debug.Log($"Binding path is {binding.path}");
                    if (bindObj == null)
                    {
                        //发生了missing，层级变了
                        string name = GetNameByPath(binding.path);

                        var bindTrans = root.transform.FindChildRecursive(name);
                        if (bindTrans != null)
                        {
                            string newPath = AnimationUtility.CalculateTransformPath(bindTrans, root.transform);
                            Debug.Log($"[AnimationFix] change {binding.path} to {newPath}");

                            AnimationCurve curve = AnimationUtility.GetEditorCurve(anim, binding);
                            AnimationUtility.SetEditorCurve(anim, binding, null);
                            binding.path = newPath;
                            AnimationUtility.SetEditorCurve(anim, binding, curve);
                        }
                    }
                }
            }
            return true;
        }
        string GetNameByPath(string path)
        {
            string[] segments = path.Trim('/').Split('/');
            string lastSegment = segments[segments.Length - 1];
            return lastSegment;
        }

        private void OnGUI()
        {

            EditorGUILayout.LabelField("Root,自动选中当前打开prefab");
            target = EditorGUILayout.ObjectField("", target, typeof(GameObject), false) as GameObject;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                target = prefabStage.prefabContentsRoot;
            }
            EditorGUILayout.LabelField("要修复的AnimationClip");
            anim = EditorGUILayout.ObjectField("", anim, typeof(AnimationClip), false) as AnimationClip;

            if (GUILayout.Button("点击修复", GUILayout.Width(200)))
            {
                //干活
                DoFix();
            }
        }
    }
}
#endif
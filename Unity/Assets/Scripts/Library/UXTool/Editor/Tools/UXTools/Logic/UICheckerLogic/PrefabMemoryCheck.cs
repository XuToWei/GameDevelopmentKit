#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace ThunderFireUITool
{
    public class PrefabMemoryCheck
    {
        [MenuItem(ThunderFireUIToolConfig.Menu_ResourceCheck + "/Prefab 内存检查(Prefab MemoryCheck)", false, ThunderFireUIToolConfig.Menu_ResourceCheckIndex + 6)]
        private static void BeginMemoryCheck()
        {
            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.ResourceCheck);
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                Debug.LogWarning("请打开一个Prefab后再执行此操作！");
                return;
            }

            long totalMemory = 0;

            var dependencies = EditorUtility.CollectDependencies(new Object[] { prefabStage.prefabContentsRoot });
            foreach (var dependency in dependencies)
            {
                var path = AssetDatabase.GetAssetPath(dependency);
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    continue;

                if (dependency.GetType() == typeof(Sprite))
                {
                    var sprite = dependency as Sprite;
                    var textureUtilType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TextureUtil");
                    var size = (long)Utils.InvokeMethod(textureUtilType, "GetStorageMemorySizeLong", new object[] { sprite.texture });
                    Debug.Log(sprite + ": " + EditorUtility.FormatBytes(size));
                    totalMemory += size;
                }
                else if (dependency.GetType() == typeof(AnimationClip))
                {
                    var clip = dependency as AnimationClip;
                    var stats = Utils.InvokeMethod(typeof(AnimationUtility), "GetAnimationClipStats", new object[] { clip });
                    var size = (int)Utils.GetFieldValue(stats, "size");
                    Debug.Log(clip + ": " + EditorUtility.FormatBytes(size));
                    totalMemory += size;
                }
            }

            var objSize = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(prefabStage.prefabContentsRoot);
            Debug.Log(prefabStage.prefabContentsRoot + ": " + EditorUtility.FormatBytes(objSize));
            totalMemory += objSize;

            Debug.Log("总占用内存：" + EditorUtility.FormatBytes(totalMemory));
        }
    }
}
#endif
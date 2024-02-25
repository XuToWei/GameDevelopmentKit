#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;
#else
using PrefabStageUtility = UnityEditor.Experimental.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStage;
#endif

namespace ThunderFireUITool
{
    [UXInitialize]
    public class UIBeginnerGuidePreview
    {
        static UIBeginnerGuidePreview()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        public static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (EditorPrefs.GetBool("InGuidePreview"))
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    ExitPreviewGuide();
                }
            }
        }
        public static void PreviewGuide(GameObject guideRoot, string previewGuideId)
        {
            EditorPrefs.SetBool("InGuidePreview", true);

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                //场景中进行新手引导预览
                var guideDataList = guideRoot.GetComponent<UIBeginnerGuideDataList>();

                var guideManager = Object.FindObjectOfType<UIBeginnerGuideManager>();
                if (guideManager != null)
                {
                    CreateGuideLauncher(guideDataList, previewGuideId, false);
                }
                else
                {
                    CreateGuideLauncher(guideDataList, previewGuideId, true);
                }

                Utils.EnterPlayMode();
            }
            else
            {
                var guideDataList = guideRoot.GetComponent<UIBeginnerGuideDataList>();
                string guid = guideDataList.guid;

                //初始化PreviewScene
                PreviewLogic.InitPreviewScene(prefabStage);

                var allGuideDataList = Object.FindObjectsOfType<UIBeginnerGuideDataList>();

                foreach (var t in allGuideDataList)
                {
                    if (t.guid == guid)
                    {
                        CreateGuideLauncher(t, previewGuideId, true);
                        break;
                    }
                }

                Utils.ExitPrefabStage();
                Utils.EnterPlayMode();
            }
        }

        /// <summary>
        /// 退出运行并且重新打开之前的场景和prefab
        /// </summary>
        public static void ExitPreviewGuide()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;

            string guideLauncherObjName = EditorPrefs.GetString("GuideLauncherObjName");
            //清理预览所需的临时对象
            if (!string.IsNullOrEmpty(guideLauncherObjName))
            {
                Object.DestroyImmediate(GameObject.Find(guideLauncherObjName));
                EditorPrefs.DeleteKey("GuideLauncherObjName");
            }

            PreviewLogic.ResumeOriginScene();
        }

        private static GameObject CreateGuideLauncher(UIBeginnerGuideDataList guideList, string previewGuideId, bool needGuideManager = false)
        {
            GameObject guideLanucherObj = new GameObject("TEMP_GuideLanucherObj");
            UIBeginnerGuidePreviewLauncher launcher = guideLanucherObj.AddComponent<UIBeginnerGuidePreviewLauncher>();
            launcher.guideList = guideList;
            launcher.previewGuideId = previewGuideId;

            EditorPrefs.SetString("GuideLauncherObjName", "TEMP_GuideLanucherObj");

            if (needGuideManager)
            {
                guideLanucherObj.AddComponent<UIBeginnerGuideManager>();
            }

            return guideLanucherObj;
        }
    }
}
#endif
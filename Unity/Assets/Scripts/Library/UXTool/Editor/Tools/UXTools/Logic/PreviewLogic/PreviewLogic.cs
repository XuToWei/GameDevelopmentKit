#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public class PreviewLogic
    {
        static PreviewLogic()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        public static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (EditorPrefs.GetBool("InPreview"))
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    ExitPreview();
                }
            }
        }

        /// <summary>
        /// 打开PreviewScene, 将当前正在编辑的prefab放到UXPreviewCanvas下
        /// 切换到playmode进行预览
        /// </summary>
        public static void Preview()
        {
            EditorPrefs.SetBool("InPreview", true);

            PrefabStage prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                //预览是预览一个Prefab的动画等，因此必须要在PrefabMode中才能用
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请打开Prefab后再进行预览),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }

            InitPreviewScene(prefabStage);
            Utils.ExitPrefabStage();
            Utils.EnterPlayMode();
        }

        /// <summary>
        /// 退出预览并且重新打开之前的场景和prefab
        /// </summary>
        public static void ExitPreview()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            ResumeOriginScene();
        }

        public static void ClearPreviewCanvas(GameObject previewCanvas)
        {
            int count = previewCanvas.transform.childCount;
            for (int i = count - 1; i > 0; i--)
            {
                Object.DestroyImmediate(previewCanvas.transform.GetChild(i).gameObject);
            }
        }
        public static GameObject InitPreviewCanvas(GameObject prefab, GameObject previewCanvas)
        {
            return Object.Instantiate(prefab, previewCanvas.transform);
        }
        public static GameObject RefreshPreviewCanvas(GameObject prefab, GameObject previewCanvas)
        {
            ClearPreviewCanvas(previewCanvas);
            return InitPreviewCanvas(prefab, previewCanvas);
        }
        //在PrefabMode时,创建PreviewScene来替换当前Scene,并把当前Prefab初始化到PreviewScene中
        public static void InitPreviewScene(PrefabStage prefabStage)
        {
            string prefabPath = prefabStage.GetAssetPath();
            var tempPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            //缓存当前Prefab路径,退出预览时重新打开
            EditorPrefs.SetString(ThunderFireUIToolConfig.PreviewPrefabPath, prefabPath);

            GameObject previewCanvas = GameObject.Find("UXPreviewCanvas");
            if (previewCanvas == null)
            {
                //缓存当前Scene路径,退出预览时重新打开
                Scene originScene = SceneManager.GetActiveScene();
                string originScenePath = originScene.path;
                EditorPrefs.SetString(ThunderFireUIToolConfig.PreviewOriginScene, originScenePath);

                if (EditorUtility.DisplayDialog("Save",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否想要保存场景),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
                {
                    EditorSceneManager.SaveScene(originScene);
                }

                EditorSceneManager.OpenScene(ThunderFireUIToolConfig.ScenePath + "PreviewScene.unity", OpenSceneMode.Single);
                previewCanvas = GameObject.Find("UXPreviewCanvas");
                InitPreviewCanvas(tempPrefab, previewCanvas);
            }
            else
            {
                RefreshPreviewCanvas(tempPrefab, previewCanvas);
            }
        }
        public static void ResumeOriginScene()
        {
            Utils.StopPlayMode();
            string prefabPath = EditorPrefs.GetString(ThunderFireUIToolConfig.PreviewPrefabPath);
            string OriginScenePath = EditorPrefs.GetString(ThunderFireUIToolConfig.PreviewOriginScene);

            if (!string.IsNullOrEmpty(OriginScenePath))
            {
                EditorSceneManager.OpenScene(OriginScenePath, OpenSceneMode.Single);
                if (!string.IsNullOrEmpty(prefabPath))
                {
                    Utils.OpenPrefab(prefabPath);
                }
            }


            EditorPrefs.DeleteKey(ThunderFireUIToolConfig.PreviewPrefabPath);
            EditorPrefs.DeleteKey(ThunderFireUIToolConfig.PreviewOriginScene);
            SceneViewToolBar.TryOpenToolbar();
        }
    }
}
#endif
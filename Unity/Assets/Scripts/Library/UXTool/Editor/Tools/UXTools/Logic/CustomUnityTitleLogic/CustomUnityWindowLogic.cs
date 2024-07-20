#if UNITY_EDITOR_WIN
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

[UXInitialize]
class CustomUnityWindowLogic
{
    static CustomUnityWindowLogic()
    {
        EditorApplication.delayCall += () =>
        {
            if (!Application.isPlaying)
            {
                DoUpdateTitleFunc();
            }
        };

        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += (scene, mode) =>
        {
            EditorApplication.delayCall += DoUpdateTitleFunc;
        };

        PrefabStageUtils.AddOpenedEvent((p) =>
        {
            EditorApplication.delayCall += DoUpdateTitleFunc;
        });
    }

    static void OnPlaymodeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.delayCall += DoUpdateTitleFunc;
        }
    }

    public static void DoUpdateTitleFunc()
    {
        //UnityEngine.Debug.Log("DoUpdateTitleFunc");
        UXToolCommonData data = AssetDatabase.LoadAssetAtPath<UXToolCommonData>(ThunderFireUIToolConfig.UXToolCommonDataPath);
        if (data != null)
        {
            CustomUnityWindowHelper.Instance.SetTitle(data.CustomUnityWindowTitle);
        }
    }
}
#endif
using System;
using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.SceneManagement.PrefabStage;
#else
using PrefabStageUtility = UnityEditor.Experimental.SceneManagement.PrefabStageUtility;
using PrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStage;
#endif

public static class PrefabStageUtils
{
    public static PrefabStage GetCurrentPrefabStage()
    {
        return PrefabStageUtility.GetCurrentPrefabStage();
    }

    public static string GetAssetPath(this PrefabStage p)
    {
#if UNITY_2020_1_OR_NEWER
        return p.assetPath;
#else
        return p.prefabAssetPath;
#endif
    }

    public static void AddOpenedEvent(Action<PrefabStage> action)
    {
        PrefabStage.prefabStageOpened += action;
    }
    public static void RemoveOpenedEvent(Action<PrefabStage> action)
    {
        PrefabStage.prefabStageOpened -= action;
    }

    public static void AddSavingEvent(Action<GameObject> action)
    {
        PrefabStage.prefabSaving += action;
    }

    public static void AddSavedEvent(Action<GameObject> action)
    {
        PrefabStage.prefabSaved += action;
    }

    public static void AddClosingEvent(Action<PrefabStage> action)
    {
        PrefabStage.prefabStageClosing += action;
    }
    public static void RemoveClosingEvent(Action<PrefabStage> action)
    {
        PrefabStage.prefabStageClosing -= action;
    }
}
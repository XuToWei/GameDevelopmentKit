#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    public class PrefabActiveLogic
    {
        static bool recordSwitcher = true;
        static Dictionary<string, bool> activeRecordDic;
        static UXBuilderDiv customBackArrow;

        static PrefabActiveLogic()
        {
            PrefabStageUtils.AddOpenedEvent((p) =>
            {
                if (recordSwitcher)
                {
                    RecordActiveState();
                    if (customBackArrow == null)
                    {
                        AddCustomBackArrowInHierarchy();
                    }
                }
            });

            PrefabStageUtils.AddClosingEvent((p) =>
            {
                RemoveCustomBackArrowInHierarchy();
            });

        }
        static void AddCustomBackArrowInHierarchy()
        {
            EditorWindow hierarchyWindow = Utils.GetHierarchyWindow();
            if (hierarchyWindow != null && customBackArrow == null)
            {
                //GUIStyle backArrowStyle = new GUIStyle();
                //Rect BackArrowRect = new Rect(0, 21, backArrowStyle.fixedWidth + backArrowStyle.margin.horizontal, 25 - 1);
                customBackArrow = UXBuilder.Div(hierarchyWindow.rootVisualElement, new UXBuilderDivStruct
                {
                    style = new UXStyle
                    {
                        left = 0,
                        top = 21,
                        width = 24,
                        height = 24,
                        backgroundColor = new Color(56.0f / 255, 56.0f / 255, 56.0f / 255, 1)
                    },
                    className = "backArrow"
                });
                customBackArrow.style.alignItems = Align.Center;
                customBackArrow.style.justifyContent = Justify.Center;

                var arrow = new Image();
                arrow.style.width = 6;
                arrow.style.height = 9;
                arrow.image = ToolUtils.GetIcon("prefabBackArrow");
                customBackArrow.Add(arrow);

                customBackArrow.RegisterCallback((PointerDownEvent e) =>
                {
                    if (e.button == 0)
                    {
                        OnBackArrowClick();
                    }
                });
            }
        }

        static void RemoveCustomBackArrowInHierarchy()
        {
            EditorWindow hierarchyWindow = Utils.GetHierarchyWindow();
            if (hierarchyWindow != null && customBackArrow != null)
            {
                hierarchyWindow.rootVisualElement.Remove(customBackArrow);
                customBackArrow = null;
            }
        }
        static void OnBackArrowClick()
        {
            Dictionary<string, bool> changedGameObjects;

            var stage = PrefabStageUtils.GetCurrentPrefabStage();
#if UNITY_2019_4
            var hasUnsavedChanges = Utils.InvokeMethod(stage, "HasSceneBeenModified");
#else
            var hasUnsavedChanges = Utils.GetPropertyValue(stage, "hasUnsavedChanges");
#endif
            if ((bool)hasUnsavedChanges)
            {
                Utils.InvokeMethod(stage, "AskUserToSaveDirtySceneBeforeDestroyingScene");
            }

            bool changed = CheckActiveState(out changedGameObjects);

            if (changed)
            {
                ShowAlertWindow(changedGameObjects);
            }
            else
            {
                Utils.ExitPrefabStage();
            }
            activeRecordDic.Clear();
        }
        static void ShowAlertWindow(Dictionary<string, bool> activeChangeDic)
        {
            var result = EditorUtility.DisplayDialogComplex(
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_显隐状态变化),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_是否还原显隐状态变化),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_还原),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_不还原),
                EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));

            if (result == 0)
            {
                //Revert
                var stage = PrefabStageUtils.GetCurrentPrefabStage();
                GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(stage.GetAssetPath());
                Transform[] gos = root.transform.GetComponentsInChildren<Transform>(true);

                Dictionary<string, Transform> prefabGameObjects = new Dictionary<string, Transform>();
                for (int i = 0; i < gos.Length; i++)
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gos[i], out string guid, out long _);
                    prefabGameObjects[guid] = gos[i];
                }

                foreach (var kvp in activeChangeDic)
                {
                    if (prefabGameObjects.ContainsKey(kvp.Key))
                    {
                        prefabGameObjects[kvp.Key].gameObject.SetActive(kvp.Value);
                    }
                }
                EditorUtility.SetDirty(root);
                AssetDatabase.Refresh();

                Utils.ExitPrefabStage();
            }
            else if (result == 1)
            {
                //Discard
                Utils.ExitPrefabStage();
            }
        }
        static void RecordActiveState()
        {
            activeRecordDic = new Dictionary<string, bool>();
            var stage = PrefabStageUtils.GetCurrentPrefabStage();
            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(stage.GetAssetPath());
            Transform[] gos = root.transform.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < gos.Length; i++)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gos[i].GetInstanceID(), out string guid, out long _);

                activeRecordDic[guid] = gos[i].gameObject.activeSelf;
            }
        }
        static bool CheckActiveState(out Dictionary<string, bool> changedGameObjects)
        {
            changedGameObjects = new Dictionary<string, bool>();

            var stage = PrefabStageUtils.GetCurrentPrefabStage();
            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(stage.GetAssetPath());
            Transform[] gos = root.transform.GetComponentsInChildren<Transform>(true);

            Dictionary<string, Transform> prefabGameObjects = new Dictionary<string, Transform>();
            for (int i = 0; i < gos.Length; i++)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gos[i], out string guid, out long _);
                prefabGameObjects[guid] = gos[i];
            }

            bool changed = false;

            foreach (var kvp in activeRecordDic)
            {
                if (prefabGameObjects.ContainsKey(kvp.Key))
                {
                    if (kvp.Value != prefabGameObjects[kvp.Key].gameObject.activeSelf)
                    {
                        changedGameObjects[kvp.Key] = kvp.Value;
                        changed = true;
                    }
                }
            }
            return changed;
        }
    }
}
#endif
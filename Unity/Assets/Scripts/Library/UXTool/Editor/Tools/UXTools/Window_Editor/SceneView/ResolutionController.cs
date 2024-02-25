#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection;
using System;


namespace ThunderFireUITool
{
    public class ResolutionController
    {
        //UI
        public static VisualElement Root;
        public static IMGUIContainer imGUIContainer;
        private static object gameView;

        //private MethodInfo GameViewDoToolBarGUI_Method;
        private static PropertyInfo selectInfo;
        private static MethodInfo GameViewSizePopup_Method;

        private static int selectedSizeIndex = -1;
        private static GameViewSizeGroupType currentSizeGroupType;

        public static bool loaded;


        public static void InitResolutionController()
        {
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.ResolutionAdjustment))
            {
                loaded = false;
                return;
            }
            loaded = InitWindowData();
            if (loaded)
            {
                VisualTreeAsset resolutionTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ThunderFireUIToolConfig.UIBuilderPath + "imageResolution.uxml");

                Root = resolutionTreeAsset.CloneTree().Children().First();
                imGUIContainer = Root.Q<IMGUIContainer>("IMGUIContainer");


                imGUIContainer.onGUIHandler += () =>
                {
                    GUILayoutOption[] ops = new GUILayoutOption[] { GUILayout.Width(160f) };
                    GameViewSizePopup_Method.Invoke(null, new object[] { currentSizeGroupType, selectedSizeIndex, gameView, EditorStyles.toolbarPopup, ops });
                    OnGUI();
                };
            }
            RefreshResolution();
        }
        public static void RefreshResolution()
        {
#if UNITY_2021_3_OR_NEWER
            if (Root != null)
            {
                Root.style.top = 0;
            }
#else
            if(Root!=null)
            {
                var prefab = PrefabStageUtils.GetCurrentPrefabStage();
                Root.style.top = prefab == null ? 21 : 46;
            }
#endif
        }
        private static bool InitWindowData()
        {
            //GameView = Utils.GetGameView();
            gameView = Utils.GetMainPlayModeView();
            //Debug.Log(gameView.GetType());
            if (gameView.GetType().ToString() == "UnityEditor.GameView")
            {
                //GameViewDoToolBarGUI_Method = Utils.GetEditorMethod(Type.GetType("UnityEditor.GameView,UnityEditor"), "DoToolbarGUI");
                GameViewSizePopup_Method = Utils.GetEditorMethod(Type.GetType("UnityEditor.EditorGUILayout,UnityEditor"), "GameViewSizePopup");

                selectInfo = typeof(Editor).Assembly.GetType("UnityEditor.GameView")
                    .GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                selectedSizeIndex = (int)selectInfo.GetValue(gameView);

                currentSizeGroupType = (GameViewSizeGroupType)typeof(Editor).Assembly.GetType("UnityEditor.GameView")
                    .GetProperty("currentSizeGroupType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .GetValue(gameView);
                return true;
            }
            return false;
        }
        public static void OnGUI()
        {
            int nowIndex = (int)selectInfo.GetValue(gameView);

            if (nowIndex != selectedSizeIndex)
            {
                selectedSizeIndex = nowIndex;

                //GUILayoutOption[] ops = new GUILayoutOption[] { GUILayout.Width(160f) };
                //GameViewSizePopup_Method.Invoke(null, new object[] { currentSizeGroupType, selectedSizeIndex, GameView, EditorStyles.toolbarPopup, ops });
            }
        }
    }

}
#endif
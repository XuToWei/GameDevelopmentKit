#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace ThunderFireUITool
{
    public class SwitchSetting
    {
        [SerializeField]
        private bool[] m_values;
        public enum SwitchType
        {
            RecentlyOpened,
            AlignSnap,
            RightClickList,
            QuickCopy,
            MovementShortcuts,
            PrefabMultiOpen,
            ResolutionAdjustment,
            PrefabResourceCheck,
            QuickBackground,
            AutoConvertTex,
            EnableGamePadBeginnerGuide
        }

        private static SwitchSetting m_instance;

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.Setting + "/SettingData", false, -41)]
        public static void Create()
        {
            m_instance = JsonAssetManager.CreateAssets<SwitchSetting>(ThunderFireUIToolConfig.SwitchSettingPath);
            int count = Enum.GetValues(typeof(SwitchType)).Length;
            m_instance.m_values = new bool[count];
            for (int i = 0; i < count; i++)
            {
                m_instance.m_values[i] = true;
            }
            m_instance.m_values[(int)SwitchType.PrefabResourceCheck] = false;
            m_instance.m_values[(int)SwitchType.AutoConvertTex] = false;
            m_instance.m_values[(int)SwitchType.EnableGamePadBeginnerGuide] = false;

            JsonAssetManager.SaveAssets(m_instance);

        }

        public static void ChangeSwitch(Toggle[] toggles)
        {
            m_instance = JsonAssetManager.GetAssets<SwitchSetting>();
            if (m_instance == null)
            {
                m_instance = JsonAssetManager.CreateAssets<SwitchSetting>(ThunderFireUIToolConfig.SwitchSettingPath);
            }
            m_instance.m_values = new bool[toggles.Length];
            for (int i = 0; i < toggles.Length; i++)
            {
                m_instance.m_values[i] = toggles[i].value;
            }
            JsonAssetManager.SaveAssets(m_instance);

            if (m_instance.m_values[(int)SwitchType.EnableGamePadBeginnerGuide] == true)
            {
                ScriptingDefineSymbolUtils.EnableInputSystemDefineSymbol();
            }
            else
            {
                ScriptingDefineSymbolUtils.DisableInputSystemDefineSymbol();
            }


            SceneViewToolBar.CloseFunction();
            SceneViewToolBar.InitFunction();
#if ODIN_INSPECTOR
            UIAtlasCheckUserData atlasData = AssetDatabase.LoadAssetAtPath<UIAtlasCheckUserData>(ThunderFireUIToolConfig.UICheckUserDataFullPath);
            if (atlasData == null)
            {
                atlasData = UIAtlasCheckUserData.Create();
            }
            atlasData.Save(CheckValid(SwitchType.PrefabResourceCheck));
#endif
        }

        public static bool CheckValid(int x)
        {
            if (m_instance == null)
            {
                m_instance = JsonAssetManager.GetAssets<SwitchSetting>();
            }
            if (m_instance == null || m_instance.m_values == null || m_instance.m_values.Length <= x)
            {
                return true;
            }
            return m_instance.m_values[x];
        }

        public static bool CheckValid(SwitchType type)
        {
            return CheckValid((int)type);
        }
    }
}
#endif

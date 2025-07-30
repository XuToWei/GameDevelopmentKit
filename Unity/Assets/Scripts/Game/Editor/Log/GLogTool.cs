using GameFramework;
using UnityEditor;
using UnityGameFramework.Runtime;

namespace Game.Editor
{
    internal static class GLogTool
    {
        [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            GameFrameworkLog.SetLogHelper(new DefaultLogHelper());
            
            GLog.Entity.IsEnabled = EditorPrefs.GetBool("Game.Log.Entity", false);
            GLog.Game.IsEnabled = EditorPrefs.GetBool("Game.Log.Game", false);
            GLog.Procedure.IsEnabled = EditorPrefs.GetBool("Game.Log.Procedure", false);
            GLog.Resource.IsEnabled = EditorPrefs.GetBool("Game.Log.Resource", false);
            GLog.Scene.IsEnabled = EditorPrefs.GetBool("Game.Log.Scene", false);
            GLog.Sound.IsEnabled = EditorPrefs.GetBool("Game.Log.Sound", false);
            GLog.UI.IsEnabled = EditorPrefs.GetBool("Game.Log.UI", false);
            
            Menu.SetChecked("Game/Log Tool/Entity", GLog.Entity.IsEnabled);
            Menu.SetChecked("Game/Log Tool/Game", GLog.Game.IsEnabled);
            Menu.SetChecked("Game/Log Tool/Procedure", GLog.Procedure.IsEnabled);
            Menu.SetChecked("Game/Log Tool/Resource", GLog.Resource.IsEnabled);
            Menu.SetChecked("Game/Log Tool/Scene", GLog.Scene.IsEnabled);
            Menu.SetChecked("Game/Log Tool/Sound",  GLog.Sound.IsEnabled);
            Menu.SetChecked("Game/Log Tool/UI", GLog.UI.IsEnabled);
        }

        [MenuItem("Game/Log Tool/Enable All", priority = -2)]
        private static void EnableAllLog()
        {
            SetAllLog(true);
        }

        [MenuItem("Game/Log Tool/Disable All", priority = -1)]
        private static void DisableAllLog()
        {
            SetAllLog(false);
        }

        private static void SetAllLog(bool isEnabled)
        {
            EditorPrefs.SetBool("Game.Log.Entity", isEnabled);
            EditorPrefs.SetBool("Game.Log.Game", isEnabled);
            EditorPrefs.SetBool("Game.Log.Procedure", isEnabled);
            EditorPrefs.SetBool("Game.Log.Resource", isEnabled);
            EditorPrefs.SetBool("Game.Log.Scene", isEnabled);
            EditorPrefs.SetBool("Game.Log.Entity", isEnabled);
            EditorPrefs.SetBool("Game.Log.UI", isEnabled);
            
            GLog.Entity.IsEnabled = isEnabled;
            GLog.Game.IsEnabled = isEnabled;
            GLog.Procedure.IsEnabled = isEnabled;
            GLog.Resource.IsEnabled = isEnabled;
            GLog.Scene.IsEnabled = isEnabled;
            GLog.Sound.IsEnabled = isEnabled;
            GLog.UI.IsEnabled = isEnabled;
            
            Menu.SetChecked("Game/Log Tool/Entity", isEnabled);
            Menu.SetChecked("Game/Log Tool/Game", isEnabled);
            Menu.SetChecked("Game/Log Tool/Procedure", isEnabled);
            Menu.SetChecked("Game/Log Tool/Resource", isEnabled);
            Menu.SetChecked("Game/Log Tool/Scene", isEnabled);
            Menu.SetChecked("Game/Log Tool/Sound", isEnabled);
            Menu.SetChecked("Game/Log Tool/UI", isEnabled);
        }

        [MenuItem("Game/Log Tool/Entity")]
        private static void ToggleEntityLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.Entity", false);
            EditorPrefs.SetBool("Game.Log.Entity", isEnabled);
            GLog.Entity.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/Entity", isEnabled);
        }
        
        [MenuItem("Game/Log Tool/Game")]
        private static void ToggleGameLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.Game", false);
            EditorPrefs.SetBool("Game.Log.Game", isEnabled);
            GLog.Game.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/Game", isEnabled);
        }
        
        [MenuItem("Game/Log Tool/Procedure")]
        private static void ToggleProcedureLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.Procedure", false);
            EditorPrefs.SetBool("Game.Log.Procedure", isEnabled);
            GLog.Procedure.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/Procedure", isEnabled);
        }
        
        [MenuItem("Game/Log Tool/Resource")]
        private static void ToggleResourceLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.Resource", false);
            EditorPrefs.SetBool("Game.Log.Resource", isEnabled);
            GLog.Resource.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/Resource", isEnabled);
        }
        
        [MenuItem("Game/Log Tool/Scene")]
        private static void ToggleSceneLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.Scene", false);
            EditorPrefs.SetBool("Game.Log.Scene", isEnabled);
            GLog.Scene.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/Scene", isEnabled);
        }
        
        [MenuItem("Game/Log Tool/Sound")]
        private static void ToggleSoundLog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.Sound", false);
            EditorPrefs.SetBool("Game.Log.Sound", isEnabled);
            GLog.Sound.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/Sound", isEnabled);
        }
        
        [MenuItem("Game/Log Tool/UI")]
        private static void ToggleUILog()
        {
            bool isEnabled = !EditorPrefs.GetBool("Game.Log.UI", false);
            EditorPrefs.SetBool("Game.Log.UI", isEnabled);
            GLog.UI.IsEnabled = isEnabled;
            Menu.SetChecked("Game/Log Tool/UI", isEnabled);
        }
    }
}

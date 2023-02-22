using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    [CustomEditor(typeof(Init))]
    public class InitInspector : UnityEditor.Editor
    {
        private GlobalConfig globalConfig;
        
        private void OnEnable()
        {
            globalConfig = Resources.Load<GlobalConfig>("ET/GlobalConfig");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                this.globalConfig.CodeMode = (CodeMode)EditorGUILayout.EnumPopup("CodeMode: ", this.globalConfig.CodeMode);
                EditorUtility.SetDirty(this.globalConfig);
                AssetDatabase.SaveAssets();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

using Game;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace ET.Editor
{
    [CustomEditor(typeof(ETComponent))]
    internal sealed class ETComponentInspector :GameFrameworkInspector
    {
        private GlobalConfig globalConfig;
        
        private void OnEnable()
        {
            globalConfig = AssetDatabase.LoadAssetAtPath<GlobalConfig>("Assets/Res/ET/GlobalConfig.asset");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                var codeMode = (CodeMode)EditorGUILayout.EnumPopup("CodeMode: ", this.globalConfig.CodeMode);
                if (codeMode != this.globalConfig.CodeMode)
                {
                    this.globalConfig.CodeMode = codeMode;
                    EditorUtility.SetDirty(this.globalConfig);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

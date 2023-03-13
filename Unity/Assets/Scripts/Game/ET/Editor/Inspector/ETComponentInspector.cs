using Game;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace ET.Editor
{
    [CustomEditor(typeof(ETComponent))]
    internal sealed class ETComponentInspector : GameFrameworkInspector
    {
        private CodeMode codeMode;

        private void OnEnable()
        {
            this.codeMode = Define.CodeMode;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                this.codeMode = (CodeMode)EditorGUILayout.EnumPopup("CodeMode: ", this.codeMode);
                if (codeMode != Define.CodeMode)
                {
                    if (this.codeMode != Define.CodeMode)
                    {
#if UNITY_ET_CODEMODE_CLIENT
                        if (this.codeMode == CodeMode.Server)
                        {
                            CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_SERVER();
                        }
                        else if (this.codeMode == CodeMode.ClientServer)
                        {
                            CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENTSERVER();
                        }
#elif UNITY_ET_CODEMODE_SERVER
                        if (this.codeMode == CodeMode.Client)
                        {
                            CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENT();
                        }
                        else if (this.codeMode == CodeMode.ClientServer)
                        {
                            CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENTSERVER();
                        }
#elif UNITY_ET_CODEMODE_CLIENTSERVER
                        if (this.codeMode == CodeMode.Client)
                        {
                            CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_CLIENT();
                        }
                        else if (this.codeMode == CodeMode.Server)
                        {
                            CodeModeDefineSymbolTool.UNITY_ET_CODEMODE_SERVER();
                        }
#endif
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
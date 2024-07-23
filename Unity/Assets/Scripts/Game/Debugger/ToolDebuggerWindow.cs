using GameFramework.Debugger;
using SRDebugger.Internal;
using UnityEngine;

namespace Game
{
    public class ToolDebuggerWindow : IDebuggerWindow
    {
        public void Initialize(params object[] args)
        {
#if !DISABLE_SRDEBUGGER
            if (!SRDebug.IsInitialized)
            {
                SRDebug.Init();
            }

            Service.Panel.VisibilityChanged += (_, isVisible) =>
            {
                GameEntry.Debugger.ActiveWindow = !isVisible;
            };
#endif
        }

        public void Shutdown()
        {
            
        }

        public void OnEnter()
        {
            
        }

        public void OnLeave()
        {
            
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void OnDraw()
        {
#if !DISABLE_SRDEBUGGER
            if (GUILayout.Button("Show SRDebugger", GUILayout.Width(150f)))
            {
                SRDebug.Instance.ShowDebugPanel();
            }
            if (GUILayout.Button("Show RuntimeInspector", GUILayout.Width(150f)))
            {
                GameEntry.UI.OpenUIForm("Assets/Res/UI/RuntimeInspector/RuntimeInspectorForm.prefab", "RuntimeInspector");
            }
#endif
        }
    }
}

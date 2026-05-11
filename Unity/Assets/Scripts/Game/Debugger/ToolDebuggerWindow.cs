using GameFramework.Debugger;
using UnityEngine;

namespace Game
{
    public class ToolDebuggerWindow : IDebuggerWindow
    {
        public void Initialize(params object[] args)
        {
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
            if (GUILayout.Button("Show RuntimeInspector", GUILayout.Width(150f)))
            {
                GameEntry.UI.OpenUIForm("Assets/Res/UI/RuntimeInspector/RuntimeInspectorForm.prefab", "RuntimeInspector");
            }
        }
    }
}

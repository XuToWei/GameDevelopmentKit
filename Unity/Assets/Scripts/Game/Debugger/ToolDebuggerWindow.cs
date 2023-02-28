using GameFramework.Debugger;
using UnityEngine;

namespace Game
{
    public class ToolDebuggerWindow : IDebuggerWindow
    {
        public void Initialize(params object[] args)
        {
#if !DISABLE_SRDEBUGGER
            SRDebug.Init();
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
            GUILayout.BeginHorizontal("box");
            {
                
            }
            GUILayout.EndHorizontal();
        }
    }
}

using GameFramework.Debugger;
using RemoteExecution;
using UnityEngine;

namespace Game
{
    public class ToolDebuggerWindow : IDebuggerWindow
    {
        /// <summary>
        /// 连接随调试工具持有，切换页签不关闭连接，调试器 Shutdown 时释放。
        /// </summary>
        private readonly RemoteExecutionPlayerConnectionUI m_RemoteExecutionUI = new RemoteExecutionPlayerConnectionUI();

        public void Initialize(params object[] args)
        {
        }

        public void Shutdown()
        {
            RemoteExecutionPlayerApi.Stop();
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

            GUILayout.Space(8f);
            GUILayout.BeginVertical("box");
            try
            {
                GUILayout.Label("Remote Execution");
                m_RemoteExecutionUI.DrawContents();
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }
    }
}

using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// 扩展的关闭辅助器。
    /// </summary>
    public class ExtensionShutdownHelper : DefaultShutdownHelper
    {
        public override void Shutdown()
        {
            base.Shutdown();
            WebSocketComponent webSocketComponent = GameEntry.GetComponent<WebSocketComponent>();
            if (webSocketComponent != null)
            {
                webSocketComponent.Shutdown();
            }
            CodeRunnerComponent codeRunnerComponent = GameEntry.GetComponent<CodeRunnerComponent>();
            if(codeRunnerComponent != null)
            {
                GameEntry.GetComponent<CodeRunnerComponent>().Shutdown();
            }
        }
    }
}

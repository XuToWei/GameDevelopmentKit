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
            CodeRunnerComponent codeRunnerComponent = GameEntry.GetComponent<CodeRunnerComponent>();
            if(codeRunnerComponent != null && codeRunnerComponent.IsRunning)
            {
                GameEntry.GetComponent<CodeRunnerComponent>().StopRun();
            }
            base.Shutdown();
        }
    }
}

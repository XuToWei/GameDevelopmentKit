using UnityGameFramework.Extension;

namespace Game
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static CodeRunnerComponent CodeRunner
        {
            get;
            private set;
        }

        public static PlatformComponent Platform
        {
            get;
            private set;
        }

        public static TablesComponent Tables
        {
            get;
            private set;
        }

        private void InitGameComponents()
        {
            CodeRunner = UnityGameFramework.Runtime.GameEntry.GetComponent<CodeRunnerComponent>();
            Platform = UnityGameFramework.Runtime.GameEntry.GetComponent<PlatformComponent>();
            Tables = UnityGameFramework.Runtime.GameEntry.GetComponent<TablesComponent>();
        }
    }
}

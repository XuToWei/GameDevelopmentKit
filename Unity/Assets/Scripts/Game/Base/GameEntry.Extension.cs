using UnityGameFramework.Extension;

namespace Game
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static AssetSetComponent AssetSet
        {
            get;
            private set;
        }

        public static CodeRunnerComponent CodeRunner
        {
            get;
            private set;
        }

        public static ScreenComponent Screen
        {
            get;
            private set;
        }

        private static void InitExtensionComponents()
        {
            AssetSet = UnityGameFramework.Runtime.GameEntry.GetComponent<AssetSetComponent>();
            CodeRunner = UnityGameFramework.Runtime.GameEntry.GetComponent<CodeRunnerComponent>();
            Screen = UnityGameFramework.Runtime.GameEntry.GetComponent<ScreenComponent>();
        }
    }
}

namespace Game
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static BuiltinComponent Builtin
        {
            get;
            private set;
        }

        public static CameraComponent Camera
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
            Builtin = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinComponent>();
            Camera = UnityGameFramework.Runtime.GameEntry.GetComponent<CameraComponent>();
            Platform = UnityGameFramework.Runtime.GameEntry.GetComponent<PlatformComponent>();
            Tables = UnityGameFramework.Runtime.GameEntry.GetComponent<TablesComponent>();
        }
    }
}

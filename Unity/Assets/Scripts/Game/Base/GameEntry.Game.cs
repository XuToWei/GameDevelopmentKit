namespace Game
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static ETComponent ET
        {
            get;
            private set;
        }
        
        public static Tables Tables
        {
            get;
            private set;
        }

        private void InitGameComponents()
        {
            ET = UnityGameFramework.Runtime.GameEntry.GetComponent<ETComponent>();
            Tables = UnityGameFramework.Runtime.GameEntry.GetComponent<Tables>();
        }
    }
}

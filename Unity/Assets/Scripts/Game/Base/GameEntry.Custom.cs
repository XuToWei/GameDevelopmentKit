using Game;
using UnityEngine;
using UnityGameFramework.Extension;

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

        public static ETRunnerComponent ETRunner
        {
            get;
            private set;
        }

        public static SpriteCollectionComponent SpriteCollection
        {
            get;
            private set;
        }

        public static Tables Tables
        {
            get;
            private set;
        }
        
        public static TextureSetComponent TextureSet
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            Builtin = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinComponent>();
            ETRunner = UnityGameFramework.Runtime.GameEntry.GetComponent<ETRunnerComponent>();
            SpriteCollection = UnityGameFramework.Runtime.GameEntry.GetComponent<SpriteCollectionComponent>();
            Tables = UnityGameFramework.Runtime.GameEntry.GetComponent<Tables>();
            TextureSet = UnityGameFramework.Runtime.GameEntry.GetComponent<TextureSetComponent>();
        }
    }
}

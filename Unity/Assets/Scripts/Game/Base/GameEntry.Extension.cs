using UnityGameFramework.Extension;

namespace Game
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static BuiltinDataComponent BuiltinData
        {
            get;
            private set;
        }

        public static CameraComponent Camera
        {
            get;
            private set;
        }

        public static ScreenComponent Screen
        {
            get;
            private set;
        }

        public static SpriteCollectionComponent SpriteCollection
        {
            get;
            private set;
        }

        public static TextureSetComponent TextureSet
        {
            get;
            private set;
        }

        private static void InitExtensionComponents()
        {
            BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
            Camera = UnityGameFramework.Runtime.GameEntry.GetComponent<CameraComponent>();
            Screen = UnityGameFramework.Runtime.GameEntry.GetComponent<ScreenComponent>();
            SpriteCollection = UnityGameFramework.Runtime.GameEntry.GetComponent<SpriteCollectionComponent>();
            TextureSet = UnityGameFramework.Runtime.GameEntry.GetComponent<TextureSetComponent>();
        }
    }
}

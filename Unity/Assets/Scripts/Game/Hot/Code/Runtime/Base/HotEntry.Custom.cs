namespace Game.Hot
{
    public static partial class HotEntry
    {
        public static HPBarComponent HPBar { get; private set; }
        
        private static void InitCustom()
        {
            HPBar = CreateModule<HPBarComponent>();
        }
    }
}

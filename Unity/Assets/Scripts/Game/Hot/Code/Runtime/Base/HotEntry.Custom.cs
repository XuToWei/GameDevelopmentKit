namespace Game.Hot
{
    public static partial class HotEntry
    {
        public static HPBarManager HPBar { get; private set; }
        
        private static void InitCustom()
        {
            HPBar = ModuleHelper.CreateModule<HPBarManager>();
        }
    }
}

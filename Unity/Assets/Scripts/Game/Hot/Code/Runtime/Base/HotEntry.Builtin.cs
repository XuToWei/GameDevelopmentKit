namespace Game.Hot
{
    public static partial class HotEntry
    {
        public static ProcedureManager Procedure { get; private set; }
        
        private static void InitBuiltin()
        {
            Procedure = ModuleHelper.CreateModule<ProcedureManager>();
        }
    }
}

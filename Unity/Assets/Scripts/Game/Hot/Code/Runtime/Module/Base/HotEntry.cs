using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public static partial class HotEntry
    {
        public static void Start()
        {
            Log.Info("Game.Hot.Code Start!");
            InitManagers();
            GameHot.UpdateEvent += Update;
            GameHot.OnShutdownEvent += Shutdown;
            
            Procedure.StartProcedure<ProcedureLaunch>();
        }
        
        private static void InitManagers()
        {
            Procedure = CreateModule<ProcedureManager>();
        }
    }
}
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public static partial class HotEntry
    {
        public static void Start()
        {
            Log.Info("Game.Hot.Code Start!");
            
            InitBuiltin();
            InitCustom();
            
            GameHot.UpdateEvent += Update;
            GameHot.OnShutdownEvent += Shutdown;
            
            Procedure.StartProcedure<ProcedureLaunch>();
        }
    }
}
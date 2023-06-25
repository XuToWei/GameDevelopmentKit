using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public static partial class HotEntry
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <returns></returns>
        public static void Start()
        {
            Log.Info("Game.Hot.Code Start!");
            
            GameHot.UpdateEvent += ModuleHelper.Update;
            GameHot.OnShutdownEvent += ModuleHelper.Shutdown;
            
            InitBuiltin();
            InitCustom();
            
            Procedure.StartProcedure<ProcedureLaunch>();
        }
    }
}
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public sealed class HotEntry : MonoBehaviour
    {
        /// <summary>
        /// 程序入口
        /// </summary>
        /// <returns></returns>
        private void Start()
        {
            Log.Info("Game.Hot.Code Start!");
            
            InitComponents();
            
            HotComponentEntry.Initialize();
        }

        private void Update()
        {
            HotComponentEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            HotComponentEntry.Shutdown();
        }

        public static ProcedureComponent Procedure { get; private set; }
        public static HPBarComponent HPBar { get; private set; }
        
        private void InitComponents()
        {
            Procedure = HotComponentEntry.GetComponent<ProcedureComponent>();
            HPBar = HotComponentEntry.GetComponent<HPBarComponent>();
        }
    }
}
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
        public static TablesComponent Tables { get; private set; }

        #region Custom Components
        public static HPBarComponent HPBar { get; private set; }
        #endregion

        
        private void InitComponents()
        {
            Procedure = HotComponentEntry.GetComponent<ProcedureComponent>();
            Tables = HotComponentEntry.GetComponent<TablesComponent>();

            #region Custom Components
            HPBar = HotComponentEntry.GetComponent<HPBarComponent>();
            #endregion
        }
    }
}
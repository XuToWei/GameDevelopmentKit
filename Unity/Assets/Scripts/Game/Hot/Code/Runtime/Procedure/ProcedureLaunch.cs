using GameFramework.Fsm;
using GameFramework.Localization;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public sealed class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            // 变体配置：根据使用的语言，通知底层加载对应的资源变体
            InitCurrentVariant();
        }

        protected override void OnUpdate(IFsm<ProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            ChangeState<ProcedurePreload>(procedureOwner);
        }
        
        private void InitCurrentVariant()
        {
            if (GameEntry.Base.EditorResourceMode)
            {
                // 编辑器资源模式不使用 AssetBundle，也就没有变体了
                return;
            }

            string currentVariant = null;
            switch (GameEntry.Localization.Language)
            {
                case Language.English:
                    currentVariant = "en-us";
                    break;

                case Language.ChineseSimplified:
                    currentVariant = "zh-cn";
                    break;

                case Language.ChineseTraditional:
                    currentVariant = "zh-tw";
                    break;

                case Language.Korean:
                    currentVariant = "ko-kr";
                    break;

                default:
                    currentVariant = "zh-cn";
                    break;
            }

            GameEntry.Resource.SetCurrentVariant(currentVariant);
            Log.Info("Init current variant complete.");
        }
    }
}

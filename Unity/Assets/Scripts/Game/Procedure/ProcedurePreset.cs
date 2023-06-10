using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedurePreset : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            void PlayButtonClickSound()
            {
                GameEntry.Sound.PlayUISound(1000);
            }
            //按钮添加点击音效
            ExButton.AllButtonOnPointerDownEvent -= PlayButtonClickSound;
            ExButton.AllButtonOnPointerDownEvent += PlayButtonClickSound;
            
#if UNITY_ET
            ChangeState<ProcedureET>(procedureOwner);
#elif UNITY_GAMEHOT
            ChangeState<ProcedureGameHot>(procedureOwner);
#else
            ChangeState<ProcedureGame>(procedureOwner);
#endif
        }
    }
}

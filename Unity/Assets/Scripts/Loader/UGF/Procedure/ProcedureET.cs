using System;
using ET;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace UGF
{
    public class ProcedureET : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            UnityGameFramework.Runtime.Log.Debug("Enter ET procedure!");
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            
            Game.Update();
            Game.LateUpdate();
            Game.FrameFinishUpdate();
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            if (isShutdown)//流程结束，退出游戏
            {
                Game.Close();
            }
            else//除非退出游戏，不能离开ET流程（终流程）
            {
                throw new Exception("Can't leave ProcedureET!");
            }
            
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}
using GameFramework.Fsm;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<Game.Hot.ProcedureComponent>;

namespace Game.Hot
{
    public sealed class ProcedureGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureComponent> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.NetworkService.InitServiceNetworkHelper(new NetworkServiceHelper());
            GameEntry.NetworkService.Connect();
            Log.Debug("开始 GameHot！");
        }

        protected override void OnUpdate(IFsm<ProcedureComponent> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            procedureOwner.SetData<VarInt32>("NextSceneId", HotEntry.Tables.DTOneConfig.SceneMenu);
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            GameEntry.NetworkService.DestroyServiceNetworkHelper();
            base.OnDestroy(procedureOwner);
        }
    }
}

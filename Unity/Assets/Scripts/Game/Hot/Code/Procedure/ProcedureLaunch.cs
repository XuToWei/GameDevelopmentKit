using System;
using GameFramework;
using GameFramework.Fsm;
using ProtoBuf;

namespace Game.Hot
{
    public sealed class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureComponent> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            // 注册ProtoBuf自定义生成器
            ProtoActivator.RegisterCustomFactory((type, nonPublic) =>
            {
                if (type.IsClass)
                {
                    return ReferencePool.Acquire(type);
                }
                return Activator.CreateInstance(type, nonPublic);
            });
        }

        protected override void OnUpdate(IFsm<ProcedureComponent> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }
}

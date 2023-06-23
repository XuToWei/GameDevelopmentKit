using System;
using System.Reflection;
using GameFramework;
using GameFramework.Fsm;
using UnityGameFramework.Extension;

namespace Game.Hot
{
    public sealed class ProcedureManager : GameHotModule
    {
        private IFsmManager m_FsmManager;
        private IFsm<ProcedureManager> m_ProcedureFsm;
        
        public ProcedureManager()
        {
            m_FsmManager = null;
            m_ProcedureFsm = null;
        }

        protected internal override void Initialize()
        {
            var fsmManager = GameFrameworkEntry.GetModule<IFsmManager>();
            if (fsmManager == null)
            {
                throw new GameFrameworkException("FSM manager is invalid.");
            }
            Type procedureBaseType = typeof(ProcedureBase);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            using UGFList<ProcedureBase> procedures = UGFList<ProcedureBase>.Create();
            for (int i = 0; i < types.Length; i++)
            {
                if (!types[i].IsClass || types[i].IsAbstract)
                {
                    continue;
                }
                if (types[i].BaseType == procedureBaseType)
                {
                    procedures.Add((ProcedureBase)Activator.CreateInstance(types[i]));
                }
            }
            m_FsmManager = fsmManager;
            m_ProcedureFsm = m_FsmManager.CreateFsm(this, procedures.ToArray());
        }

        protected internal override int Priority => -2;

        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    throw new GameFrameworkException("You must initialize procedure first.");
                }

                return (ProcedureBase)m_ProcedureFsm.CurrentState;
            }
        }
        
        public float CurrentProcedureTime
        {
            get
            {
                if (m_ProcedureFsm == null)
                {
                    throw new GameFrameworkException("You must initialize procedure first.");
                }

                return m_ProcedureFsm.CurrentStateTime;
            }
        }
        
        protected internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        
        protected internal override void Shutdown()
        {
            if (m_FsmManager != null)
            {
                if (m_ProcedureFsm != null)
                {
                    m_FsmManager.DestroyFsm(m_ProcedureFsm);
                    m_ProcedureFsm = null;
                }

                m_FsmManager = null;
            }
        }

        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                throw new GameFrameworkException("You must initialize procedure first.");
            }

            m_ProcedureFsm.Start<T>();
        }
        
        public void StartProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                throw new GameFrameworkException("You must initialize procedure first.");
            }

            m_ProcedureFsm.Start(procedureType);
        }
        
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                throw new GameFrameworkException("You must initialize procedure first.");
            }

            return m_ProcedureFsm.HasState<T>();
        }
        
        public bool HasProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                throw new GameFrameworkException("You must initialize procedure first.");
            }

            return m_ProcedureFsm.HasState(procedureType);
        }
        
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                throw new GameFrameworkException("You must initialize procedure first.");
            }

            return m_ProcedureFsm.GetState<T>();
        }
        
        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                throw new GameFrameworkException("You must initialize procedure first.");
            }

            return (ProcedureBase)m_ProcedureFsm.GetState(procedureType);
        }
    }
}

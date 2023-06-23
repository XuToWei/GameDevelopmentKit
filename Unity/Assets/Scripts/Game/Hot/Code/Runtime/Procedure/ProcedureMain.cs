//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using GameFramework.Fsm;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public class ProcedureMain : ProcedureBase
    {
        private const float GameOverDelayedSeconds = 2f;

        private readonly Dictionary<GameMode, GameBase> m_Games = new Dictionary<GameMode, GameBase>();
        private GameBase m_CurrentGame = null;
        private bool m_GotoMenu = false;
        private float m_GotoMenuDelaySeconds = 0f;

        public void GotoMenu()
        {
            m_GotoMenu = true;
        }

        protected override void OnInit(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);

            m_Games.Add(GameMode.Survival, new SurvivalGame());
        }

        protected override void OnDestroy(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnDestroy(procedureOwner);

            m_Games.Clear();
        }

        protected override void OnEnter(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_GotoMenu = false;
            GameMode gameMode = (GameMode)procedureOwner.GetData<VarByte>("GameMode").Value;
            m_CurrentGame = m_Games[gameMode];
            m_CurrentGame.Initialize();
        }

        protected override void OnLeave(IFsm<ProcedureManager> procedureOwner, bool isShutdown)
        {
            if (m_CurrentGame != null)
            {
                m_CurrentGame.Shutdown();
                m_CurrentGame = null;
            }

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(IFsm<ProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_CurrentGame != null && !m_CurrentGame.GameOver)
            {
                m_CurrentGame.Update(elapseSeconds, realElapseSeconds);
                return;
            }

            if (!m_GotoMenu)
            {
                m_GotoMenu = true;
                m_GotoMenuDelaySeconds = 0;
            }

            m_GotoMenuDelaySeconds += elapseSeconds;
            if (m_GotoMenuDelaySeconds >= GameOverDelayedSeconds)
            {
                procedureOwner.SetData<VarInt32>("NextSceneId", Tables.Instance.DTOneConfig.SceneMenu);
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }
    }
}

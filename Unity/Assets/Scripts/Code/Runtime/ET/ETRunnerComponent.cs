using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class ETRunnerComponent : GameFrameworkComponent
    {
        public GameObject RunnerObj
        {
            get;
            private set;
        }

        private Action m_StartRunAction;

        protected override void Awake()
        {
            base.Awake();
            this.RunnerObj = this.gameObject;
        }

        public void SetRunAction(Action action)
        {
            this.m_StartRunAction = action;
        }

        public void StartRun()
        {
            this.m_StartRunAction.Invoke();
        }
    }
}

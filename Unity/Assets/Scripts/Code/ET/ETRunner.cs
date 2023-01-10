using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class ETRunner : GameFrameworkComponent
    {
        public GameObject RunnerObj => this.gameObject;

        private Action m_StartRunAction;

        protected override void Awake()
        {
            base.Awake();
            this.gameObject.name = "ET";//提供给编辑器ETView使用
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

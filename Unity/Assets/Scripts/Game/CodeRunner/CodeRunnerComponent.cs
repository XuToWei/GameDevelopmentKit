using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class CodeRunnerComponent : GameFrameworkComponent
    {
        [ShowInInspector, ReadOnly]
        public bool IsRunning { get; private set; } = false;

        private Component m_InitComponent;
        
        public void StartRun(string startMonoType)
        {
            Type initType = Utility.Assembly.GetType(startMonoType);
            if (initType == null)
            {
                throw new GameFrameworkException($"Not Found {startMonoType}!");
            }
            this.m_InitComponent = gameObject.AddComponent(initType);
            if (this.m_InitComponent == null)
            {
                throw new GameFrameworkException($"Add {initType} Fail!");
            }
            IsRunning = true;
        }

        public void Shutdown()
        {
            IsRunning = false;
            DestroyImmediate(this.m_InitComponent);
        }
    }
}

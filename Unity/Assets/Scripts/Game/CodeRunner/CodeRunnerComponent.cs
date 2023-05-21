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

        [ShowInInspector, ReadOnly]
        private Component m_InitComponent;
        
        public void StartRun(string startMonoType)
        {
            if (IsRunning)
            {
                throw new GameFrameworkException("CodeRunnerComponent StartRun duplicate!");
            }
            Type initType = Utility.Assembly.GetType(startMonoType);
            if (initType == null)
            {
                throw new GameFrameworkException($"Not Found {startMonoType}!");
            }
            m_InitComponent = gameObject.AddComponent(initType);
            if (m_InitComponent == null)
            {
                throw new GameFrameworkException($"Add {initType} Fail!");
            }
            IsRunning = true;
        }

        public void Shutdown()
        {
            if (!IsRunning)
            {
                throw new GameFrameworkException("CodeRunnerComponent can Shutdown only when is running!");
            }
            IsRunning = false;
            DestroyImmediate(this.m_InitComponent);
        }
    }
}

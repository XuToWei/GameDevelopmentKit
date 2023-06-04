using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class CodeRunnerComponent : GameFrameworkComponent
    {
        [Tooltip("编辑器模式下能否加载bytes方式运行代码")]
        [SerializeField]
        private bool m_EditorCodeBytesMode = false;

        [ShowInInspector, ReadOnly]
        public bool IsRunning { get; private set; } = false;

        [ShowInInspector, ReadOnly]
        private Component m_InitComponent;

        public bool EditorCodeBytesMode => m_EditorCodeBytesMode;

        public void StartRun(string startMonoType)
        {
            if (IsRunning)
            {
                throw new GameFrameworkException("CodeRunnerComponent StartRun duplicate!");
            }
            if (!Application.isEditor)
            {
                m_EditorCodeBytesMode = false;
            }
            Type initType = Utility.Assembly.GetType(startMonoType);
            if (initType == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Not Found {0}!", startMonoType));
            }
            m_InitComponent = gameObject.AddComponent(initType);
            if (m_InitComponent == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Add {0} Fail!", initType));
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

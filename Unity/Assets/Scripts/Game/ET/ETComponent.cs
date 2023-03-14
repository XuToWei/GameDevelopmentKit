#if UNITY_ET
using System;
using GameFramework;
#endif

using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class ETComponent : GameFrameworkComponent
    {
#if UNITY_ET
        [ShowInInspector]
        public bool IsOpen = true;

        private Component m_InitComponent;
        
        public void StartRun()
        {
            Type initType = Utility.Assembly.GetType("ET.Init");
            if (initType == null)
            {
                throw new GameFrameworkException("Not Found ET.Init(Game.ET.Loader)!");
            }
            this.m_InitComponent = gameObject.AddComponent(initType);
            if (this.m_InitComponent == null)
            {
                throw new GameFrameworkException("Add ET.Init(Game.ET.Loader) Fail!");
            }
        }

        public void ShutDown()
        {
            DestroyImmediate(this.m_InitComponent);
        }
#else
        public bool IsOpen = false;
#endif
    }
}

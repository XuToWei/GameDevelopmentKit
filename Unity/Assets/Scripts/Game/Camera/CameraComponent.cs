using System.Collections;
using GameFramework.Event;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public sealed class CameraComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Camera m_UICamera;

        [ShowInInspector]
        [ReadOnly]
        private Camera m_SceneCamera;

        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera => m_UICamera;
        
        /// <summary>
        /// 场景相机
        /// </summary>
        public Camera SceneCamera => m_SceneCamera;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            GameEntry.Event.Subscribe(SceneCameraEnableEventArgs.EventId, OnSceneCameraEnable);
            GameEntry.Event.Subscribe(SceneCameraDisableEventArgs.EventId, OnSceneCameraDisable);
        }

        private void OnSceneCameraEnable(object sender, GameEventArgs e)
        {
            SceneCameraEnableEventArgs eventArgs = e as SceneCameraEnableEventArgs;
            if (eventArgs == null)
            {
                return;
            }
            m_SceneCamera = eventArgs.SceneCamera;
        }

        private void OnSceneCameraDisable(object sender, GameEventArgs e)
        {
            SceneCameraDisableEventArgs eventArgs = e as SceneCameraDisableEventArgs;
            if (eventArgs == null)
            {
                return;
            }
            if (m_SceneCamera == eventArgs.SceneCamera)
            {
                m_SceneCamera = null;
            }
        }
    }
}
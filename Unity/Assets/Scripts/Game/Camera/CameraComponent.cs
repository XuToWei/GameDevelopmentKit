using System.Collections;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityGameFramework.Runtime;

namespace Game
{
    public sealed class CameraComponent : GameFrameworkComponent
    {
        [SerializeField]
        private Camera m_UICamera;

        [SerializeField]
        private Camera m_DefaultSceneCamera;

        private Camera m_CurrentSceneCamera;

        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera => m_UICamera;
        
        /// <summary>
        /// 场景相机
        /// </summary>
        public Camera CurrentSceneCamera => m_CurrentSceneCamera;

        protected override void Awake()
        {
            base.Awake();
            SetCurrentSceneCamera(m_DefaultSceneCamera);
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            GameEntry.Event.Subscribe(SceneCameraEnableEventArgs.EventId, OnSceneCameraEnable);
            GameEntry.Event.Subscribe(SceneCameraDisableEventArgs.EventId, OnSceneCameraDisable);
        }
        
        private void SetCurrentSceneCamera(Camera sceneCamera)
        {
            m_CurrentSceneCamera = sceneCamera;
            if (m_CurrentSceneCamera == m_DefaultSceneCamera)
            {
                m_DefaultSceneCamera.enabled = true;
            }
            else
            {
                m_DefaultSceneCamera.enabled = false;
                var currentCameraData = m_CurrentSceneCamera.GetUniversalAdditionalCameraData();
                if (currentCameraData.renderType != CameraRenderType.Base)
                {
                    throw new GameFrameworkException("Scene camera must be base camera.");
                }
                if(!currentCameraData.cameraStack.Contains(m_UICamera))
                {
                    currentCameraData.cameraStack.Add(m_UICamera);
                }
            }
        }

        private void OnSceneCameraEnable(object sender, GameEventArgs e)
        {
            SceneCameraEnableEventArgs eventArgs = e as SceneCameraEnableEventArgs;
            if (eventArgs == null)
            {
                return;
            }
            SetCurrentSceneCamera(eventArgs.SceneCamera);
        }

        private void OnSceneCameraDisable(object sender, GameEventArgs e)
        {
            SceneCameraDisableEventArgs eventArgs = e as SceneCameraDisableEventArgs;
            if (eventArgs == null)
            {
                return;
            }
            if (m_CurrentSceneCamera == eventArgs.SceneCamera)
            {
                SetCurrentSceneCamera(m_DefaultSceneCamera);
            }
        }
    }
}
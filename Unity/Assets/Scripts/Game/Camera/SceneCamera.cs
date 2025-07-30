using System;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    internal sealed class SceneCamera : MonoBehaviour
    {
        private Camera m_SceneCamera;
        private void Awake()
        {
            m_SceneCamera = GetComponent<Camera>();
            if (GameEntry.Event == null)
                return;
            SceneCameraEnableEventArgs eventArgs = SceneCameraEnableEventArgs.Create(m_SceneCamera);
            GameEntry.Event.Fire(this, eventArgs);
        }
        
        private void OnDestroy()
        {
            if (GameEntry.Event == null)
                return;
            SceneCameraDisableEventArgs eventArgs = SceneCameraDisableEventArgs.Create(m_SceneCamera);
            GameEntry.Event.Fire(this, eventArgs);
        }
    }
}
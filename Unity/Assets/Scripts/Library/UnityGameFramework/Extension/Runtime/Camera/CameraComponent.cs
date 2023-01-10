using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public class CameraComponent : GameFrameworkComponent
    {
        [SerializeField] private Camera m_UICamera;
        [SerializeField] private Camera m_SceneCamera;

        /// <summary>
        /// UI相机
        /// </summary>
        public Camera UICamera => m_UICamera;

        /// <summary>
        /// 场景相机
        /// </summary>
        public Camera SceneCamera => m_SceneCamera;
    }
}
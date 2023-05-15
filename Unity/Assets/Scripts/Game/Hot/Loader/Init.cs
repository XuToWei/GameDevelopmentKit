using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    [DisallowMultipleComponent]
    public class Init : MonoBehaviour
    {
        public static Init Instance
        {
            get;
            private set;
        }

        private class Runner : MonoBehaviour
        {
            private void Update()
            {
                GameHot.Update();
            }

            private void LateUpdate()
            {
                GameHot.LateUpdate();
            }

            private void OnDestroy()
            {
                GameHot.OnApplicationQuit();
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                GameHot.OnApplicationPause(pauseStatus);
            }

            private void OnApplicationFocus(bool hasFocus)
            {
                GameHot.OnApplicationFocus(hasFocus);
            }
        }

        private Runner m_RunnerComponent;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartAsync().Forget();
        }

        private void OnDestroy()
        {
            if (this.m_RunnerComponent != null)
            {
                DestroyImmediate(this.m_RunnerComponent);
            }
        }

        private async UniTaskVoid StartAsync()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };
            
            this.m_RunnerComponent = this.gameObject.AddComponent<Runner>();
        }
    }
}
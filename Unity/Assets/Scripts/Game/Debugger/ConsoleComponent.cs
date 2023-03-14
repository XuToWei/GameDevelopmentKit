using System;
using QFSW.QC;
using QFSW.QC.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game
{
    public sealed class ConsoleComponent : GameFrameworkComponent
    {
        private const string RectMagnificationKey = "Console.DynamicCanvasScaler.RectMagnification";
        private const string ZoomMagnificationKey = "Console.DynamicCanvasScaler.ZoomMagnification";
        
        [SerializeField]
        private QuantumConsole m_QuantumConsole;

        public QuantumConsole QuantumConsole => this.m_QuantumConsole;

        private DynamicCanvasScaler m_DynamicCanvasScaler;
        private BlurShaderController m_BlurShaderController;
        private RectTransform m_ContainerRect;
        private CanvasScaler m_CanvasScaler;

        private bool m_IsInit = false;

        private void Start()
        {
            this.QuantumConsole.Deactivate();
        }

        public void Init()
        {
            this.m_BlurShaderController = this.QuantumConsole.GetComponent<BlurShaderController>();
            this.m_DynamicCanvasScaler = this.QuantumConsole.GetComponent<DynamicCanvasScaler>();
            this.m_CanvasScaler = this.QuantumConsole.GetComponent<CanvasScaler>();
            Canvas canvas = this.QuantumConsole.GetComponent<Canvas>();
            canvas.sortingOrder = 32767;//Console置顶
            this.QuantumConsole.OnDeactivate += SaveConsoleRect;
            this.m_IsInit = true;
        }

        public void Refresh()
        {
            Vector2 resolution = GameEntry.Screen.UIRootCanvasScaler.referenceResolution;
            this.m_CanvasScaler.referenceResolution = resolution;
            this.m_CanvasScaler.matchWidthOrHeight = GameEntry.Screen.UIRootCanvasScaler.matchWidthOrHeight;
            this.m_BlurShaderController.referenceResolution = resolution;
            this.m_DynamicCanvasScaler.referenceResolution = resolution;
            if (GameEntry.Setting.HasSetting(RectMagnificationKey))
            {
                this.m_DynamicCanvasScaler.RectMagnification = GameEntry.Setting.GetFloat(RectMagnificationKey, 1);
            }
            if (GameEntry.Setting.HasSetting(ZoomMagnificationKey))
            {
                this.m_DynamicCanvasScaler.ZoomMagnification = GameEntry.Setting.GetFloat(ZoomMagnificationKey, 1);
            }
        }
        

        public bool IsShow => this.QuantumConsole.IsActive;

        public void Show()
        {
            this.QuantumConsole.Activate();
        }

        public void Hide()
        {
            this.QuantumConsole.Deactivate();
        }

        public void SaveConsoleRect()
        {
            if(!this.m_IsInit)
                return;
            GameEntry.Setting.SetFloat(RectMagnificationKey, this.m_DynamicCanvasScaler.RectMagnification);
            GameEntry.Setting.SetFloat(ZoomMagnificationKey, this.m_DynamicCanvasScaler.ZoomMagnification);
            GameEntry.Setting.Save();
        }
    }
}

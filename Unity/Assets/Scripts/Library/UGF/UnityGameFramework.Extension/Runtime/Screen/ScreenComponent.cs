using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    [InfoBox("目前只针对于UGui")]
    public sealed class ScreenComponent : GameFrameworkComponent
    {
        [SerializeField]
        private CanvasScaler m_UIRootCanvasScaler;
        
        [SerializeField, OnValueChanged("OnDefaultStandardSizeChange"), DisableInPlayMode]
        private int m_DefaultStandardWidth;
        
        [SerializeField, OnValueChanged("OnDefaultStandardSizeChange"), DisableInPlayMode]
        private int m_DefaultStandardHeight;

        public CanvasScaler UIRootCanvasScaler => this.m_UIRootCanvasScaler;
        /// <summary>
        /// 标准屏幕宽
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int StandardWidth { private set; get; }

        /// <summary>
        /// 标准屏幕高
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int StandardHeight { private set; get; }
        
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int Width { private set; get; }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        [ShowInInspector, ReadOnly]
        public int Height { private set; get; }

        /// <summary>
        /// 屏幕安全区域
        /// </summary>
        [ShowInInspector, ReadOnly]
        public Rect SafeArea { private set; get; }

        /// <summary>
        /// UI宽
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float UIWidth { private set; get; }
        
        /// <summary>
        /// UI高
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float UIHeight { private set; get; }
        
        /// <summary>
        /// 标准屏幕比例（高/宽）
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float StandardVerticalRatio { private set; get; }
        
        /// <summary>
        /// 标准屏幕比例（高/宽）
        /// </summary>
        [ShowInInspector, ReadOnly]
        public float StandardHorizontalRatio { private set; get; }
        
        private RectTransform m_UIRootRectTransform;
        
        protected override void Awake()
        {
            base.Awake();
            this.m_UIRootRectTransform = this.m_UIRootCanvasScaler.GetComponent<RectTransform>();
            Set(this.m_DefaultStandardWidth, this.m_DefaultStandardHeight);
        }

        public void Set(int standardWidth, int standardHeight)
        {
            this.SafeArea = Screen.safeArea;
            Log.Info(Utility.Text.Format("设置屏幕安全区域 x:{0} ,y:{1} ,width:{2} ,height:{3} .", this.SafeArea.x, this.SafeArea.y, this.SafeArea.width, this.SafeArea.height));
            this.StandardWidth = standardWidth;
            this.StandardHeight = standardHeight;
            Log.Info(Utility.Text.Format("设置屏幕标准宽高:{0} ,高:{1} .", this.StandardWidth, this.StandardHeight));
            this.m_UIRootCanvasScaler.referenceResolution = new Vector2(this.StandardWidth, this.StandardHeight);
            this.StandardVerticalRatio = 1f * this.StandardHeight / this.StandardWidth;
            this.StandardHorizontalRatio = 1f * this.StandardWidth / this.StandardHeight;
            this.Width = Screen.width;
            this.Height = Screen.height;
            Log.Info(Utility.Text.Format("屏幕宽高:{0} ,高:{1} .", this.Width, this.Height));
            float ratio = SafeArea.height / SafeArea.width;
            this.m_UIRootCanvasScaler.matchWidthOrHeight = ratio > StandardVerticalRatio ? 0 : 1;
            Canvas.ForceUpdateCanvases();
            Vector2 sizeDelta = this.m_UIRootRectTransform.sizeDelta;
            this.UIWidth = sizeDelta.x;
            this.UIHeight = sizeDelta.y;
        }

        private void OnDefaultStandardSizeChange()
        {
            this.m_UIRootCanvasScaler.referenceResolution = new Vector2(this.m_DefaultStandardWidth, this.m_DefaultStandardHeight);
        }
    }
}
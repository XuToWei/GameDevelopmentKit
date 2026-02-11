using Cysharp.Threading.Tasks;
using Game;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET
{
    [RequireComponent(typeof(RectTransform))]
    public abstract partial class ADynamicUIWidgetLoader : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private string m_UIWidgetAssetPath;

        private AUIWidget m_UIWidget;

        /// <summary>
        /// UIWidget资源路径
        /// </summary>
        public string UIWidgetAssetPath => m_UIWidgetAssetPath;

        /// <summary>
        /// 动态加载的UIWidget实例
        /// </summary>
        public AUIWidget UIWidget => m_UIWidget;

        public bool IsLoaded => m_UIWidget != null;

        private void Start()
        {
            LoadUIWidgetAsync().Forget();
        }

        private async UniTaskVoid LoadUIWidgetAsync()
        {
            GameObject go = await GameEntry.Resource.LoadAssetAsync<GameObject>(m_UIWidgetAssetPath);
            if (this == null || go == null)
            {
                GameEntry.Resource.UnloadAsset(go);
                return;
            }
            GameObject uiWidgetInstance = Instantiate(go, transform);
            AUIWidget uiWidget = uiWidgetInstance.GetComponent<AUIWidget>();
            if (uiWidget == null)
            {
                Destroy(uiWidgetInstance);
                GameEntry.Resource.UnloadAsset(go);
                throw new GameFrameworkException(Utility.Text.Format("DynamicMonoUIWidget: Loaded asset at path {0} does not contain an AUIWidget component.", m_UIWidgetAssetPath));
            }
            RectTransform uiWidgetRectTransform = uiWidgetInstance.GetComponent<RectTransform>();
            uiWidgetRectTransform.localRotation = Quaternion.identity;
            uiWidgetRectTransform.localPosition = Vector3.zero;
            uiWidgetRectTransform.localScale = Vector3.one;
            uiWidgetRectTransform.anchorMin = Vector2.zero;
            uiWidgetRectTransform.anchorMax = Vector2.one;
            uiWidgetRectTransform.pivot = new Vector2(0.5f, 0.5f);
            uiWidgetRectTransform.anchoredPosition3D = Vector3.zero;
            uiWidgetRectTransform.sizeDelta = Vector2.zero;

            m_UIWidget = uiWidget;
            OnUIWidgetLoaded(uiWidget);
        }

        protected abstract void OnUIWidgetLoaded(AUIWidget uiWidget);
    }
}

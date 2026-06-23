using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// 纵向循环列表：继承 LoopVerticalScrollRect，内置对象池与数据源（无需再额外挂组件）。
    /// 用法：设置 itemRenderer 回调与 numItems 即可。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/Ex Loop Vertical Scroll Rect")]
    public class ExLoopVerticalScrollRect : LoopVerticalScrollRect, LoopScrollPrefabSource, LoopScrollDataSource
    {
        [SerializeField]
        [OnValueChanged("OnItemTemplateChanged")]
        private GameObject m_ItemTemplate;

        private int m_NumItems;
        private readonly Stack<Transform> m_ItemPool = new Stack<Transform>();

        [IgnorePropertyDeclaration]
        public Action<int, Transform> itemRenderer { set; private get; }

        [ShowInInspector]
        [DisableInEditorMode]
        [IgnorePropertyDeclaration]
        public int numItems
        {
            set
            {
                m_NumItems = value;
                totalCount = m_NumItems;
                Refresh();
            }
            get => m_NumItems;
        }

        public void Refresh()
        {
            RefillCells();
        }

        public virtual GameObject GetObject(int index)
        {
            if (m_ItemPool.Count == 0)
            {
                return Instantiate(m_ItemTemplate);
            }
            Transform candidate = m_ItemPool.Pop();
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        public virtual void ReturnObject(Transform trans)
        {
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            m_ItemPool.Push(trans);
        }

        void LoopScrollDataSource.ProvideData(Transform trans, int idx)
        {
            if (itemRenderer != null)
            {
                itemRenderer.Invoke(idx, trans);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            prefabSource = this;
            dataSource = this;
            m_ItemPool.Push(m_ItemTemplate.transform);
            m_ItemTemplate.SetActive(false);
        }

#if UNITY_EDITOR
        [IgnoreLogMethod]
        private void OnItemTemplateChanged()
        {
            if (m_ItemTemplate != null && m_ItemTemplate.transform.parent != content)
            {
                Debug.LogError($"Item template must be a child of LoopScrollRect '{this.name}' content.");
                m_ItemTemplate = null;
            }
        }
#endif
    }
}

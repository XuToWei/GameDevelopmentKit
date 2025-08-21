using System;
using System.Collections.Generic;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LoopScrollRect))]
    public sealed class CommonLoopScrollRect : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        [SerializeField]
        private GameObject m_ItemPrefab;

        private int m_NumItems;
        private Stack<Transform> m_ItemPool = new Stack<Transform>();
        private LoopScrollRect m_LoopScrollRect;

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
                m_LoopScrollRect.totalCount = m_NumItems;
                Refresh();
            }
            get => m_NumItems;
        }

        public void Refresh()
        {
            m_LoopScrollRect.RefillCells();
        }

        public GameObject GetObject(int index)
        {
            if (m_ItemPool.Count == 0)
            {
                return Instantiate(m_ItemPrefab);
            }
            Transform candidate = m_ItemPool.Pop();
            candidate.gameObject.SetActive(true);
            return candidate.gameObject;
        }

        public void ReturnObject(Transform trans)
        {
            //trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            m_ItemPool.Push(trans);
        }

        public void ProvideData(Transform trans, int idx)
        {
            //trans.SendMessage("ScrollCellIndex", idx);
            if (itemRenderer != null)
            {
                itemRenderer.Invoke(idx, trans);
            }
        }

        private void Awake()
        {
            m_LoopScrollRect = GetComponent<LoopScrollRect>();
            m_LoopScrollRect.prefabSource = this;
            m_LoopScrollRect.dataSource = this;
        }

#if UNITY_EDITOR
        private void Start()
        {
            if (m_ItemPrefab == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("ItemPrefab is not set in CommonLoopScrollRect '{0}'.", this.name));
            }
        }
#endif
    }
}

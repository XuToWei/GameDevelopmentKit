using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LoopScrollRect))]
    public sealed class LoopScrollView : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
    {
        [SerializeField]
        private GameObject cellItem;

        private Action<Transform, int> scrollCellIndexAction;
        private LoopScrollRect loopScrollRect;
        
        private readonly Stack<Transform> pool = new Stack<Transform>();
        public GameObject GetObject(int index)
        {
            if (pool.Count == 0)
            {
                return Instantiate(cellItem);
            }
            Transform candidate = pool.Pop();
            GameObject go = candidate.gameObject;
            go.SetActive(true);
            return go;
        }

        public void ReturnObject(Transform trans)
        {
            trans.gameObject.SetActive(false);
            trans.SetParent(transform, false);
            pool.Push(trans);
        }

        public void ProvideData(Transform trans, int idx)
        {
            scrollCellIndexAction?.Invoke(trans, idx);
        }

        private void  Awake()
        {
            loopScrollRect = GetComponent<LoopScrollRect>();
            loopScrollRect.prefabSource = this;
            loopScrollRect.dataSource = this;
        }

        public void Init(int totalCount, Action<Transform, int> cellIndexAction)
        {
            scrollCellIndexAction = cellIndexAction;
            loopScrollRect.totalCount = totalCount;
            loopScrollRect.RefillCells();
        }
    }
}

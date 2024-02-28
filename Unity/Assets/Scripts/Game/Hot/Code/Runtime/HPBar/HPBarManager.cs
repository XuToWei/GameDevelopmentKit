//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.ObjectPool;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public class HPBarManager : GameHotModule
    {
        private HPBarItem m_HPBarItemTemplate;
        
        private Transform m_HPBarInstanceRoot;
        
        private int m_InstancePoolCapacity;

        private IObjectPool<HPBarItemObject> m_HPBarItemObjectPool;
        private List<HPBarItem> m_ActiveHPBarItems;
        private Canvas m_CachedCanvas;

        private GameObject m_HPBarItemAssetObj;
        private GameObject m_HPBarInstancesAssetObj;

        public async UniTask PreloadAsync()
        {
            m_HPBarItemAssetObj = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetUIPrefabAsset("HPBarItem"));
            m_HPBarItemTemplate = m_HPBarItemAssetObj.GetComponent<HPBarItem>();
            m_HPBarInstancesAssetObj = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetUIPrefabAsset("HPBarInstances"));
            m_HPBarInstanceRoot = Object.Instantiate(m_HPBarInstancesAssetObj.transform, GameEntry.CodeRunner.transform);
            m_CachedCanvas = m_HPBarInstanceRoot.GetComponent<Canvas>();
            m_InstancePoolCapacity = 16;
        }

        protected internal override void Initialize()
        {
            m_HPBarItemObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<HPBarItemObject>("HPBarItem", m_InstancePoolCapacity);
            m_ActiveHPBarItems = new List<HPBarItem>();
        }

        protected internal override void Shutdown()
        {
            GameEntry.Resource.UnloadAsset(m_HPBarItemAssetObj);
            GameEntry.Resource.UnloadAsset(m_HPBarInstancesAssetObj);
        }

        protected internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            for (int i = m_ActiveHPBarItems.Count - 1; i >= 0; i--)
            {
                HPBarItem hpBarItem = m_ActiveHPBarItems[i];
                if (hpBarItem.Refresh())
                {
                    continue;
                }

                HideHPBar(hpBarItem);
            }
        }

        public void ShowHPBar(Entity entity, float fromHPRatio, float toHPRatio)
        {
            if (entity == null)
            {
                Log.Warning("Entity is invalid.");
                return;
            }

            HPBarItem hpBarItem = GetActiveHPBarItem(entity);
            if (hpBarItem == null)
            {
                hpBarItem = CreateHPBarItem(entity);
                m_ActiveHPBarItems.Add(hpBarItem);
            }

            hpBarItem.Init(entity, m_CachedCanvas, fromHPRatio, toHPRatio);
        }

        private void HideHPBar(HPBarItem hpBarItem)
        {
            hpBarItem.Reset();
            m_ActiveHPBarItems.Remove(hpBarItem);
            m_HPBarItemObjectPool.Unspawn(hpBarItem);
        }

        private HPBarItem GetActiveHPBarItem(Entity entity)
        {
            if (entity == null)
            {
                return null;
            }

            for (int i = 0; i < m_ActiveHPBarItems.Count; i++)
            {
                if (m_ActiveHPBarItems[i].Owner == entity)
                {
                    return m_ActiveHPBarItems[i];
                }
            }

            return null;
        }

        private HPBarItem CreateHPBarItem(Entity entity)
        {
            HPBarItem hpBarItem = null;
            HPBarItemObject hpBarItemObject = m_HPBarItemObjectPool.Spawn();
            if (hpBarItemObject != null)
            {
                hpBarItem = (HPBarItem)hpBarItemObject.Target;
            }
            else
            {
                hpBarItem = Object.Instantiate(m_HPBarItemTemplate);
                Transform transform = hpBarItem.GetComponent<Transform>();
                transform.SetParent(m_HPBarInstanceRoot);
                transform.localScale = Vector3.one;
                m_HPBarItemObjectPool.Register(HPBarItemObject.Create(hpBarItem), true);
            }

            return hpBarItem;
        }
    }
}

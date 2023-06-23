//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;

namespace Game.Hot
{
    [Serializable]
    public class ThrusterData : AccessoryObjectData
    {
        [SerializeField]
        private float m_Speed = 0f;

        public ThrusterData(int entityId, int typeId, int ownerId, CampType ownerCamp)
            : base(entityId, typeId, ownerId, ownerCamp)
        {
            DRThruster drThruster = Tables.Instance.DTThruster.GetOrDefault(TypeId);
            if (drThruster == null)
            {
                return;
            }

            m_Speed = drThruster.Speed;
        }

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed
        {
            get
            {
                return m_Speed;
            }
        }
    }
}

//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hot
{
    [Serializable]
    public abstract class AircraftData : TargetableObjectData
    {
        [SerializeField]
        private ThrusterData m_ThrusterData = null;

        [SerializeField]
        private List<WeaponData> m_WeaponDatas = new List<WeaponData>();

        [SerializeField]
        private List<ArmorData> m_ArmorDatas = new List<ArmorData>();

        [SerializeField]
        private int m_MaxHP = 0;

        [SerializeField]
        private int m_Defense = 0;

        [SerializeField]
        private int m_DeadEffectId = 0;

        [SerializeField]
        private int m_DeadSoundId = 0;

        public AircraftData(int entityId, int typeId, CampType camp)
            : base(entityId, typeId, camp)
        {
            DRAircraft drAircraft = Tables.Instance.DTAircraft.GetOrDefault(TypeId);
            if (drAircraft == null)
            {
                return;
            }

            m_ThrusterData = new ThrusterData(GameEntry.Entity.GenerateSerialId(), drAircraft.ThrusterId, Id, Camp);

            for (int i = 0; i < drAircraft.WeaponIds.Count; i++)
            {
                AttachWeaponData(new WeaponData(GameEntry.Entity.GenerateSerialId(), drAircraft.WeaponIds[i], Id, Camp));
            }
            for (int i = 0; i < drAircraft.ArmorIds.Count; i++)
            {
                AttachArmorData(new ArmorData(GameEntry.Entity.GenerateSerialId(), drAircraft.ArmorIds[i], Id, Camp));
            }

            m_DeadEffectId = drAircraft.DeadEffectId;
            m_DeadSoundId = drAircraft.DeadSoundId;

            HP = m_MaxHP;
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public override int MaxHP
        {
            get
            {
                return m_MaxHP;
            }
        }

        /// <summary>
        /// 防御。
        /// </summary>
        public int Defense
        {
            get
            {
                return m_Defense;
            }
        }

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed
        {
            get
            {
                return m_ThrusterData.Speed;
            }
        }

        public int DeadEffectId
        {
            get
            {
                return m_DeadEffectId;
            }
        }

        public int DeadSoundId
        {
            get
            {
                return m_DeadSoundId;
            }
        }

        public ThrusterData GetThrusterData()
        {
            return m_ThrusterData;
        }

        public List<WeaponData> GetAllWeaponDatas()
        {
            return m_WeaponDatas;
        }

        public void AttachWeaponData(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                return;
            }

            if (m_WeaponDatas.Contains(weaponData))
            {
                return;
            }

            m_WeaponDatas.Add(weaponData);
        }

        public void DetachWeaponData(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                return;
            }

            m_WeaponDatas.Remove(weaponData);
        }

        public List<ArmorData> GetAllArmorDatas()
        {
            return m_ArmorDatas;
        }

        public void AttachArmorData(ArmorData armorData)
        {
            if (armorData == null)
            {
                return;
            }

            if (m_ArmorDatas.Contains(armorData))
            {
                return;
            }

            m_ArmorDatas.Add(armorData);
            RefreshData();
        }

        public void DetachArmorData(ArmorData armorData)
        {
            if (armorData == null)
            {
                return;
            }

            m_ArmorDatas.Remove(armorData);
            RefreshData();
        }

        private void RefreshData()
        {
            m_MaxHP = 0;
            m_Defense = 0;
            for (int i = 0; i < m_ArmorDatas.Count; i++)
            {
                m_MaxHP += m_ArmorDatas[i].MaxHP;
                m_Defense += m_ArmorDatas[i].Defense;
            }

            if (HP > m_MaxHP)
            {
                HP = m_MaxHP;
            }
        }
    }
}

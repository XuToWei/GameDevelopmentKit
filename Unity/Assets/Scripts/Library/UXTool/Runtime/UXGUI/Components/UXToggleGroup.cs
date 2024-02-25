using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/UXToggle Group", 48)]
    [DisallowMultipleComponent]
    public class UXToggleGroup : UIBehaviour
    {

    	[SerializeField] private bool m_AllowSwitchOff = false;
        public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

        private List<UXToggle> m_Toggles = new List<UXToggle>();

        //zrh 这个不要序列化进prefab
        [System.NonSerialized]
        public int selectId = -1;

        protected UXToggleGroup()
        { }

        private void ValidateToggleIsInGroup(UXToggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
        }

        public void NotifyToggleOn(UXToggle toggle)
        {
            ValidateToggleIsInGroup(toggle);

            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                m_Toggles[i].isOn = m_Toggles[i] == toggle;
            }
        }

        public void NotifyToggleOn(int id)
        {
            selectId = id;
            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
            	// Debug.Log("m_Toggles[i].id:"+m_Toggles[i].id);
            	// Debug.Log("selectId:"+selectId);
                m_Toggles[i].isOn = m_Toggles[i].id == selectId;
            }
        }



        public void UnregisterToggle(UXToggle toggle)
        {
            if (m_Toggles.Contains(toggle))
                m_Toggles.Remove(toggle);
        }

        public void RegisterToggle(UXToggle toggle)
        {
            if (!m_Toggles.Contains(toggle))
                m_Toggles.Add(toggle);
        }

        public bool AnyTogglesOn()
        {
            return m_Toggles.Find(x => x.isOn) != null;
        }

        public IEnumerable<UXToggle> ActiveToggles()
        {
            return m_Toggles.Where(x => x.isOn);
        }

        public List<UXToggle> AllEnableToggles()
        {
            //不要用linq的迭代器，不能保证顺序
            var ret = new List<UXToggle>();
            foreach (var toggle in m_Toggles)
            {
                if (toggle.IsActive())
                {
                    ret.Add(toggle);
                }
            }
            return ret;
        }
        
		// Comment: 确定当前toggle是被enable再调用这个接口
        public void SetAllTogglesOff()
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            for (var i = 0; i < m_Toggles.Count; i++)
                m_Toggles[i].isOn = false;

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}

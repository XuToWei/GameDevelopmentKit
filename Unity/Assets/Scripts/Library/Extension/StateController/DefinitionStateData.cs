using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public struct AnchorData
    {
        [HorizontalGroup]
        [SerializeField]
        [LabelText("AnchorMin")]
        [LabelWidth(70)]
        private Vector2 m_AnchorMin;
        
        [HorizontalGroup]
        [SerializeField]
        [LabelText("AnchorMax")]
        [LabelWidth(70)]
        private Vector2 m_AnchorMax;
        
        public Vector2 AnchorMin => m_AnchorMin;
        public Vector2 AnchorMax => m_AnchorMax;
    }

    [Serializable]
    public struct LocalizationData
    {
        [HorizontalGroup]
        [SerializeField]
        [LabelText("Enable")]
        private bool m_EnableLocalization;
        
        [HorizontalGroup]
        [SerializeField]
        [LabelText("Key")]
        [ValueDropdown("LocalizationAllKeys", DropdownWidth = 300)]
        private string m_LocalizationKey;

        public bool EnableLocalization => m_EnableLocalization;
        public string LocalizationKey => m_LocalizationKey;

#if UNITY_EDITOR
        private string[] LocalizationAllKeys()
        {
            return ThunderFireUITool.EditorLocalizationTool.AllKeys;
        }
#endif
    }
}
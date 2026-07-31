using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public struct LocalizationValue
    {
        [HorizontalGroup]
        [SerializeField]
        [LabelText("Enable")]
        private bool m_EnableLocalization;

        [HorizontalGroup]
        [SerializeField]
        [LabelText("Key")]
#if UNITY_EDITOR
        [ValueDropdown(nameof(EditorGetAllLocalizationKeys), DropdownWidth = 300)]
#endif
        private string m_LocalizationKey;

        public bool EnableLocalization => m_EnableLocalization;
        public string LocalizationKey => m_LocalizationKey;

#if UNITY_EDITOR
        private List<string> EditorGetAllLocalizationKeys()
        {
            return ThunderFireUITool.EditorLocalizationTool.AllKeyList;
        }
#endif
    }
}

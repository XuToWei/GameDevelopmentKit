using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateController
{
    [Serializable]
    public class AnchorData
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
}
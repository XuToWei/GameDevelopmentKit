using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace StateController
{
    [Serializable]
    public class ImageStateData : BaseStateData
    {
        [HideLabel]
        [SerializeField]
        private Sprite m_Sprite;

        public Sprite Sprite => m_Sprite;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class StateImage : BaseSelectableState<ImageStateData>
    {
        [SerializeField]
        private Image m_Image;

        internal override void OnStateChanged(ImageStateData stateData)
        {
            m_Image.sprite = stateData.Sprite;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Image = GetComponent<Image>();
        }
#endif
    }
}
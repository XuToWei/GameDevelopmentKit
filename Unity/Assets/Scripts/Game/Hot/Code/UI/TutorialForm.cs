using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hot
{
    public class TutorialForm : StarForceUIForm
    {
        [SerializeField]
        private Text m_BubbleText = null;

        [SerializeField]
        private Image m_MascotImage = null;

        [SerializeField]
        private Button m_SkipButton = null;

        private Action m_OnSkip;

        public void OnSkipButtonClick()
        {
            m_OnSkip?.Invoke();
            FadeClose();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            if (userData is TutorialFormParams tutorialParams)
            {
                if (!string.IsNullOrEmpty(tutorialParams.Message))
                {
                    m_BubbleText.text = tutorialParams.Message;
                }

                if (tutorialParams.MascotSprite != null)
                {
                    m_MascotImage.sprite = tutorialParams.MascotSprite;
                    m_MascotImage.color = Color.white;
                }

                m_OnSkip = tutorialParams.OnSkip;
            }
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_OnSkip = null;

            base.OnClose(isShutdown, userData);
        }
    }

    public class TutorialFormParams
    {
        public string Message { get; set; }
        public Sprite MascotSprite { get; set; }
        public Action OnSkip { get; set; }
    }
}

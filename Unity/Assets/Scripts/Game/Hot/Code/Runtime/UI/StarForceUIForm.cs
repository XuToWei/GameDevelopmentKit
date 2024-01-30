using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public abstract class StarForceUIForm : AHotUIForm
    {
        private const float FadeTime = 0.3f;
        
        private static Font s_MainFont = null;
        private CanvasGroup m_CanvasGroup = null;
        
        public void FadeClose()
        {
            StopAllCoroutines();

            IEnumerator CloseCo(float duration)
            {
                yield return m_CanvasGroup.FadeToAlpha(0f, duration);
                Close();
            }

            StopAllCoroutines();
            StartCoroutine(CloseCo(FadeTime));
        }

        public static void SetMainFont(Font mainFont)
        {
            if (mainFont == null)
            {
                Log.Error("Main font is invalid.");
                return;
            }

            s_MainFont = mainFont;
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            
            Text[] texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].font = s_MainFont;
                if (!string.IsNullOrEmpty(texts[i].text))
                {
                    texts[i].text = GameEntry.Localization.GetString(texts[i].text);
                }
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
        }
    }
}

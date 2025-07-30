using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public static partial class UGuiExtension
    {
        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }

        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            float time = 0f;
            float originalValue = slider.value;
            while (time < duration)
            {
                time += Time.deltaTime;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return new WaitForEndOfFrame();
            }

            slider.value = value;
        }
        
        public static void Set(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }

        public static void SetAsync(this UnityEvent unityEvent, Func<UniTask> action)
        {
            async UniTaskVoid OnClickAsync()
            {
                await action();
            }
            
            void OnClick()
            {
                OnClickAsync().Forget();
            }

            unityEvent.Set(OnClick);
        }

        public static void SetAsync(this Button button, Func<UniTask> action)
        {
            async UniTaskVoid OnClickAsync()
            {
                try
                {
                    button.interactable = false;
                    await action();
                }
                catch (Exception e)
                {
                    if (!e.IsOperationCanceledException())
                    {
                        throw new GameFrameworkException($"{button.name} click error", e);
                    }
                }
                finally
                {
                    button.interactable = true;
                }
            }

            void OnClick()
            {
                OnClickAsync().Forget();
            }

            button.onClick.Set(OnClick);
        }

        public static void Set(this Button button, UnityAction unityAction)
        {
            button.onClick.Set(unityAction);
        }

        public static void Set<T>(this UnityEvent<T> unityEvent, UnityAction<T> unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }
    }
}

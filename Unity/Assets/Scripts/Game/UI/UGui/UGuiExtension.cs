using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public static class UGuiExtension
    {
        public static void Set(this UnityEvent unityEvent, UnityAction unityAction)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }

        public static void SetAsync(this UnityEvent unityEvent, Func<UniTask> action)
        {
            async UniTask OnClickAsync()
            {
                try
                {
                    await action();
                }
                catch (Exception e)
                {
                    throw new Exception($"click error", e);
                }
            }

            unityEvent.Set(delegate
            {
                OnClickAsync().Forget();
            });
        }

        public static void SetAsync(this Button button, Func<UniTask> action)
        {
            async UniTask OnClickAsync()
            {
                try
                {
                    button.interactable = false;
                    await action();
                }
                catch (Exception e)
                {
                    throw new Exception($"{button.name} click error", e);
                }
                finally
                {
                    button.interactable = true;
                }
            }

            button.onClick.Set(delegate
            {
                OnClickAsync().Forget();
            });
        }

        public static void Set<T>(this UnityEvent<T> unityEvent, UnityAction<T> unityAction) where T : Object
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }
    }
}

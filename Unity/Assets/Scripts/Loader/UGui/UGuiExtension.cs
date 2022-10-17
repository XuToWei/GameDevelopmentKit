using System;
using ET;
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
        
        public static void Set(this Button button, Func<ETTask> action)
        {
            async ETTask OnClickAsync()
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
                OnClickAsync().Coroutine();
            });
        }

        public static void Set<T>(this UnityEvent<T> unityEvent, UnityAction<T> unityAction) where T : Object
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(unityAction);
        }
    }
}

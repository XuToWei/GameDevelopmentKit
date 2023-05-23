using System;

namespace Game.Hot
{
    public static class GameHot
    {
        public static event Action UpdateEvent;
        
        public static event Action LateUpdateEvent;

        public static event Action OnShutdownEvent;

        public static event Action<bool> OnApplicationPauseEvent;

        public static event Action<bool> OnApplicationFocusEvent; 

        internal static void Update()
        {
            UpdateEvent?.Invoke();
        }

        internal static void LateUpdate()
        {
            LateUpdateEvent?.Invoke();
        }

        internal static void OnShutdown()
        {
            OnShutdownEvent?.Invoke();
        }

        internal static void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent?.Invoke(pauseStatus);
        }

        internal static void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent?.Invoke(hasFocus);
        }
    }
}

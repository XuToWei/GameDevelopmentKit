using System;

namespace Game.Hot
{
    public static class GameHot
    {
        public static event Action UpdateEvent;
        
        public static event Action LateUpdateEvent;

        public static event Action OnApplicationQuitEvent;

        public static event Action<bool> OnApplicationPauseEvent;

        public static event Action<bool> OnApplicationFocusEvent; 

        internal static void Update()
        {
            UpdateEvent();
        }

        internal static void LateUpdate()
        {
            LateUpdateEvent();
        }

        internal static void OnApplicationQuit()
        {
            OnApplicationQuitEvent();
        }

        internal static void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent(pauseStatus);
        }

        internal static void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent(hasFocus);
        }
    }
}

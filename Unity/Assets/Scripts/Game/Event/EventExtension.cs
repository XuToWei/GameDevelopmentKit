using System;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game
{
    public static class EventExtension
    {
        public static void TryUnsubscribe(this EventComponent eventComponent, int id, EventHandler<GameEventArgs> handler)
        {
            if (eventComponent.Check(id, handler))
            {
                eventComponent.Unsubscribe(id, handler);
            }
        }
    }
}
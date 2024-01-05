using GameFramework;
using GameFramework.Event;
using System;

namespace Game
{
    public class EventSubscriber : IReference
    {
        private readonly GameFrameworkMultiDictionary<int, EventHandler<GameEventArgs>> m_EventHandlerDict = new GameFrameworkMultiDictionary<int, EventHandler<GameEventArgs>>();

        public object Owner
        {
            get;
            private set;
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (handler == null)
            {
                throw new Exception("Event handler is invalid.");
            }

            m_EventHandlerDict.Add(id, handler);
            GameEntry.Event.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (!m_EventHandlerDict.Remove(id, handler))
            {
                throw new Exception(Utility.Text.Format("Event '{0}' not exists specified handler.", id.ToString()));
            }

            GameEntry.Event.Unsubscribe(id, handler);
        }

        public void UnsubscribeAll()
        {
            if (m_EventHandlerDict == null)
                return;

            foreach (var item in m_EventHandlerDict)
            {
                foreach (var eventHandler in item.Value)
                {
                    GameEntry.Event.Unsubscribe(item.Key, eventHandler);
                }
            }

            m_EventHandlerDict.Clear();
        }

        public static EventSubscriber Create(object owner)
        {
            EventSubscriber eventSubscriber = ReferencePool.Acquire<EventSubscriber>();
            eventSubscriber.Owner = owner;
            return eventSubscriber;
        }

        public void Clear()
        {
            m_EventHandlerDict.Clear();
            Owner = null;
        }
    }
}
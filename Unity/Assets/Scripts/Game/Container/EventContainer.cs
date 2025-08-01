using GameFramework;
using GameFramework.Event;
using System;

namespace Game
{
    public sealed class EventContainer : IReference
    {
        private readonly GameFrameworkMultiDictionary<int, EventHandler<GameEventArgs>> m_EventHandlerDict = new GameFrameworkMultiDictionary<int, EventHandler<GameEventArgs>>();

        public object Owner
        {
            get;
            private set;
        }

        public static EventContainer Create(object owner)
        {
            EventContainer eventContainer = ReferencePool.Acquire<EventContainer>();
            eventContainer.Owner = owner;
            return eventContainer;
        }

        public void Clear()
        {
            m_EventHandlerDict.Clear();
            Owner = null;
        }

        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (handler == null)
            {
                throw new GameFrameworkException("Event handler is invalid.");
            }
            m_EventHandlerDict.Add(id, handler);
            GameEntry.Event.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (!m_EventHandlerDict.Remove(id, handler))
            {
                throw new GameFrameworkException(Utility.Text.Format("Event '{0}' not exists specified handler.", id.ToString()));
            }
            GameEntry.Event.Unsubscribe(id, handler);
        }

        public void TryUnsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            if (m_EventHandlerDict.Remove(id, handler))
            {
                GameEntry.Event.TryUnsubscribe(id, handler);
            }
        }

        public void UnsubscribeAll()
        {
            UnsubscribeAll(false);
        }

        public void UnsubscribeAll(bool isShutdown)
        {
            if (!isShutdown)
            {
                if (m_EventHandlerDict.Count > 0)
                {
                    foreach (var item in m_EventHandlerDict)
                    {
                        foreach (var eventHandler in item.Value)
                        {
                            GameEntry.Event.Unsubscribe(item.Key, eventHandler);
                        }
                    }
                }
            }

            m_EventHandlerDict.Clear();
        }

        public void TryUnsubscribeAll()
        {
            TryUnsubscribeAll(false);
        }

        public void TryUnsubscribeAll(bool isShutdown)
        {
            if (!isShutdown)
            {
                if (m_EventHandlerDict.Count > 0)
                {
                    foreach (var item in m_EventHandlerDict)
                    {
                        foreach (var eventHandler in item.Value)
                        {
                            GameEntry.Event.TryUnsubscribe(item.Key, eventHandler);
                        }
                    }
                }
            }

            m_EventHandlerDict.Clear();
        }
    }
}
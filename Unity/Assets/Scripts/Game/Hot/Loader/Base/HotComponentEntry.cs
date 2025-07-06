using System;
using System.Collections.Generic;
using GameFramework;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public static class HotComponentEntry
    {
        private static readonly GameFrameworkLinkedList<HotComponent> s_GameHotComponents = new GameFrameworkLinkedList<HotComponent>();

        public static void Initialize()
        {
            foreach (HotComponent module in s_GameHotComponents)
            {
                module.OnInitialize();
            }
        }
        
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (HotComponent module in s_GameHotComponents)
            {
                module.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<HotComponent> current = s_GameHotComponents.Last; current != null; current = current.Previous)
            {
                current.Value.OnShutdown();
            }

            s_GameHotComponents.Clear();
        }

        public static T GetComponent<T>() where T : HotComponent
        {
            return (T)GetComponent(typeof(T));
        }

        public static HotComponent GetComponent(Type type)
        {
            LinkedListNode<HotComponent> current = s_GameHotComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        public static HotComponent GetComponent(string typeName)
        {
            LinkedListNode<HotComponent> current = s_GameHotComponents.First;
            while (current != null)
            {
                Type type = current.Value.GetType();
                if (type.FullName == typeName || type.Name == typeName)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        public static void RegisterComponent<T>(T hotComponent) where T : HotComponent
        {
            if (hotComponent == null)
            {
                Log.Error("Hot component is invalid.");
                return;
            }

            Type type = hotComponent.GetType();

            LinkedListNode<HotComponent> current = s_GameHotComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    Log.Error("Hot component type '{0}' is already exist.", type.FullName);
                    return;
                }

                current = current.Next;
            }

            current = s_GameHotComponents.First;
            while (current != null)
            {
                if (hotComponent.Priority > current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }

            if (current != null)
            {
                s_GameHotComponents.AddBefore(current, hotComponent);
            }
            else
            {
                s_GameHotComponents.AddLast(hotComponent);
            }
        }
    }
}

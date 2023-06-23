using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Game.Hot
{
    public static partial class HotEntry
    {
        private static readonly GameFrameworkLinkedList<GameHotModule> s_GameFrameworkModules = new GameFrameworkLinkedList<GameHotModule>();
        
        public static void Update()
        {
            foreach (GameHotModule module in s_GameFrameworkModules)
            {
                module.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }
        
        public static void Shutdown()
        {
            for (LinkedListNode<GameHotModule> current = s_GameFrameworkModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            s_GameFrameworkModules.Clear();
            ReferencePool.ClearAll();
            Utility.Marshal.FreeCachedHGlobal();
            GameFrameworkLog.SetLogHelper(null);
        }

        public static T CreateModule<T>() where T : GameHotModule
        {
            Type moduleType = typeof(T);
            foreach (GameHotModule gameHotModule in s_GameFrameworkModules)
            {
                if (gameHotModule.GetType() == moduleType)
                {
                    return gameHotModule as T;
                }
            }
            
            GameHotModule module = (GameHotModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not create module '{0}'.", moduleType.FullName));
            }
            module.Initialize();

            LinkedListNode<GameHotModule> current = s_GameFrameworkModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }

            if (current != null)
            {
                s_GameFrameworkModules.AddBefore(current, module);
            }
            else
            {
                s_GameFrameworkModules.AddLast(module);
            }

            return module as T;
        }
    }
}

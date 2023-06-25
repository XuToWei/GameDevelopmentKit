using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Game.Hot
{
    public static class ModuleHelper
    {
        private static readonly GameFrameworkLinkedList<GameHotModule> s_GameHotModules = new GameFrameworkLinkedList<GameHotModule>();
        
        public static void Update()
        {
            foreach (GameHotModule module in s_GameHotModules)
            {
                module.Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }
        
        public static void Shutdown()
        {
            for (LinkedListNode<GameHotModule> current = s_GameHotModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            s_GameHotModules.Clear();
            ReferencePool.ClearAll();
            Utility.Marshal.FreeCachedHGlobal();
            GameFrameworkLog.SetLogHelper(null);
        }

        public static T CreateModule<T>() where T : GameHotModule
        {
            Type moduleType = typeof(T);
            foreach (GameHotModule gameHotModule in s_GameHotModules)
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

            LinkedListNode<GameHotModule> current = s_GameHotModules.First;
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
                s_GameHotModules.AddBefore(current, module);
            }
            else
            {
                s_GameHotModules.AddLast(module);
            }

            return module as T;
        }
    }
}

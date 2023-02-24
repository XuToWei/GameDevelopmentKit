#if UNITY_ET
using System;
using System.Reflection;
using GameFramework;
#endif

using UnityGameFramework.Runtime;

namespace Game
{
    public class ETComponent : GameFrameworkComponent
    {
#if UNITY_ET
        public void StartRun()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in assemblies)
            {
                if (ass.GetName().Name == "Game.ET.Loader")
                {
                    Type initType = ass.GetType("ET.Init");
                    gameObject.AddComponent(initType);
                    return;
                }
            }
            throw new GameFrameworkException("Not Found Game.ET.Loader or ET.Init!");
        }
#endif
    }
}

using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Reflection;
using Sirenix.OdinInspector;
using GameFramework;
#endif

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Create HybridCLRConfig", fileName = "HybridCLRConfig", order = 0)]
    [InfoBox("根据依赖关系调整加载先后顺序")]
    public class HybridCLRConfig : ScriptableObject
    {
        [SerializeField]
        public TextAsset[] aotAssemblies;

#if UNITY_EDITOR
        [Button("自动链接AotDlls")]
         public void AutoLinkAotDlls()
         {
             Type editorType = Utility.Assembly.GetType("Game.Editor.HybridCLREditor");
             MethodInfo linkMethodInfo = editorType.GetMethod("AutoLinkAotDllsToHybridCLRConfig", BindingFlags.Static | BindingFlags.Public);
             linkMethodInfo.Invoke(null, null);
         }
#endif
    }
}

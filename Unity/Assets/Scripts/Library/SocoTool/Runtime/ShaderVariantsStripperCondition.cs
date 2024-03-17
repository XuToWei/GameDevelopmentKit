using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Soco.ShaderVariantsStripper
{
#if UNITY_EDITOR
    public struct ShaderVariantsStripperConditionOnGUIContext
    {
        public Shader shader;

        
        //ShaderVariantsStripperConditionWindow window;
        internal string[] globalKeywords;
        internal string[] localKeywords;

        public void Init()
        {
            if (shader != null)
            {
                InitGetKeywordMethod();

                globalKeywords = sGetShaderGlobalKeywordsMethod.Invoke(null, new object[] { shader }) as string[];
                localKeywords = sGetShaderLocalKeywordsMethod.Invoke(null, new object[] { shader }) as string[];
            }
        }

        private static MethodInfo sGetShaderGlobalKeywordsMethod = null;
        private static MethodInfo sGetShaderLocalKeywordsMethod = null;

        private static void InitGetKeywordMethod()
        {
            if (sGetShaderGlobalKeywordsMethod == null)
                sGetShaderGlobalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);

            if (sGetShaderLocalKeywordsMethod == null)
                sGetShaderLocalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);
        }
    }
#endif
    
    public interface ShaderVariantsStripperCondition
    {
        bool Completion(Shader shader, ShaderVariantsData data);

        bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData);
        
#if UNITY_EDITOR
        string Overview();
        void OnGUI(ShaderVariantsStripperConditionOnGUIContext context);
        string GetName();
#endif
    }

}

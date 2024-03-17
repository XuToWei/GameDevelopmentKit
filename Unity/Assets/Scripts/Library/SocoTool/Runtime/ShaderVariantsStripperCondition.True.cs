using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionTrue : ShaderVariantsStripperCondition
    {
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            return true;
        }

        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            return true;
        }

#if UNITY_EDITOR
        public string Overview()
        {
            return "恒定满足条件";
        }

        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("恒定满足条件");
            
            EditorGUILayout.EndVertical();
        }

        public string GetName()
        {
            return "恒定满足条件";
        }
#endif
    }
}
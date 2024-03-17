using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionPassTypeIs : ShaderVariantsStripperCondition
    {
        public PassType passType = PassType.Normal;
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            return data.passType == passType;
        }

        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            return other.GetType() == typeof(ShaderVariantsStripperConditionPassTypeIs) &&
                   (other as ShaderVariantsStripperConditionPassTypeIs).passType == this.passType;
        }
        
#if UNITY_EDITOR
        public string Overview()
        {
            return $"当Pass类型是{passType}时";
        }

        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();

            passType = (PassType)EditorGUILayout.EnumPopup("PassType", passType);
            
            EditorGUILayout.EndVertical();
        }

        public string GetName()
        {
            return "PassType是";
        }
#endif
    }
}
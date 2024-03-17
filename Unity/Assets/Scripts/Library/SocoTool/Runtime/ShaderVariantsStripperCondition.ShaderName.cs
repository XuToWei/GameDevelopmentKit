using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionShaderName : ShaderVariantsStripperCondition
    {
        public enum MatchMode
        {
            AllMatch,
            StartsWith,
            EndsWith,
            Regex
        }

        public MatchMode mode;
        public string shaderName;
        public RegexOptions regexOptions;
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            switch (mode)
            {
                case MatchMode.AllMatch:
                    return shader.name == shaderName;
                case MatchMode.StartsWith:
                    return shader.name.StartsWith(shaderName);
                case MatchMode.EndsWith:
                    return shader.name.EndsWith(shaderName);
                case MatchMode.Regex:
                    return Regex.IsMatch(shader.name, shaderName);
            }

            return false;
        }

        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            return other.GetType() == typeof(ShaderVariantsStripperConditionShaderName) &&
                   (other as ShaderVariantsStripperConditionShaderName).mode == this.mode &&
                   (other as ShaderVariantsStripperConditionShaderName).shaderName == this.shaderName &&
                   (this.mode != MatchMode.Regex || (other as ShaderVariantsStripperConditionShaderName).regexOptions == this.regexOptions);
        }
        
#if UNITY_EDITOR
        
        public string Overview()
        {
            return $"当Shader名称或模式<{shaderName}>符合条件<{mode}>时";
        }

        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();

            mode = (MatchMode)EditorGUILayout.EnumPopup("MatchMode", mode);

            string label = mode == MatchMode.Regex ? "pattern" : "shader name";
            shaderName = EditorGUILayout.TextField(label, shaderName);
            
            if (mode == MatchMode.Regex)
                regexOptions = (RegexOptions)EditorGUILayout.EnumFlagsField("Regex Options", regexOptions);

            EditorGUILayout.EndVertical();
        }

        public string GetName()
        {
            return "Shader名称匹配";
        }
#endif
    }
}
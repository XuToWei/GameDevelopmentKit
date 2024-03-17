using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionPassName : ShaderVariantsStripperCondition
    {
        public enum MatchMode
        {
            AllMatch,
            StartsWith,
            EndsWith,
            Regex
        }

        public MatchMode mode;
        public string passName = "";
        public RegexOptions regexOptions;
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            switch (mode)
            {
                case MatchMode.AllMatch:
                    return data.passName == passName;
                case MatchMode.StartsWith:
                    return data.passName.StartsWith(passName);
                case MatchMode.EndsWith:
                    return data.passName.EndsWith(passName);
                case MatchMode.Regex:
                    return Regex.IsMatch(data.passName, passName, regexOptions);
            }

            return false;
        }

        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            return other.GetType() == typeof(ShaderVariantsStripperConditionPassName) &&
                   (other as ShaderVariantsStripperConditionPassName).mode == this.mode &&
                   (other as ShaderVariantsStripperConditionPassName).passName == this.passName &&
                   (this.mode != MatchMode.Regex || (other as ShaderVariantsStripperConditionPassName).regexOptions == this.regexOptions);
        }
        
#if UNITY_EDITOR
        
        public string Overview()
        {
            return $"当Pass名称或模式<{passName}>符合条件<{mode}>时";
        }

        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();

            mode = (MatchMode)EditorGUILayout.EnumPopup("MatchMode", mode);

            string label = mode == MatchMode.Regex ? "pattern" : "pass name";
            passName = EditorGUILayout.TextField(label, passName);

            if (mode == MatchMode.Regex)
                regexOptions = (RegexOptions)EditorGUILayout.EnumFlagsField("Regex Options", regexOptions);
            
            EditorGUILayout.EndVertical();
        }

        public string GetName()
        {
            return "Pass名称匹配";
        }
#endif
    }
}
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    public static class ResourceRuleEditorUtility
    {
        public static void RefreshResourceCollection()
        {
            ResourceRuleEditor ruleEditor = ScriptableObject.CreateInstance<ResourceRuleEditor>();
            ruleEditor.RefreshResourceCollection();
        }
        public static void RefreshResourceCollection(string configPath)
        {
            ResourceRuleEditor ruleEditor = ScriptableObject.CreateInstance<ResourceRuleEditor>();
            ruleEditor.RefreshResourceCollection(configPath);
        }
    }
}
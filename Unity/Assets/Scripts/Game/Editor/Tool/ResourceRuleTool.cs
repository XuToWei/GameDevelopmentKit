using System.Linq;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class ResourceRuleTool
    {
        private readonly static string ResourceRuleAsset_ET = "Assets/Res/Editor/Config/ResourceRuleEditor_ET.asset";
        
        private readonly static string ResourceRuleAsset_GameHot = "Assets/Res/Editor/Config/ResourceRuleEditor_GameHot.asset";
        
        public static void ActivateRule(string ruleDataAsset)
        {
            ruleDataAsset = Utility.Path.GetRegularPath(ruleDataAsset);
            var ruleData = AssetDatabase.LoadAssetAtPath<ResourceRuleEditorData>(ruleDataAsset);
            if (ruleData == null)
            {
                throw new GameFrameworkException($"{ruleDataAsset} not exist!");
            }
            ruleData.isActivate = true;
            string[] allConfigs = AssetDatabase.FindAssets("t:ResourceRuleEditorData").Select(AssetDatabase.GUIDToAssetPath).ToArray();
            foreach (var config in allConfigs)
            {
                string configPath = Utility.Path.GetRegularPath(config);
                ResourceRuleEditorData ruleEditorData = AssetDatabase.LoadAssetAtPath<ResourceRuleEditorData>(configPath);
                ruleEditorData.isActivate = ruleDataAsset == configPath;
            }
            AssetDatabase.SaveAssets();
        }

        public static void ActivateRule_ET()
        {
            ActivateRule(ResourceRuleAsset_ET);
        }
        
        public static void ActivateRule_GameHot()
        {
            ActivateRule(ResourceRuleAsset_GameHot);
        }
    }
}

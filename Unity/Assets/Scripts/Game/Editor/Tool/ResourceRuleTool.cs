using System.Linq;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Extension.Editor;

namespace Game.Editor
{
    public static class ResourceRuleTool
    {
        public static readonly string ResourceRuleAsset_ET = "Assets/Res/Editor/Config/ResourceRuleEditor_ET.asset";
        
        public static readonly string ResourceRuleAsset_GameHot = "Assets/Res/Editor/Config/ResourceRuleEditor_GameHot.asset";
        
        public static void ActivateRule(string ruleDataAsset)
        {
            ruleDataAsset = Utility.Path.GetRegularPath(ruleDataAsset);
            ResourceRuleEditorData ruleData = AssetDatabase.LoadAssetAtPath<ResourceRuleEditorData>(ruleDataAsset);
            if (ruleData == null)
            {
                throw new GameFrameworkException($"{ruleDataAsset} not exist!");
            }
            ruleData.IsActivate = true;
            string[] allConfigs = AssetDatabase.FindAssets("t:ResourceRuleEditorData").Select(AssetDatabase.GUIDToAssetPath).ToArray();
            foreach (string config in allConfigs)
            {
                string configPath = Utility.Path.GetRegularPath(config);
                ResourceRuleEditorData ruleEditorData = AssetDatabase.LoadAssetAtPath<ResourceRuleEditorData>(configPath);
                ruleEditorData.IsActivate = ruleDataAsset == configPath;
                EditorUtility.SetDirty(ruleEditorData);
            }
            AssetDatabase.SaveAssets();
            Debug.Log($"Activate Resource Rule : {ruleDataAsset} !");
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

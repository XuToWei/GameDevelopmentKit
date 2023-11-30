using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    public sealed class ResourceListEditorRuntime
    {
        public void GenerateList(ref Dictionary<string, string> namePathDict)
        {
            List<string> allConfigPaths = AssetDatabase.FindAssets("t:ResourceRuleEditorData").Select(AssetDatabase.GUIDToAssetPath).ToList();
            foreach (var configPath in allConfigPaths)
            {
                ResourceRuleEditorData ruleEditorData = AssetDatabase.LoadAssetAtPath<ResourceRuleEditorData>(configPath);
                if (ruleEditorData.isActivate)
                {
                    List<string> coveredAssetSearchPaths = new List<string>();
                    foreach (ResourceRule rule in ruleEditorData.rules)
                    {
                        if (rule.valid)
                        {
                            coveredAssetSearchPaths.Add(rule.assetsDirectoryPath.Replace("Assets/Res/", ""));
                        }
                    }
                    ResourceEditorController controller = new ResourceEditorController();
                    controller.Load(coveredAssetSearchPaths);
                    SourceAsset[] assets = controller.GetSourceAssets();
                    foreach (SourceAsset asset in assets)
                    {
                        namePathDict.Add(ResourceListGenerator.GetNewName(asset.Path), asset.Path);
                    }
                }
            }
        }
    }
}

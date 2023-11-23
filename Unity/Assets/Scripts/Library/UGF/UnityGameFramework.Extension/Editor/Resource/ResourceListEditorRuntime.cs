using System.Collections.Generic;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    public sealed class ResourceListEditorRuntime
    {
        public void GenerateList(ref Dictionary<string, string> namePathDict)
        {
            ResourceEditorController controller = new ResourceEditorController();
            controller.Load();
            controller.ScanSourceAssets();
            Resource[] resources = controller.GetResources();
            foreach (var resource in resources)
            {
                foreach (Asset asset in resource.GetAssets())
                {
                    namePathDict.Add(ResourceListGenerator.GetNewName(asset.Name), asset.Name);
                }
            }
        }
    }
}

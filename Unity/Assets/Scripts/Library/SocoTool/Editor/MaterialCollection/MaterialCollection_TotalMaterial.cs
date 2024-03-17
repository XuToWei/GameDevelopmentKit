using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    //这是一个编写示例
    //将收集目录下所有材质
    //实际项目不建议这样做
    public sealed class MaterialCollection_TotalMaterial : IMaterialCollector
    {
        public enum PathMode
        {
            Asset,
            String
        }
        //private readonly string[] cIncludePath = { "Assets", "Packages" };
        [Tooltip("路径选择模式，以文件夹资产的方式，或以字符串路径")]
        public PathMode pathMode = PathMode.String;
        public string[] mIncludePath = { "Assets", "Packages" };
        public DefaultAsset[] mFolders = new DefaultAsset[0];

        public override void AddMaterialBuildDependency(IList<Material> buildDependencyList)
        {
            string[] includePath = mFolders.Select(f => AssetDatabase.GetAssetPath(f)).ToArray();
            includePath = pathMode == PathMode.Asset ? includePath : mIncludePath;
            var materialsGUID = AssetDatabase.FindAssets("t:Material", includePath);

            foreach (Material m in materialsGUID.Select(
                         guid => AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid))
                     ))
            {
                buildDependencyList.Add(m);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    //用于收集所有打包场景依赖的材质
    public class MaterialCollection_SceneDependency : IMaterialCollector
    {
        //是否只收集在EditorBuildSettings中enable的场景
        public bool collectOnlyEnable = true;
        public override void AddMaterialBuildDependency(IList<Material> buildDependencyList)
        {
            var sceneDependencyMaterials = EditorBuildSettings.scenes
                .Where(scene => !collectOnlyEnable || scene.enabled)
                .SelectMany(scene => AssetDatabase.GetDependencies(scene.path))
                .Where(dependencyAsset => dependencyAsset.EndsWith(".mat"))
                .Distinct()
                .Select(matPath => AssetDatabase.LoadAssetAtPath<Material>(matPath));

            foreach (Material m in sceneDependencyMaterials)
                buildDependencyList.Add(m);
        }
    }
}
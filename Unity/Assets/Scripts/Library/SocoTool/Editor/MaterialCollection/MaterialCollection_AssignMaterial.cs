using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    // 这个是指定具体某些材质收集进来
    // 一般用于Debug
    public class MaterialCollection_AssignMaterial : IMaterialCollector
    {
        public Material[] materials = new Material[0];

        public override void AddMaterialBuildDependency(IList<Material> buildDependencyList)
        {
            materials.ToList().ForEach(buildDependencyList.Add);
        }
    }
}
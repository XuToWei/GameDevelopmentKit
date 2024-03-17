using System.Collections.Generic;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    public abstract class IMaterialCollector : ScriptableObject
    {
        public abstract void AddMaterialBuildDependency(IList<Material> buildDependencyList);
    }
}
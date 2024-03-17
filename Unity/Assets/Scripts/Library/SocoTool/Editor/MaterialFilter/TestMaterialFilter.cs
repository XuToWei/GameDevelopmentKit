using System.Collections.Generic;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    public sealed class TestMaterialFilter : IMaterialFilter
    {
        public override bool Filter(Material material, List<IMaterialCollector> collections)
        {
            //nothing to do
            return true;
        }
    }
}
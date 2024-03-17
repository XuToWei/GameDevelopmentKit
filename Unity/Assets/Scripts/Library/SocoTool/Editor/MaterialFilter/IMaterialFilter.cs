using System.Collections.Generic;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    public abstract class IMaterialFilter : ScriptableObject
    {
        //return true will save and false will strip
        public abstract bool Filter(Material material, List<IMaterialCollector> collections);
    }
}
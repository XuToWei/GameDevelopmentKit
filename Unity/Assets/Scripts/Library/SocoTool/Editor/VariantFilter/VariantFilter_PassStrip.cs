using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

namespace Soco.ShaderVariantsCollection
{
    public class VariantFilter_PassStrip : IVariantFilter
    {
        public PassType[] mStripPasses = new PassType[0];
        
        public override bool Filter(ShaderVariantCollection.ShaderVariant variant)
        {
            return !mStripPasses.Contains(variant.passType);
        }
    }
}
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    public abstract class IVariantFilter : ScriptableObject
    {
        //return true will save and false will strip
        public abstract bool Filter(ShaderVariantCollection.ShaderVariant variant);
    }
}
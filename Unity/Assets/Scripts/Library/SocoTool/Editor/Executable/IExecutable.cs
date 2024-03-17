using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    public abstract class IExecutable : ScriptableObject
    {
        public abstract void Execute(ShaderVariantCollectionMapper mapper);
    }
}
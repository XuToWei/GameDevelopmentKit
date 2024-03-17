using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Soco.ShaderVariantsCollection
{
    [Serializable]
    public struct SerializableShaderVariant
    {
        public Shader shader;
        public PassType passType;
        public string[] keywords;

        public SerializableShaderVariant(ShaderVariantCollection.ShaderVariant variant)
        {
            shader = variant.shader;
            passType = variant.passType;
            keywords = variant.keywords;
        }

        public ShaderVariantCollection.ShaderVariant Deserialize()
        {
            //这样初始化的原因是，假如变体无效不会报错
            return new ShaderVariantCollection.ShaderVariant()
            {
                shader = shader,
                passType = passType,
                keywords = keywords
            };
        }

        public bool IsValid()
        {
            try
            {
                new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

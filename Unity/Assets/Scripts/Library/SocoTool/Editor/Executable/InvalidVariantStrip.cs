using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    public class InvalidVariantStrip : IExecutable
    {
        public override void Execute(ShaderVariantCollectionMapper mapper)
        {
            var collection = mapper.mCollection;
            
            SerializedObject serializedObject = new UnityEditor.SerializedObject(collection);
            //serializedObject.Update();
            SerializedProperty m_Shaders = serializedObject.FindProperty("m_Shaders");
            
            for (int i = 0; i < m_Shaders.arraySize; ++i)
            {
                SerializedProperty pair = m_Shaders.GetArrayElementAtIndex(i);

                SerializedProperty first = pair.FindPropertyRelative("first");
                SerializedProperty second = pair.FindPropertyRelative("second");//ShaderInfo

                Shader shader = first.objectReferenceValue as Shader;

                if (shader == null)
                    continue;

                SerializedProperty variants = second.FindPropertyRelative("variants");
                for (var vi = variants.arraySize - 1; vi >= 0 ; --vi)
                {
                    SerializedProperty variantInfo = variants.GetArrayElementAtIndex(vi);
                    ShaderVariantCollection.ShaderVariant variant = ShaderVariantCollectionMapper.PropToVariantObject(shader, variantInfo);
                    
                    if (!IsValid(variant))
                        variants.DeleteArrayElementAtIndex(vi);
                }
            }

            serializedObject.ApplyModifiedProperties();
            mapper.Refresh();

        }

        private bool IsValid(ShaderVariantCollection.ShaderVariant variant)
        {
            try
            {
                new ShaderVariantCollection.ShaderVariant(variant.shader, variant.passType, variant.keywords);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
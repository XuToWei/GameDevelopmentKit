using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Soco.ShaderVariantsCollection
{
    //用于访问ShaderVariantCollection被隐藏的C++部分
    //继承UnityEngine.ScriptableObject, ISerializationCallbackReceiver是为了Undo
    public class ShaderVariantCollectionMapper : UnityEngine.ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        internal ShaderVariantCollection mCollection;
        private Dictionary<Shader, List<SerializableShaderVariant>> mMapper = new Dictionary<Shader, List<SerializableShaderVariant>>();

        private List<Shader> mMapperSerializeKeys = new List<Shader>();
        [System.Serializable]
        private struct ListWrapper
        {
            public List<SerializableShaderVariant> list;

            public ListWrapper(List<SerializableShaderVariant> list)
            {
                this.list = list;
            }

            // public List<ShaderVariantCollection.ShaderVariant> Deserialize()
            // {
            //     return list.Select(variant => variant.Deserialize()).ToList();
            // }
        }
        private List<ListWrapper> mMapperSerializeValues = new List<ListWrapper>();

        [NonSerialized]
        private bool mSerializedFlag = false;
        
        private static MethodInfo sAddNewShaderToCollection = null;

        private static void InitShaderUtilMethod()
        {
            if (sAddNewShaderToCollection == null)
                sAddNewShaderToCollection = typeof(ShaderUtil).GetMethod("AddNewShaderToCollection",
                    BindingFlags.NonPublic | BindingFlags.Static);
        }
        
        //当用CreateInstance时，调用这个方法
        public void Init(ShaderVariantCollection collection)
        {
            mCollection = collection;

            if (mCollection == null)
            {
                throw new Exception(
                    "ShaderVariantCollectionMapper constructor get a null ShaderVariantCollection Object");
            }

            ReadFromFile();
            InitShaderUtilMethod();
        }
        
        public ShaderVariantCollectionMapper(ShaderVariantCollection collection)
        {
            Init(collection);
        }

        internal static  ShaderVariantCollection.ShaderVariant PropToVariantObject(Shader shader, SerializedProperty variantInfo)
        {
            PassType passType = (PassType)variantInfo.FindPropertyRelative("passType").intValue;
            string keywords = variantInfo.FindPropertyRelative("keywords").stringValue;
            string[] keywordSet = keywords.Split(' ');
            keywordSet = (keywordSet.Length == 1 && keywordSet[0] == "") ? new string[0] : keywordSet;
            
            ShaderVariantCollection.ShaderVariant newVariant = new ShaderVariantCollection.ShaderVariant()
            {
                shader = shader,
                keywords = keywordSet,
                passType = passType
            };

            return newVariant;
        }
        
        private void ReadFromFile()
        {
            mMapper.Clear();
            
            SerializedObject serializedObject = new UnityEditor.SerializedObject(mCollection);
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
                    
                mMapper[shader] = new List<SerializableShaderVariant>();

                SerializedProperty variants = second.FindPropertyRelative("variants");
                for (var vi = 0; vi < variants.arraySize; ++vi)
                {
                    SerializedProperty variantInfo = variants.GetArrayElementAtIndex(vi);

                    ShaderVariantCollection.ShaderVariant variant = PropToVariantObject(shader, variantInfo);
                    mMapper[shader].Add(new SerializableShaderVariant(variant));
                }
            }
        }

        public Dictionary<Shader, List<SerializableShaderVariant>>.KeyCollection shaders => mMapper.Keys;

        public ReadOnlyCollection<ShaderVariantCollection.ShaderVariant> GetShaderVariants(Shader shader) =>
            mMapper[shader].Select(v=>v.Deserialize()).ToList().AsReadOnly();

        public bool HasShader(Shader shader)
        {
            bool collectionHasShader = false;
            SerializedObject serializedObject = new UnityEditor.SerializedObject(mCollection);
            //serializedObject.Update();
            SerializedProperty m_Shaders = serializedObject.FindProperty("m_Shaders");

            for (int i = 0; i < m_Shaders.arraySize; ++i)
            {
                SerializedProperty pair = m_Shaders.GetArrayElementAtIndex(i);

                SerializedProperty first = pair.FindPropertyRelative("first");
                SerializedProperty second = pair.FindPropertyRelative("second"); //ShaderInfo

                Shader currentShader = first.objectReferenceValue as Shader;

                if (currentShader == shader)
                {
                    collectionHasShader = true;
                    break;
                }
            }

            bool mapperHasShader = mMapper.ContainsKey(shader);

            Debug.Assert(collectionHasShader == mapperHasShader, "map对象与源文件内容不符");
            return mapperHasShader;
        }
        
        public void AddShader(Shader shader)
        {
            if (!mMapper.ContainsKey(shader))
            {
                mMapper.Add(shader, new List<SerializableShaderVariant>());
            }

            sAddNewShaderToCollection.Invoke(null, new object[] { shader, mCollection });
        }

        public void RemoveShader(Shader shader)
        {
            if (mMapper.ContainsKey(shader))
            {
                mMapper.Remove(shader);
            }

            SerializedObject serializedObject = new UnityEditor.SerializedObject(mCollection);
            //serializedObject.Update();
            SerializedProperty m_Shaders = serializedObject.FindProperty("m_Shaders");

            for (int shaderIndex = 0; shaderIndex < m_Shaders.arraySize; ++shaderIndex)
            {
                SerializedProperty pair = m_Shaders.GetArrayElementAtIndex(shaderIndex);

                SerializedProperty first = pair.FindPropertyRelative("first");

                Shader currentShader = first.objectReferenceValue as Shader;

                if (currentShader == shader)
                {
                    m_Shaders.DeleteArrayElementAtIndex(shaderIndex);
                    break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool VariantCompare(SerializableShaderVariant l, SerializableShaderVariant r)
        {
            if (l.shader != r.shader)
                return false;

            if (l.passType != r.passType)
                return false;

            if (l.keywords.Length != r.keywords.Length)
                return false;

            return l.keywords.OrderBy(s => s).SequenceEqual(r.keywords.OrderBy(t => t));
        }
        
        public bool HasVariant(ShaderVariantCollection.ShaderVariant variant)
        {
            bool collectionHasVariant = mCollection.Contains(variant);

            bool mapperHasVariant =
                mMapper.ContainsKey(variant.shader) && mMapper[variant.shader].FindIndex(v=>VariantCompare(new SerializableShaderVariant(variant), v)) != -1;

            Debug.Assert(collectionHasVariant == mapperHasVariant, "map对象与源文件内容不符");
            return collectionHasVariant;
        }

        public bool AddVariant(ShaderVariantCollection.ShaderVariant variant)
        {
            if (!mCollection.Add(variant))
                return false;

            if (!mMapper.ContainsKey(variant.shader))
                mMapper.Add(variant.shader, new List<SerializableShaderVariant>());
            
            mMapper[variant.shader].Add(new SerializableShaderVariant(variant));
            
            return true;
        }

        public bool RemoveVariant(ShaderVariantCollection.ShaderVariant variant)
        {
            if (!mCollection.Remove(variant))
                return false;
            
            if (mMapper.TryGetValue(variant.shader, out var variantList))
            {
                int index = variantList.FindIndex(v => VariantCompare(new SerializableShaderVariant(variant), v));
                Debug.Assert(index != -1, "map对象与源文件内容不符，无法在map对象中找到变体");
                variantList.RemoveAt(index);
                
                return true;
            }
            
            Debug.Assert(false, "map对象与源文件内容不符，无法在map对象中找到Shader");
            return false;
        }

        private ShaderVariantCollection.ShaderVariant PropToObject(Shader shader, SerializedProperty variantInfo)
        {
            PassType passType = (PassType)variantInfo.FindPropertyRelative("passType").intValue;
            string keywords = variantInfo.FindPropertyRelative("keywords").stringValue;
            string[] keywordSet = keywords.Split(' ');
            ShaderVariantCollection.ShaderVariant newVariant = new ShaderVariantCollection.ShaderVariant()
            {
                shader = shader,
                keywords = keywordSet,
                passType = passType
            };

            return newVariant;
        }
        
        public void Merge(ShaderVariantCollection otherFile)
        {
            if (otherFile == null)
                return;
            
            SerializedObject serializedOther = new UnityEditor.SerializedObject(otherFile);
            SerializedProperty m_Shaders = serializedOther.FindProperty("m_Shaders");

            for (int i = 0; i < m_Shaders.arraySize; ++i)
            {
                SerializedProperty pair = m_Shaders.GetArrayElementAtIndex(i);

                SerializedProperty first = pair.FindPropertyRelative("first");
                SerializedProperty second = pair.FindPropertyRelative("second");//ShaderInfo

                Shader shader = first.objectReferenceValue as Shader;

                SerializedProperty variants = second.FindPropertyRelative("variants");
                for (var vi = 0; vi < variants.arraySize; ++vi)
                {
                    SerializedProperty variantInfo = variants.GetArrayElementAtIndex(vi);

                    ShaderVariantCollection.ShaderVariant newVariant = PropToObject(shader, variantInfo);
                    if (!mCollection.Contains(newVariant))
                    {
                        mCollection.Add(newVariant);
                    }
                }
            }
            
            ReadFromFile();
        }

        public void Refresh() => ReadFromFile();

        public void SetSerializeFlag(bool serialize) => mSerializedFlag = serialize;
        
        public void OnBeforeSerialize()
        {
            if (!mSerializedFlag)
            {
                return;
            }
                
            if (mMapper == null)
                mMapper = new Dictionary<Shader, List<SerializableShaderVariant>>();
            
            mMapperSerializeKeys.Clear();
            mMapperSerializeValues.Clear();
            
            foreach (var kvp in mMapper)
            {
                mMapperSerializeKeys.Add(kvp.Key);
                //序列化与反序列化都需要浅拷贝一层引用，否则对map的value的list操作，依旧会改变序列化中的数值
                mMapperSerializeValues.Add(new ListWrapper(kvp.Value.ToList()));
            }
        }

        public void OnAfterDeserialize()
        {
            if (mMapper == null)
                mMapper = new Dictionary<Shader, List<SerializableShaderVariant>>();
            
            mMapper.Clear();
            for (int i = 0; i < mMapperSerializeKeys.Count; ++i)
            {
                mMapper.Add(mMapperSerializeKeys[i], mMapperSerializeValues[i].list.ToList());
            }
        }
    }

}
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    [System.Serializable]
    public class ToggleObject
    {
        [SerializeReference]
        public ScriptableObject obj;
        public bool use;
    }
    
    //解决嵌套list序列化问题
    [System.Serializable]
    public struct ListWrapper
    {
        public List<ToggleObject> list;
        public ListWrapper(List<ToggleObject> list) => this.list = list;
    }
    
    [CreateAssetMenu(menuName = "Soco/ShaderVariantCollectionTools/Create Config")]
    public class ShaderVariantCollectionToolConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        private Dictionary<System.Type, List<ToggleObject>> mToggleObjects = new Dictionary<Type, List<ToggleObject>>();

        [SerializeField] private List<string> mKeys = new List<string>();
        [SerializeField] private List<ListWrapper> mValues = new List<ListWrapper>();
        public void OnBeforeSerialize()
        {
            mKeys.Clear();
            mValues.Clear();

            foreach (var kvp in mToggleObjects)
            {
                mKeys.Add(kvp.Key.AssemblyQualifiedName);
                mValues.Add(new ListWrapper(kvp.Value));
            }
        }
        public void OnAfterDeserialize()
        {
            mToggleObjects.Clear();
            for (int i = 0; i < mKeys.Count; ++i)
            {
                System.Type type = System.Type.GetType(mKeys[i]);
                if (type != null)
                {
                    mToggleObjects.Add(type, mValues[i].list);
                }
            }
        }
        
        public List<ToggleObject> GetToggleObjectList(System.Type type)
        {
            if (!mToggleObjects.TryGetValue(type, out var list))
            {
                list = new List<ToggleObject>();
                mToggleObjects.Add(type, list);
            }

            return list;
        }

        public void AddToggleObject(ToggleObject obj)
        {
            GetToggleObjectList(obj.obj.GetType().BaseType).Add(obj);
        }

        public void RemoveToggleObject(ToggleObject obj)
        {
            GetToggleObjectList(obj.obj.GetType().BaseType).Remove(obj);
        }
    }
    
    public class ToolDefaultEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

    public abstract class ShaderVariantCollectionToolEditor : Editor
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ShaderVariantCollectionToolEditorAttribute : Attribute
    {
        public readonly Type componentType;

        public ShaderVariantCollectionToolEditorAttribute(Type componentType)
        {
            this.componentType = componentType;
        }
    }
}

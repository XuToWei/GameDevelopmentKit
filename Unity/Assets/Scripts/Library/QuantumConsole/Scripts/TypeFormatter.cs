using System;
using UnityEngine;
using UnityEngine.Serialization;

#region Preserve Fix
#if UNITY_2018_4_OR_NEWER
using UnityEngine.Scripting;
#else
/// <summary>
///   <para>PreserveAttribute prevents byte code stripping from removing a class, method, field, or property.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
public sealed class PreserveAttribute : Attribute
{
}
#endif
#endregion

namespace QFSW.QC
{
    [Serializable]
    public abstract class TypeFormatter : ISerializationCallbackReceiver
    {
        public Type Type { get; private set; }
        [SerializeField, HideInInspector] private string _type;

        [Preserve]
        protected TypeFormatter(Type type) { Type = type; }

        public void OnAfterDeserialize()
        {
            Type = Type.GetType(_type, false);
            if (Type == null) { Type = QuantumParser.ParseType(_type.Split(',')[0]); }
        }

        public void OnBeforeSerialize()
        {
            if (Type != null) { _type = Type.AssemblyQualifiedName; }
        }
    }

    [Serializable]
    public class TypeColorFormatter : TypeFormatter
    {
        [FormerlySerializedAs("color")]
        public Color Color = Color.white;

        [Preserve]
        public TypeColorFormatter(Type type) : base(type) { }
    }

    [Serializable]
    public class CollectionFormatter : TypeFormatter
    {
        [FormerlySerializedAs("seperatorString")]
        public string SeperatorString = ",";
        [FormerlySerializedAs("leftScoper")]
        public string LeftScoper = "[";
        [FormerlySerializedAs("rightScoper")]
        public string RightScoper = "]";

        [Preserve]
        public CollectionFormatter(Type type) : base(type) { }
    }
}
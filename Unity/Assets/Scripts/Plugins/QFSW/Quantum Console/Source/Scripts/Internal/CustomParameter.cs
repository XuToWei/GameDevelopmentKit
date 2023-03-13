using System;
using System.Collections.Generic;
using System.Reflection;

namespace QFSW.QC.Internal
{
    internal class CustomParameter : ParameterInfo
    {
        private readonly ParameterInfo _internalParameter;
        private readonly Type _typeOverride;
        private readonly string _nameOverride;

        public CustomParameter(ParameterInfo internalParameter, Type typeOverride, string nameOverride)
        {
            _typeOverride = typeOverride;
            _nameOverride = nameOverride;
            _internalParameter = internalParameter;
        }

        public CustomParameter(ParameterInfo internalParameter, string nameOverride) : this(internalParameter, internalParameter.ParameterType, nameOverride) { }

        public override Type ParameterType { get { return _typeOverride; } }
        public override string Name { get { return _nameOverride; } }

        public override ParameterAttributes Attributes { get { return _internalParameter.Attributes; } }
        public override object DefaultValue { get { return _internalParameter.DefaultValue; } }
        public override bool Equals(object obj) { return _internalParameter.Equals(obj); }
        public override IEnumerable<CustomAttributeData> CustomAttributes { get { return _internalParameter.CustomAttributes; } }
        public override object[] GetCustomAttributes(bool inherit) { return _internalParameter.GetCustomAttributes(inherit); }
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) { return _internalParameter.GetCustomAttributes(attributeType, inherit); }
        public override IList<CustomAttributeData> GetCustomAttributesData() { return _internalParameter.GetCustomAttributesData(); }
        public override int GetHashCode() { return _internalParameter.GetHashCode(); }
        public override Type[] GetOptionalCustomModifiers() { return _internalParameter.GetOptionalCustomModifiers(); }
        public override Type[] GetRequiredCustomModifiers() { return _internalParameter.GetRequiredCustomModifiers(); }
        public override bool HasDefaultValue => _internalParameter.HasDefaultValue;
        public override bool IsDefined(Type attributeType, bool inherit) { return _internalParameter.IsDefined(attributeType, inherit); }
        public override MemberInfo Member { get { return _internalParameter.Member; } }
        public override int MetadataToken { get { return _internalParameter.MetadataToken; } }
        public override int Position { get { return _internalParameter.Position; } }
        public override object RawDefaultValue { get { return _internalParameter.RawDefaultValue; } }
        public override string ToString() { return _internalParameter.ToString(); }
    }
}
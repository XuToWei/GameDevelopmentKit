using UnityEngine;
using System;

namespace Slate
{

    [AttributeUsage(AttributeTargets.Class)]
    ///<summary>Use to override the naming of a type</summary>
    public class NameAttribute : Attribute
    {
        public readonly string name;
        public NameAttribute(string name) {
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    ///<summary>Use to set the category of a type</summary>
    public class CategoryAttribute : Attribute
    {
        public readonly string category;
        public CategoryAttribute(string category) {
            this.category = category;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    ///<summary>Use to set the description of a type</summary>
    public class DescriptionAttribute : Attribute
    {
        public readonly string description;
        public DescriptionAttribute(string description) {
            this.description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    ///<summary>Use to specify an icon for a type</summary>
    public class IconAttribute : Attribute
    {
        public readonly string iconName;
        public readonly System.Type fromType;
        public IconAttribute(string iconName) {
            this.iconName = iconName;
        }
        public IconAttribute(System.Type fromType) {
            this.fromType = fromType; ;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    ///<summary>Use to define which other types this type can be attached to in regards to IDirectrables</summary>
    public class AttachableAttribute : Attribute
    {
        public readonly Type[] types;
        public AttachableAttribute(params Type[] types) {
            this.types = types;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    ///<summary>Use to define that this Type should be unique within it's IDirectable parent</summary>
    public class UniqueElementAttribute : Attribute { }


    ///<summary>Attribute used along with a Vector3 to show it's trajectory in the scene</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ShowTrajectoryAttribute : Attribute { }

    ///<summary>Attribute used along with a Vector3 to control it with a position handle in the scene</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PositionHandleAttribute : Attribute
    {
        public readonly string rotationPropertyName;
        public PositionHandleAttribute() { }
        public PositionHandleAttribute(string rotationPropertyName) {
            this.rotationPropertyName = rotationPropertyName;
        }
    }

    ///<summary>Attribute used along with a Vector3 to control it with a rotation handle in the scene</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RotationHandleAttribute : Attribute
    {
        public readonly string positionPropertyName;
        public RotationHandleAttribute(string positionPropertyName) {
            this.positionPropertyName = positionPropertyName;
        }
    }
}
using UnityEngine;
using System;

namespace Slate
{

    ///<summary>Attribute to mark a field or property as an animatable parameter</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AnimatableParameterAttribute : PropertyAttribute
    {
        public string link;
        public readonly float? min;
        public readonly float? max;
        public readonly string customName;
        public AnimatableParameterAttribute() { }
        public AnimatableParameterAttribute(string customName) {
            this.customName = customName;
        }
        public AnimatableParameterAttribute(string customName, float min, float max) {
            this.customName = customName;
            this.min = min;
            this.max = max;
        }
        public AnimatableParameterAttribute(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }

    ///<summary>Attribute to mark a class to be parsed for sub animatable parameters</summary>
    ///TODO: Support structs
    [AttributeUsage(AttributeTargets.Class)]
    public class ParseAnimatableParametersAttribute : PropertyAttribute { }

    ///<summary>Attribute used to show a popup of shader properties of type</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ShaderPropertyPopupAttribute : PropertyAttribute
    {
        public readonly Type propertyType;
        public ShaderPropertyPopupAttribute() { }
        public ShaderPropertyPopupAttribute(Type propertyType) {
            this.propertyType = propertyType;
        }
    }

    ///<summary>Attribute used to make a bool display as left toggle</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LeftToggleAttribute : PropertyAttribute { }

    ///<summary>Attribute used to restrict float or int to a min value</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MinAttribute : PropertyAttribute
    {
        public readonly float min;
        public MinAttribute(float min) {
            this.min = min;
        }
    }

    ///<summary>Show an example text in place of string field if string is null or empty</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ExampleTextAttribute : PropertyAttribute
    {
        public readonly string text;
        public ExampleTextAttribute(string text) {
            this.text = text;
        }
    }

    ///<summary>Shows a HelpBox bellow field</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public readonly string text;
        public HelpBoxAttribute(string text) {
            this.text = text;
        }
    }

    ///<summary>Shows the property only if another property/field returns the specified value. The target value is int type, which means that can both be used for boolean as well as enum targets</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string propertyName;
        public readonly int value;
        public ShowIfAttribute(string propertyName, int value) {
            this.propertyName = propertyName;
            this.value = value;
        }
    }

    ///<summary>Enabled the property only if another property/field returns the specified value. The target value is int type, which means that can both be used for boolean as well as enum targets</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnabledIfAttribute : PropertyAttribute
    {
        public readonly string propertyName;
        public readonly int value;
        public EnabledIfAttribute(string propertyName, int value) {
            this.propertyName = propertyName;
            this.value = value;
        }
    }

    ///<summary>Callbacks target method when property changes in inspector</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CallbackAttribute : PropertyAttribute
    {
        public readonly string methodName;
        public CallbackAttribute(string methodName) {
            this.methodName = methodName;
        }
    }

    ///<summary>Attribute used on Object or string field to mark them as required (red) if not set</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute { }

    ///<summary>Attribute used to protect changes when cutscene playing</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PlaybackProtectedAttribute : PropertyAttribute { }

    ///<summary>Attribute used to view field as read-only</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }

    ///<summary>Used for a sorting layer popup</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SortingLayerAttribute : PropertyAttribute { }

    ///<summary>Makes a field of type CutsceneGroup show a dropdown selection of groups</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ActorGroupPopupAttribute : PropertyAttribute { }
}
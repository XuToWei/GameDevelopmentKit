#if UNITY_2021_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{

    public class UXBuilderRadioGroupStruct
    {
        public string className = "UXRadioGroup";
        public UXStyle style = new UXStyle();

        // public bool allowClear = true;
        public bool disabled = false;

        public List<string> options = new List<string>() { "" };

        public Action<int> onChange = val => { };
    }
    public class UXBuilderRadioGroup : RadioButtonGroup
    {
        private static UXBuilderRadioGroup _mUXRadioGroup;
        private static UXStyle _mStyle = new UXStyle();

        private static VisualElement _mUXRadio = new VisualElement();
        private static VisualElement _mUXInput = new VisualElement();
        private static VisualElement _mUXArrow = new VisualElement();

        private static UXBuilderRadioGroupStruct _mComponent = new UXBuilderRadioGroupStruct();

        public UXBuilderRadioGroupStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderRadioGroup SetComponents(UXBuilderRadioGroupStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public UXBuilderRadioGroup(List<string> choices)
            : base((string)null, choices)
        {
        }

        public void SetOptions(List<string> options)
        {
            _mUXRadioGroup.choices = options;
        }

        public static UXBuilderRadioGroup Create(VisualElement root, UXBuilderRadioGroupStruct component)
        {
            _mComponent = component;

            _mUXRadioGroup = new UXBuilderRadioGroup(component.options);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath +
                                                          "USS/UXElements/UXRadioGroup.uss");
            _mUXRadioGroup.styleSheets.Add(styleSheet);

            _mUXRadioGroup.AddToClassList("ux-radio-group");
            _mUXRadioGroup.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXRadioGroup.style);

            _mUXRadioGroup.Q<VisualElement>(null, "unity-base-field__input").AddToClassList("ux-radio-group-field");
            _mUXRadioGroup.Query<VisualElement>(null, "unity-radio-button__input")
                .ForEach(element => element.AddToClassList("ux-radio-cursor"));

            _mUXRadioGroup.Query<RadioButton>().ForEach(button => button.AddToClassList("ux-radio"));
            _mUXRadioGroup.Query<Label>().ForEach(button => button.AddToClassList("ux-radio-label"));

            InitComponent(component);

            root.Add(_mUXRadioGroup);

            return _mUXRadioGroup;
        }

        private static void InitComponent(UXBuilderRadioGroupStruct component)
        {
            InitDisabled(component.disabled);
            InitOnChange(component.onChange);
            InitStyle(component.style);
        }

        private static void InitStyle(UXStyle style)
        {
            if (style == _mStyle) return;
            UXStyle compStyle = new UXStyle();
            Type type = typeof(UXStyle);
            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(style);
                if (!value.Equals(property.GetValue(compStyle)))
                {
                    property.SetValue(_mStyle, property.GetValue(style));
                }
            }
            StyleCopy.UXStyleToIStyle(_mUXRadioGroup.style, _mStyle);
        }

        private static void InitDisabled(bool disabled)
        {
            if (disabled)
            {
                _mUXRadioGroup.Query<VisualElement>(null, "unity-radio-button__input")
                    .ForEach(element => element.AddToClassList("ux-radio-cursor-disabled"));
            }

            _mUXRadioGroup.SetEnabled(!disabled); ;
        }


        private static void InitOnChange(Action<int> action)
        {
            _mUXRadioGroup.RegisterValueChangedCallback(evt => action(evt.newValue));
        }
    }
}
#endif
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderCheckBoxStruct
    {
        public string className = "UXCheckBox";
        public UXStyle style = new UXStyle();

        public bool disabled = false;
        // public bool error = false;

        public Action<bool> onChange = (bool val) => { };
    }

    public class UXBuilderCheckBox : Toggle
    {
        private static UXBuilderCheckBox _mUXCheckBox;
        private static VisualElement _mUXCheckBoxField;
        // private static VisualElement _mUXCheckBoxCheck;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderCheckBoxStruct _mComponent = new UXBuilderCheckBoxStruct();

        public UXBuilderCheckBoxStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderCheckBox SetComponents(UXBuilderCheckBoxStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderCheckBox Create(VisualElement root, UXBuilderCheckBoxStruct component)
        {
            _mComponent = component;

            _mUXCheckBox = new UXBuilderCheckBox();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXCheckBox.uss");
            _mUXCheckBox.styleSheets.Add(styleSheet);

            _mUXCheckBoxField = _mUXCheckBox.Q<VisualElement>(null, "unity-toggle__input");
            // _mUXCheckBoxCheck = _mUXCheckBox.Q<VisualElement>("unity-checkmark");

            _mUXCheckBox.AddToClassList("ux-checkbox");
            _mUXCheckBoxField.AddToClassList("ux-checkbox-field");

            _mUXCheckBox.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXCheckBox.style);
            InitComponent(component);
            root.Add(_mUXCheckBox);

            return _mUXCheckBox;
        }

        private static void InitComponent(UXBuilderCheckBoxStruct component)
        {
            InitDisabled(component.disabled);
            InitOnChange(component.onChange);
            // InitError(component.error);
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
                var val = property.GetValue(style);
                if (!val.Equals(property.GetValue(compStyle)))
                {
                    property.SetValue(_mStyle, property.GetValue(style));
                }
            }

            StyleCopy.UXStyleToIStyle(_mUXCheckBox.style, _mStyle);
        }

        private static void InitOnChange(Action<bool> action)
        {
            _mUXCheckBox.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
            {
                action(evt.newValue);
            });
        }

        private static void InitDisabled(bool disabled)
        {
            if (disabled) _mUXCheckBoxField.AddToClassList("ux-checkbox-field-disabled");
            _mUXCheckBox.SetEnabled(!disabled); ;
        }
    }
}
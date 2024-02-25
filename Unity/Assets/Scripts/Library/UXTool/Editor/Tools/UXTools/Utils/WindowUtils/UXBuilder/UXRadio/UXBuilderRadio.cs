#if UNITY_2021_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderRadioStruct
    {
        public string className = "UXRadio";
        public UXStyle style = new UXStyle();
        
        public bool disabled = false;

        public Action<bool> onChange = (bool val) => { };
    }
    
    public class UXBuilderRadio : RadioButton
    {
        private static UXBuilderRadio _mUXRadio;
        private static VisualElement _mUXRadioField;
        private static UXStyle _mStyle = new UXStyle();
        
        private static UXBuilderRadioStruct _mComponent = new UXBuilderRadioStruct();

        public UXBuilderRadioStruct GetComponents()
        {
            return _mComponent;
        }
        
        public UXBuilderRadio SetComponents(UXBuilderRadioStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent,_mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }
        
        public static UXBuilderRadio Create(VisualElement root, UXBuilderRadioStruct component)
        {
            _mComponent = component;
            
            _mUXRadio = new UXBuilderRadio();
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXRadio.uss");
            _mUXRadio.styleSheets.Add(styleSheet);

            _mUXRadioField = _mUXRadio.Q<VisualElement>(null,"unity-radio-button__input");
            
            _mUXRadio.AddToClassList("ux-radio");
            _mUXRadioField.AddToClassList("ux-radio-field");
            
            _mUXRadio.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle,_mUXRadio.style);
            InitComponent(component);
            root.Add(_mUXRadio);

            return _mUXRadio;
        }
        
        private static void InitComponent(UXBuilderRadioStruct component)
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

            StyleCopy.UXStyleToIStyle(_mUXRadio.style, _mStyle);
        }

        private static void InitOnChange(Action<bool> action)
        {
            _mUXRadio.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
            {
                action(evt.newValue);
            });
        }
        
        private static void InitDisabled(bool disabled)
        {
            if (disabled) _mUXRadioField.AddToClassList("ux-radio-field-disabled");
            _mUXRadio.SetEnabled(!disabled);;
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderSelectStruct
    {
        public string className = "UXSelect";
        public UXStyle style = new UXStyle();
        
        // public bool allowClear = true;
        public bool disabled = false;

        public List<string> options = new List<string>(){""};
        public string defaultValue = "";
        
        public Action<string> onChange = (string str) => { };
    }
    
    public class UXBuilderSelect : PopupField<string>
    {
        private static UXBuilderSelect _mUXSelect;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderSelectStruct _mComponent = new UXBuilderSelectStruct();

        public UXBuilderSelectStruct GetComponents()
        {
            return _mComponent;
        }
        
        public UXBuilderSelect SetComponents(UXBuilderSelectStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent,_mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }
        
        public UXBuilderSelect(
            List<string> choices, string defaultValue)
            : base((string)null, choices, defaultValue, null, null)
        {
        }
#if UNITY_2020_3_OR_NEWER
        public void SetOptions(List<string> options)
        {
            _mUXSelect.choices = options;
        }            
#endif
        public static UXBuilderSelect Create(VisualElement root, UXBuilderSelectStruct component)
        {
            if (!component.options.Contains(component.defaultValue))
            {
                Debug.LogWarning("Default value is not present in the list of possible values.Changed to the top value of Options");
                component.defaultValue = component.options.Count > 0 ? component.options[0] : "";
            }
            _mComponent = component;
            
            _mUXSelect = new UXBuilderSelect(component.options, component.defaultValue);
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXSelect.uss");
            _mUXSelect.styleSheets.Add(styleSheet);
            
            _mUXSelect.AddToClassList("ux-select");
            _mUXSelect.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle,_mUXSelect.style);
            
            _mUXSelect.Q<VisualElement>(null,"unity-base-popup-field__input").AddToClassList("ux-select__background");

            _mUXSelect.Q<VisualElement>(null,"unity-base-popup-field__text").AddToClassList("ux-select__input");
            
            _mUXSelect.Q<VisualElement>(null,"unity-base-popup-field__arrow").AddToClassList("ux-select__arrow");

            InitComponent(component);
            
            root.Add(_mUXSelect);
            
            return _mUXSelect;
        }

        private static void InitComponent(UXBuilderSelectStruct component)
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
            StyleCopy.UXStyleToIStyle(_mUXSelect.style, _mStyle);
        }
        
        private static void InitDisabled(bool disabled)
        {
            if (disabled) _mUXSelect.AddToClassList("ux-select-disabled");
            _mUXSelect.SetEnabled(!disabled);;
        }
        

        private static void InitOnChange(Action<string> action)
        {
            _mUXSelect.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                action(evt.newValue);
            });
        }
        
    }
}
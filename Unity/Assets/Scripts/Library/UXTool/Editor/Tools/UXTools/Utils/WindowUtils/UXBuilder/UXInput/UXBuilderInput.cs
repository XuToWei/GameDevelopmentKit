using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderInputStruct
    {
        public string className = "UXInput";
        public UXStyle style = new UXStyle();
        
        // public bool allowClear = true;
        public bool disabled = false;
        public bool error = false;
        public bool password = false;
        public bool readOnly = false;
        public int maxLength = -1;
        
        public Action<string> onChange = (string str) => { };
        // public Action onClear = () => { };
    }
    
    public class UXBuilderInput : TextField
    {
        private static UXBuilderInput _mUXInput;
        private static VisualElement _mUXInputField;
        private static UXStyle _mStyle = new UXStyle();
        
        private static UXBuilderInputStruct _mComponent = new UXBuilderInputStruct();

        public UXBuilderInputStruct GetComponents()
        {
            return _mComponent;
        }
        
        public UXBuilderInput SetComponents(UXBuilderInputStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent,_mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderInput Create(VisualElement root, UXBuilderInputStruct component)
        {
            _mComponent = component;
            
            _mUXInput = new UXBuilderInput();
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXInput.uss");
            _mUXInput.styleSheets.Add(styleSheet);
            
            _mUXInput.AddToClassList("ux-input");
            _mUXInputField = _mUXInput.Q<VisualElement>("unity-text-input");
            _mUXInputField.AddToClassList("ux-input-field");
            _mUXInput.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle,_mUXInput.style);
            InitComponent(component);
            root.Add(_mUXInput);
            
            return _mUXInput;
        }

        private static void InitComponent(UXBuilderInputStruct component)
        {
            InitMaxLength(component.maxLength);
            InitReadOnly(component.readOnly);
            InitDisabled(component.disabled);
            InitPassword(component.password);
            InitOnChange(component.onChange);
            InitError(component.error);
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
            StyleCopy.UXStyleToIStyle(_mUXInput.style, _mStyle);
            type = typeof(IStyle);
            properties = type.GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name.ToLower();
                if (name.Contains("text") || name.Contains("font") || name.Contains("color"))
                {
                    property.SetValue(_mUXInputField.style, property.GetValue(_mUXInput.style));
                }
            }
        }
        
        private static void InitMaxLength(int maxLength)
        {
            _mUXInput.maxLength = maxLength;
        }

        private static void InitReadOnly(bool readOnly)
        {
            _mUXInput.isReadOnly = readOnly;
        }
        
        private static void InitDisabled(bool disabled)
        {
            if (disabled) _mUXInputField.AddToClassList("ux-input-field-disabled");
            _mUXInput.SetEnabled(!disabled);;
        }

        private static void InitPassword(bool password)
        {
            _mUXInput.isPasswordField = password;
        }

        private static void InitError(bool error)
        {
            if (error)
            {
                _mUXInputField.AddToClassList("ux-input-field-error");
            }
        }

        private static void InitOnChange(Action<string> action)
        {
            _mUXInput.RegisterCallback((ChangeEvent<string> evt) =>
            {
                action(evt.newValue);
            });
        }
        
    }
}
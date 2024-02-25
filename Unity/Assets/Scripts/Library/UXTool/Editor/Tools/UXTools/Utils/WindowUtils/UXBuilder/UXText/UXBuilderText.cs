using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderTextStruct
    {
        // public IStyle style;
        public string className = "UXText";
        public UXStyle style = new UXStyle();
        public string text = "Text";
    }
    
    public class UXBuilderText : Label
    {
        private static UXBuilderText _mUXText;
        private static UXStyle _mStyle = new UXStyle();
        
        private static UXBuilderTextStruct _mComponent = new UXBuilderTextStruct();

        public UXBuilderTextStruct GetComponents()
        {
            return _mComponent;
        }
        
        public UXBuilderText SetComponents(UXBuilderTextStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent,_mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }
        
        public static UXBuilderText Create(VisualElement root, UXBuilderTextStruct component)
        {
            _mComponent = component;
            
            _mUXText = new UXBuilderText();
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXText.uss");
            _mUXText.styleSheets.Add(styleSheet);
            
            _mUXText.AddToClassList("ux-text");
            
            _mUXText.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle,_mUXText.style);
            InitComponent(component);
            root.Add(_mUXText);

            return _mUXText;
        }

        private static void InitComponent(UXBuilderTextStruct component)
        {
            InitText(component.text);
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
            StyleCopy.UXStyleToIStyle(_mUXText.style, _mStyle);
        }
        
        private static void InitText(string text)
        {
            _mUXText.text = text;
        }
    }
}
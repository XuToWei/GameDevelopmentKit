using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderDividerStruct
    {
        // public IStyle style;
        public string className = "UXButton";
        public UXStyle style = new UXStyle();
        public UXDividerType type = UXDividerType.Horizontal;
        // public UXDividerPosition orientation = UXDividerPosition.Center;
        // public string text = "";
    }
    
    public enum UXDividerType
    {
        Vertical,
        Horizontal,
    }
    
    public enum UXDividerPosition 
    {
        Left,
        Center,
        Right,
    }
    
    public class UXBuilderDivider : VisualElement
    {
        private static UXBuilderDivider _mUXDivider;
        private static UXStyle _mStyle = new UXStyle();
        
        private static UXBuilderDividerStruct _mComponent = new UXBuilderDividerStruct();

        public UXBuilderDividerStruct GetComponents()
        {
            return _mComponent;
        }
        
        public UXBuilderDivider SetComponents(UXBuilderDividerStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent,_mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }
        
        public static UXBuilderDivider Create(VisualElement root, UXBuilderDividerStruct component)
        {
            _mComponent = component;
            
            _mUXDivider = new UXBuilderDivider();
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXDivider.uss");
            _mUXDivider.styleSheets.Add(styleSheet);
            
            _mUXDivider.AddToClassList("ux-divider");
            
            _mUXDivider.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle,_mUXDivider.style);
            InitComponent(component);
            root.Add(_mUXDivider);

            return _mUXDivider;
        }
        
        private static void InitComponent(UXBuilderDividerStruct component)
        {
            InitType(component.type);
            // InitOrientation(component.orientation);
            // InitText(component);
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
            StyleCopy.UXStyleToIStyle(_mUXDivider.style, _mStyle);
        }
        
        private static void InitType(UXDividerType type)
        {
            switch (type)
            {
                case UXDividerType.Horizontal:
                    _mUXDivider.AddToClassList("ux-divider-horizontal");
                    break;
                case UXDividerType.Vertical:
                    _mUXDivider.AddToClassList("ux-divider-vertical");
                    break;
            }
        }
    }
}
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderRowStruct
    {
        public string className = "UXRow";
        public UXStyle style = new UXStyle();

        public StyleEnum<Align> align = Align.Auto;
        public StyleEnum<Justify> justify = Justify.FlexStart;
    }
    
    public class UXBuilderRow : VisualElement
    {
        private static UXBuilderRow _mUXRow;
        private static UXStyle _mStyle = new UXStyle();
        
        private static UXBuilderRowStruct _mComponent = new UXBuilderRowStruct();
        
        public UXBuilderRowStruct GetComponents()
        {
            return _mComponent;
        }
        
        public UXBuilderRow SetComponents(UXBuilderRowStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent,_mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }
        
        public static UXBuilderRow Create(VisualElement root, UXBuilderRowStruct component)
        {
            _mComponent = component;

            _mUXRow = new UXBuilderRow();
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXRow.uss");
            _mUXRow.styleSheets.Add(styleSheet);
            
            _mUXRow.AddToClassList("ux-row");
            
            _mUXRow.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle,_mUXRow.style);
            InitComponent(component);
            root.Add(_mUXRow);

            return _mUXRow;
        }
        
        private static void InitComponent(UXBuilderRowStruct component)
        {
            InitStyle(component.style);
            InitAlign(component.align);
            InitJustify(component.justify);
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
            StyleCopy.UXStyleToIStyle(_mUXRow.style, _mStyle);
        }

        private static void InitAlign(StyleEnum<Align> align)
        {
            _mUXRow.style.alignItems = align;
        }

        private static void InitJustify(StyleEnum<Justify> justify)
        {
            _mUXRow.style.justifyContent = justify;
        }
    }
}
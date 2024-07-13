using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderColStruct
    {
        public string className = "UXCol";
        public UXStyle style = new UXStyle();

        public int offset = 0;
        public int span = 24;
    }


    public class UXBuilderCol : VisualElement
    {
        private static UXBuilderCol _mUXCol;
        private static VisualElement _mUXParent;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderColStruct _mComponent = new UXBuilderColStruct();

        public UXBuilderColStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderCol SetComponents(UXBuilderColStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderCol Create(VisualElement root, UXBuilderColStruct component)
        {
            _mUXParent = root;

            _mComponent = component;

            _mUXCol = new UXBuilderCol();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXCol.uss");
            _mUXCol.styleSheets.Add(styleSheet);

            _mUXCol.AddToClassList("ux-col");

            _mUXCol.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXCol.style);
            InitComponent(component);
            root.Add(_mUXCol);

            return _mUXCol;
        }

        private static void InitComponent(UXBuilderColStruct component)
        {
            InitStyle(component.style);
            InitOffset(component.offset);
            InitSpan(component.span);
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
            StyleCopy.UXStyleToIStyle(_mUXCol.style, _mStyle);
        }

        private static void InitSpan(int span)
        {
            _mUXCol.style.width = Length.Percent(span / 24f * 100f);
        }

        private static void InitOffset(int offset)
        {
            _mUXCol.style.marginLeft = Length.Percent(offset / 24f * 100f);
        }
    }
}
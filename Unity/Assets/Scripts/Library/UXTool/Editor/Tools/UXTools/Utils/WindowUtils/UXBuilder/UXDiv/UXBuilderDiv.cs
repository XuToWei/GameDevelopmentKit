using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderDivStruct
    {
        public string className = "UXDiv";
        public UXStyle style = new UXStyle();

    }

    public class UXBuilderDiv : VisualElement
    {
        private static UXBuilderDiv _mUXDiv;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderDivStruct _mComponent = new UXBuilderDivStruct();

        public UXBuilderDivStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderDiv SetComponents(UXBuilderDivStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderDiv Create(VisualElement root, UXBuilderDivStruct component)
        {
            _mComponent = component;

            _mUXDiv = new UXBuilderDiv();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXDiv.uss");
            _mUXDiv.styleSheets.Add(styleSheet);

            _mUXDiv.AddToClassList("ux-div");

            _mUXDiv.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXDiv.style);
            InitComponent(component);
            root.Add(_mUXDiv);

            return _mUXDiv;
        }

        private static void InitComponent(UXBuilderDivStruct component)
        {
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
            StyleCopy.UXStyleToIStyle(_mUXDiv.style, _mStyle);
        }
    }
}
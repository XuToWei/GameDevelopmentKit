using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderScrollViewStruct
    {
        public string className = "UXScrollView";
        public UXStyle style = new UXStyle();
#if UNITY_2021_1_OR_NEWER
        public ScrollerVisibility verticalScrollerVisibility = ScrollerVisibility.Auto;
        public ScrollerVisibility horizontalScrollerVisibility = ScrollerVisibility.Auto;
#endif
    }

    public class UXBuilderScrollView : ScrollView
    {
        private static UXBuilderScrollView _mUXScrollView;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderScrollViewStruct _mComponent = new UXBuilderScrollViewStruct();

        public UXBuilderScrollViewStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderScrollView SetComponents(UXBuilderScrollViewStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderScrollView Create(VisualElement root, UXBuilderScrollViewStruct component)
        {
            _mComponent = component;

            _mUXScrollView = new UXBuilderScrollView();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXScrollView.uss");
            _mUXScrollView.styleSheets.Add(styleSheet);

            _mUXScrollView.AddToClassList("ux-scroll-view");

            _mUXScrollView.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXScrollView.style);
            InitComponent(component);
#if UNITY_2021_1_OR_NEWER
            _mUXScrollView.verticalScrollerVisibility = component.verticalScrollerVisibility;
            _mUXScrollView.horizontalScrollerVisibility = component.horizontalScrollerVisibility;
#endif
            root.Add(_mUXScrollView);

            return _mUXScrollView;
        }

        private static void InitComponent(UXBuilderScrollViewStruct component)
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
            StyleCopy.UXStyleToIStyle(_mUXScrollView.style, _mStyle);
        }
    }
}
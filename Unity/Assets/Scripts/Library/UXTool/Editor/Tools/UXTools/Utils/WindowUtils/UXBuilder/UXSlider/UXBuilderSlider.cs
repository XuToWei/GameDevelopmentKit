using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderSliderStruct
    {
        public string className = "UXSlider";
        public UXStyle style = new UXStyle();

        public bool disabled = false;
        // public bool error = false;
        public float maxValue = 100f;
        public float minValue = 0f;

        public Action<float> onChange = (float val) => { };
    }

    public class UXBuilderSlider : Slider
    {
        private static UXBuilderSlider _mUXSlider;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderSliderStruct _mComponent = new UXBuilderSliderStruct();

        public UXBuilderSliderStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderSlider SetComponents(UXBuilderSliderStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public UXBuilderSlider(
            float minValue, float maxValue)
            : base(minValue, maxValue)
        {
        }


        public static UXBuilderSlider Create(VisualElement root, UXBuilderSliderStruct component)
        {
            _mComponent = component;

            _mUXSlider = new UXBuilderSlider(component.minValue, component.maxValue);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXSlider.uss");
            _mUXSlider.styleSheets.Add(styleSheet);


            _mUXSlider.AddToClassList("ux-slider");

            _mUXSlider.Q<VisualElement>("unity-drag-container").AddToClassList("ux-slider-field");
            _mUXSlider.Q<VisualElement>("unity-dragger").AddToClassList("ux-slider-field");

            _mUXSlider.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXSlider.style);
            InitComponent(component);
            root.Add(_mUXSlider);

            return _mUXSlider;
        }

        private static void InitComponent(UXBuilderSliderStruct component)
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

            StyleCopy.UXStyleToIStyle(_mUXSlider.style, _mStyle);
        }

        private static void InitOnChange(Action<float> action)
        {
            _mUXSlider.RegisterValueChangedCallback(evt => action(evt.newValue));
        }

        private static void InitDisabled(bool disabled)
        {
            if (disabled)
            {
                _mUXSlider.Q<VisualElement>("unity-drag-container").AddToClassList("ux-slider-field-disabled");
                _mUXSlider.Q<VisualElement>("unity-dragger").AddToClassList("ux-slider-field-disabled");
            }
            _mUXSlider.SetEnabled(!disabled); ;
        }
    }
}
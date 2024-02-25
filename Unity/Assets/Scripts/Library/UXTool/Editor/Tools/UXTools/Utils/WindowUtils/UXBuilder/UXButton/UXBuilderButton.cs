using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderButtonStruct
    {
        // public IStyle style;
        public string className = "UXButton";
        public UXStyle style = new UXStyle();
        public bool disabled = false;
        public Action OnClick = () => { };
        public ButtonType type = ButtonType.Default;
        public UXBuilderStatus status = UXBuilderStatus.Default;
        public string text = "Button";
    }


    public class UXBuilderButton : Button
    {
        private static UXBuilderButton _mUXButton;
        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderButtonStruct _mComponent = new UXBuilderButtonStruct();

        public UXBuilderButtonStruct GetComponents()
        {
            return _mComponent;
        }

        public UXBuilderButton SetComponents(UXBuilderButtonStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }


        public static UXBuilderButton Create(VisualElement root, UXBuilderButtonStruct component)
        {
            _mComponent = component;

            _mUXButton = new UXBuilderButton();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXButton.uss");
            _mUXButton.styleSheets.Add(styleSheet);

            _mUXButton.AddToClassList("ux-button");

            _mUXButton.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXButton.style);
            InitComponent(component);
            root.Add(_mUXButton);

            return _mUXButton;
        }

        private static void InitComponent(UXBuilderButtonStruct component)
        {
            InitDisabled(component.disabled);
            InitOnClick(component.OnClick);
            InitType(component.type);
            InitStatus(component.status);
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
            StyleCopy.UXStyleToIStyle(_mUXButton.style, _mStyle);
        }

        private static void InitDisabled(bool disabled)
        {
            if (disabled) _mUXButton.AddToClassList("ux-button-disabled");
            _mUXButton.SetEnabled(!disabled);
        }

        private static void InitOnClick(Action action)
        {
            _mUXButton.clicked += action;
        }

        private static void InitType(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Default:
                    _mUXButton.AddToClassList("ux-button-secondary");
                    break;
                case ButtonType.Secondary:
                    _mUXButton.AddToClassList("ux-button-secondary");
                    break;
                case ButtonType.Primary:
                    _mUXButton.AddToClassList("ux-button-primary");
                    break;
                case ButtonType.Outline:
                    _mUXButton.AddToClassList("ux-button-outline");
                    break;
                case ButtonType.Text:
                    _mUXButton.AddToClassList("ux-button-text");
                    break;
                case ButtonType.Icon:
                    _mUXButton.AddToClassList("ux-button-icon");
                    break;
            }
        }

        private static void InitText(string text)
        {
            if (_mComponent.type == ButtonType.Icon)
            {
                return;
            }
            _mUXButton.text = text;
        }

        private static void InitStatus(UXBuilderStatus status)
        {
            switch (status)
            {
                case UXBuilderStatus.Default:
                    break;
                case UXBuilderStatus.Success:
                    _mUXButton.AddToClassList("ux-button-success");
                    break;
                case UXBuilderStatus.Danger:
                    _mUXButton.AddToClassList("ux-button-danger");
                    break;
                case UXBuilderStatus.Warning:
                    _mUXButton.AddToClassList("ux-button-warning");
                    break;
            }
        }
    }


}
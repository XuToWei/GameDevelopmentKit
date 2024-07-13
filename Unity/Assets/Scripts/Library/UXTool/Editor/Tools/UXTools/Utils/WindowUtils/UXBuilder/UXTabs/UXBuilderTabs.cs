using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UXBuilderTabsStruct
    {
        public string className = "UXTabs";
        public UXStyle style = new UXStyle();

        public UXStyle tabStyle = new UXStyle();
        public UXTabsPosition position = UXTabsPosition.Left;
        public Action<string, VisualElement> onTabClick = (string val, VisualElement content) => { };

        public List<string> titles = new List<string>() { "Tab1", "Tab2" };
        public string defaultTitle = "";
    }

    public class UXBuilderTabs : VisualElement
    {
        private static UXBuilderTabs _mUXTabs;

        private static VisualElement rightContainer;
        private static VisualElement leftContainer;
        private static ScrollView labelScroll;
        private static ScrollView widgetScroll;

        private static UXStyle _mStyle = new UXStyle();

        private static UXBuilderTabsStruct _mComponent = new UXBuilderTabsStruct();


        public UXBuilderTabsStruct GetComponents()
        {
            return _mComponent;
        }

        public VisualElement GetRightContainer()
        {
            return rightContainer;
        }

        public VisualElement GetLeftContainer()
        {
            return leftContainer;
        }

        public VisualElement GetLabelScroll()
        {
            return labelScroll;
        }

        public VisualElement GetWidgetScroll()
        {
            return widgetScroll;
        }

        public UXBuilderTabs SetComponents(UXBuilderTabsStruct component)
        {
            VisualElement parent = this.hierarchy.parent;
            if (parent == null) return this;
            _mComponent = component;
            var a = Create(parent, _mComponent);
            a.PlaceBehind(this);
            parent.Remove(this);
            return a;
        }

        public static UXBuilderTabs Create(VisualElement root, UXBuilderTabsStruct component)
        {
            _mComponent = component;

            _mUXTabs = new UXBuilderTabs();

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ThunderFireUIToolConfig.UIBuilderPath + "USS/UXElements/UXTabs.uss");
            _mUXTabs.styleSheets.Add(styleSheet);

            _mUXTabs.AddToClassList("ux-tabs");

            _mUXTabs.name = component.className;
            StyleCopy.IStyleToUXStyle(_mStyle, _mUXTabs.style);

            switch (component.position)
            {
                case UXTabsPosition.Left:
                    DrawElementLeft(component);
                    break;
                case UXTabsPosition.Top:
                    DrawElementTop(component);
                    break;
            }

            InitComponent(component);
            root.Add(_mUXTabs);

            return _mUXTabs;
        }

        private static void InitComponent(UXBuilderTabsStruct component)
        {
            // InitDisabled(component.disabled);
            // InitOnChange(component.onChange);
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

            StyleCopy.UXStyleToIStyle(_mUXTabs.style, _mStyle);
        }

        private static void DrawElementLeft(UXBuilderTabsStruct component)
        {
            var row = UXBuilder.Row(_mUXTabs, new UXBuilderRowStruct()
            {
                style = new UXStyle() { height = Length.Percent(100) }
            });
            leftContainer = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 5,
                style = new UXStyle() { borderTopWidth = 10, height = Length.Percent(100), maxWidth = 180 }
            });

            labelScroll = UXBuilder.ScrollView(leftContainer, new UXBuilderScrollViewStruct());


            var dragLine = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 1,
                style = new UXStyle()
                { height = Length.Percent(100), alignItems = Align.Center, maxWidth = 20 }
            });
            UXBuilder.Div(dragLine, new UXBuilderDivStruct()
            {
                style = new UXStyle()
                {
                    height = Length.Percent(100),
                    width = 1,
                    backgroundColor = Color.black
                }
            });

            rightContainer = UXBuilder.Col(row, new UXBuilderColStruct()
            {
                span = 18,
                style = new UXStyle() { height = Length.Percent(100) }
            });


            widgetScroll = UXBuilder.ScrollView(rightContainer,
                new UXBuilderScrollViewStruct() { style = new UXStyle() { marginTop = 20, whiteSpace = WhiteSpace.NoWrap } });
            var ve = widgetScroll.contentContainer;
            ve.style.flexDirection = FlexDirection.Row;
            ve.style.flexWrap = Wrap.Wrap;

            var selected = component.defaultTitle == "" && component.titles.Count != 0
                ? component.titles[0]
                : component.defaultTitle;
            DrawTab(component, selected);
            if (component.titles.Contains(selected))
                component.onTabClick(selected, rightContainer);

        }

        private static void DrawElementTop(UXBuilderTabsStruct component)
        {

        }

        private static void DrawTab(UXBuilderTabsStruct component, string selectedTitle)
        {
            for (int i = 0; i < component.titles.Count; i++)
            {
                var tmp = component.titles[i];
                var str = tmp;
                // if (tmp.Length > 7)
                // {
                //     str = str.Substring(0, 7) + "...";
                // }

                var content = UXBuilder.Div(labelScroll, new UXBuilderDivStruct()
                {
                    style = new UXStyle()
                    {
                        width = Length.Percent(85),
                        maxWidth = 170,
                        height = 35,
                        // fontSize = 14,
                        marginLeft = Length.Percent(10),
                        marginTop = 10,
                        // overflow = Overflow.Hidden,
                        // textOverflow = TextOverflow.Ellipsis,
                    },
                });
                var btn = UXBuilder.Button(content, new UXBuilderButtonStruct()
                {
                    type = selectedTitle == tmp ? ButtonType.Primary : ButtonType.Default,
                    OnClick = () =>
                    {
                        if (component.titles.Contains(tmp))
                            component.onTabClick(tmp, rightContainer);
                        labelScroll.Clear();
                        DrawTab(component, tmp);
                    },
                    style = new UXStyle()
                    {
                        height = 35,
                        fontSize = 14,
                        overflow = Overflow.Hidden,
#if UNITY_2020_3_OR_NEWER
                        textOverflow = TextOverflow.Ellipsis,
#endif
                    },
                    text = str,
                });
                btn.style.marginLeft = 0;
                btn.style.marginTop = 0;
                btn.style.marginBottom = 0;
                btn.style.marginRight = 0;
                btn.tooltip = tmp;
            }
        }
    }
}
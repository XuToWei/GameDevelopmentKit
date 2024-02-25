using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class UIElementUtils
    {
        public static void TextOverflowWithEllipsis(TextElement label)
        {
            label.RegisterCallback<GeometryChangedEvent, TextElement>(MyOverflowEvent,
                label);
            // label.UnregisterCallback<GeometryChangedEvent, KeyValuePair<TextElement, VisualElement>>(MyOverflowEvent);

        }

        private static void MyOverflowEvent(GeometryChangedEvent evt, TextElement label)
        {
            CutTextElement(label);
            // evt.StopPropagation();
            label.UnregisterCallback<GeometryChangedEvent, TextElement>(MyOverflowEvent);
        }

        public static void CutTextElement(TextElement label)
        {
            var parent = label;
            var scrollView = new ScrollView(){style = { width = 200000, height = 0 }};
            scrollView.style.visibility = Visibility.Hidden;
            parent.Add(scrollView);
            var newLabel = new TextElement
            {
                style =
                {
                    position = Position.Absolute,
                    fontSize = label.style.fontSize,
                    whiteSpace = WhiteSpace.NoWrap,
#if UNITY_2021_2_OR_NEWER
                    letterSpacing = label.style.letterSpacing,
#endif
                    paddingLeft = Length.Percent(0),
                    paddingRight = Length.Percent(0),
                    visibility = Visibility.Hidden
                },
                text = label.text,
            };
            scrollView.Add(newLabel);
            var list = new List<VisualElement>(){label, newLabel, parent, scrollView};
            newLabel.RegisterCallback<GeometryChangedEvent, List<VisualElement>>(MyCutEvent, list);
        }

        private static void MyCutEvent(GeometryChangedEvent evt, List<VisualElement> list)
        {
            AfterNewLabelCreate(list[0] as TextElement, list[1] as TextElement, list[2], list[3]);
            (list[1] as TextElement)?.UnregisterCallback<GeometryChangedEvent, List<VisualElement>>(MyCutEvent);
        }

        private static void AfterNewLabelCreate(TextElement label, TextElement newLabel, VisualElement parent,
            VisualElement scrollView)
        {
            var rate = (label.layout.width - label.resolvedStyle.paddingLeft -
                        label.resolvedStyle.paddingRight - label.resolvedStyle.borderRightWidth -
                        label.resolvedStyle.borderLeftWidth) / newLabel.worldBound.width;
            parent.Remove(scrollView);
            if (double.IsNaN(rate))
            {
                return;
            }

            if (rate >= 1) return;
            var str = label.text;
            var count = str.Length;
            var total = 0;
            foreach (var c in str)
            {
                if (Regex.IsMatch(c.ToString(), @"[\u4e00-\u9fbb]+"))
                    total += 2;
                else total += 1;
            }
            total = (int)(total * rate) - 2;
            var finalStr = "";
            foreach (var c in str)
            {
                if (Regex.IsMatch(c.ToString(), @"[\u4e00-\u9fbb]+"))
                {
                    if (total < 2) break;
                    total -= 2;
                    finalStr += c.ToString();
                }
                else
                {
                    if (total < 1) break;
                    total -= 1;
                    finalStr += c.ToString();
                }
            }
            label.text = finalStr + "...";
        }
    }
}
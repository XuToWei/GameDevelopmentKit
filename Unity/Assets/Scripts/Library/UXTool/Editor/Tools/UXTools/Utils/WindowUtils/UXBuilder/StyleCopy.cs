using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class StyleCopy
    {
        public static void IStyleToUXStyle(UXStyle style, IStyle copiedStyle)
        {
            // if (style == null || copiedStyle == null) return;
            // style.width = copiedStyle.width;
            // style.height = copiedStyle.height;
            // style.maxWidth = copiedStyle.maxWidth;
            // style.maxHeight = copiedStyle.maxHeight;
            // style.minWidth = copiedStyle.minWidth;
            // style.minHeight = copiedStyle.minHeight;
            // style.flexBasis = copiedStyle.flexBasis;
            // style.flexGrow = copiedStyle.flexGrow;
            // style.flexShrink = copiedStyle.flexShrink;
            // style.flexDirection = copiedStyle.flexDirection;
            // style.flexWrap = copiedStyle.flexWrap;
            // style.overflow = copiedStyle.overflow;
            // style.unityOverflowClipBox = copiedStyle.unityOverflowClipBox;
            // style.left = copiedStyle.left;
            // style.right = copiedStyle.right;
            // style.top = copiedStyle.top;
            // style.bottom = copiedStyle.bottom;
            // style.marginLeft = copiedStyle.marginLeft;
            // style.marginBottom = copiedStyle.marginBottom;
            // style.marginRight = copiedStyle.marginRight;
            // style.marginTop = copiedStyle.marginTop;
            // style.paddingLeft = copiedStyle.paddingLeft;
            // style.paddingRight = copiedStyle.paddingRight;
            // style.paddingBottom = copiedStyle.paddingBottom;
            // style.paddingTop = copiedStyle.paddingTop;
            // style.position = copiedStyle.position;
            // style.alignSelf = copiedStyle.alignSelf;
            // style.unityTextAlign = copiedStyle.unityTextAlign;
            // style.unityFontStyleAndWeight = copiedStyle.unityFontStyleAndWeight;
            // style.unityFont = copiedStyle.unityFont;
            // style.fontSize = copiedStyle.fontSize;
            // style.whiteSpace = copiedStyle.whiteSpace;
            // style.color = copiedStyle.color;
            // style.backgroundColor = copiedStyle.backgroundColor;
            // style.backgroundImage = copiedStyle.backgroundImage;
            // style.borderColor = copiedStyle.borderColor;
            // style.unityBackgroundScaleMode = copiedStyle.unityBackgroundScaleMode;
            // style.unityBackgroundImageTintColor = copiedStyle.unityBackgroundImageTintColor;
            // style.alignItems = copiedStyle.alignItems;
            // style.alignContent = copiedStyle.alignContent;
            // style.justifyContent = copiedStyle.justifyContent;
            // style.borderLeftColor = copiedStyle.borderLeftColor;
            // style.borderRightColor = copiedStyle.borderRightColor;
            // style.borderBottomColor = copiedStyle.borderBottomColor;
            // style.borderTopColor = copiedStyle.borderTopColor;
            // style.borderTopWidth = copiedStyle.borderTopWidth;
            // style.borderLeftWidth = copiedStyle.borderLeftWidth;
            // style.borderBottomWidth = copiedStyle.borderBottomWidth;
            // style.borderRightWidth = copiedStyle.borderRightWidth;
            // style.borderTopLeftRadius = copiedStyle.borderTopLeftRadius;
            // style.borderBottomLeftRadius = copiedStyle.borderBottomLeftRadius;
            // style.borderBottomRightRadius = copiedStyle.borderBottomRightRadius;
            // style.borderTopRightRadius = copiedStyle.borderTopRightRadius;
            // style.unitySliceTop = copiedStyle.unitySliceTop;
            // style.unitySliceLeft = copiedStyle.unitySliceLeft;
            // style.unitySliceBottom = copiedStyle.unitySliceBottom;
            // style.unitySliceRight = copiedStyle.unitySliceRight;
            // style.cursor = copiedStyle.cursor;
            // style.opacity = copiedStyle.opacity;
            // style.display = copiedStyle.display;
            // style.visibility = copiedStyle.visibility;
            
            Type type = typeof(IStyle);
            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(style, property.GetValue(copiedStyle));
            }
        }
        
        public static void UXStyleToIStyle(IStyle style, UXStyle copiedStyle)
        {
            Type type = typeof(IStyle);
            PropertyInfo[] properties = type.GetProperties();
            // Type uxType = typeof(UXStyle);
            foreach (var property in properties)
            {
                // var uxProperty = uxType.GetProperty(property.Name);
                property.SetValue(style, property.GetValue(copiedStyle));
            }
        }
    }
}
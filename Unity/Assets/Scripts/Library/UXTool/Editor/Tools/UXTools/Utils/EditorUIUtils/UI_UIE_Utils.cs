using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public static partial class EditorUIUtils
    {
        public static Button CreateUIEButton(string text = "", Image image = null, Action clickAction = null, float width = -1, float height = -1)
        {
            Button btn = new Button();
            btn.style.position = Position.Absolute;
            if (width > 0)
            {
                btn.style.width = width;
            }
            if (height > 0)
            {
                btn.style.height = height;
            }
            btn.text = text;
            btn.tooltip = text;
            if (image != null)
            {
                image.style.alignSelf = Align.Center;
                image.style.top = (btn.style.height.value.value - image.style.height.value.value) / 2 - 2.5f;
                btn.Add(image);
            }
            if (clickAction != null)
            {
                btn.clicked += clickAction;
            }
            return btn;
        }


        //public static VisualElement CreateUIEButton(VisualElement Selected, VisualElement UnSelected, Action clickAction = null)
        //{


        //}


        //public static VisualElement CreateButton(string text = "", Image image = null, Action clickAction = null, float width = -1, float height = -1)
        //{


        //}
        //public static Button CreateUIEButton(string text = "")
        //{
        //    Button btn = new Button();
        //    Debug.Log("111111");
        //    return btn;


        //}

        //public static Button CreateUIEButton(string text = "", GameObject t = null)
        //{
        //    Button btn = new Button();
        //    return btn;
        //}

        //public static Button CreateUIEButton(string text, Vector2 size = Vector2(0,0), Image image = null, Action clickAction = null)
        //{
        //    Button btn = new Button();
        //    btn.style.position = Position.Absolute;
        //    btn.style.width = size.x;
        //    btn.style.height = size.y;
        //    btn.text = text;
        //    btn.tooltip = text;
        //    if (image != null)
        //    {
        //        image.style.alignSelf = Align.Center;
        //        image.style.top = (btn.style.height.value.value - image.style.height.value.value) / 2 - 2.5f;
        //        btn.Add(image);
        //    }
        //    if (clickAction != null)
        //        btn.clicked += clickAction;
        //    return btn;
        //}

    }
}
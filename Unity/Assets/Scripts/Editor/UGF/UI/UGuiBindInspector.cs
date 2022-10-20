using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UGF.Editor
{
    [CustomEditor(typeof(UGuiBind), true)]
    public class UGuiBindInspector : BaseBindInspector
    {
        protected override List<(string, Type)> DefaultBindTypeList => this.m_DefaultBindTypeList;
        protected override List<(string, Type)> CustomBindTypeList => this.m_CustomBindTypeList;

        private readonly List<(string, Type)> m_DefaultBindTypeList = new ()
        {
            ( "Transform", typeof (Transform) ),
            ( "Animation", typeof (Animation) ),
            ( "Animator", typeof (Animator) ),
            ( "RectTransform", typeof (RectTransform) ),
            ( "Canvas", typeof (Canvas) ),
            ( "CanvasGroup", typeof (CanvasGroup) ),
            ( "VerticalLayoutGroup", typeof (VerticalLayoutGroup) ),
            ( "HorizontalLayoutGroup", typeof (HorizontalLayoutGroup) ),
            ( "GridLayoutGroup", typeof (GridLayoutGroup) ),
            ( "ToggleGroup", typeof (ToggleGroup) ),
            ( "Button", typeof (Button) ),
            ( "Image", typeof (Image) ),
            ( "RawImage", typeof (RawImage) ),
            ( "Text", typeof (Text) ),
            ( "InputField", typeof (InputField) ),
            ( "Slider", typeof (Slider) ),
            ( "Mask", typeof (Mask) ),
            ( "RectMask2D", typeof (RectMask2D) ),
            ( "Toggle", typeof (Toggle) ),
            ( "Scrollbar", typeof (Scrollbar) ),
            ( "ScrollRect", typeof (ScrollRect) ),
            ( "Dropdown", typeof (Dropdown) ),
            ( "TMPText", typeof (TMP_Text) ),
            ( "TMPInputField", typeof (TMP_InputField) ),
            ( "TMPProText", typeof (TextMeshProUGUI) ),
        };

        private readonly List<(string, Type)> m_CustomBindTypeList = new()
        {
            ( "Bind", typeof (UGuiBind) ),
        };
    }
}

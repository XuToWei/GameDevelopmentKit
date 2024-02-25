using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ThunderFireUITool;

public class UIGradientItem : VisualElement
{
    //资产块初始大小
    public static int size_w = 90;
    public static int size_h = 30;
    public UIGradient uiColor;
    IMGUIContainer container;
    Label nameLabel;
    public UIGradientItem(UIGradient asset)
    {
        uiColor = asset;
        //style.width = size_w;
        style.height = size_h;
        style.marginTop = 5;
        style.marginRight = 5;
        style.marginLeft = 5;
        style.marginBottom = 5;
        style.alignContent = Align.Center;
        int borderWidth = 1;
        style.borderTopWidth = borderWidth;
        style.borderBottomWidth = borderWidth;
        style.borderLeftWidth = borderWidth;
        style.borderRightWidth = borderWidth;

        IMGUIContainer container = new IMGUIContainer();
        //container.style.width = 90;
        container.style.height = 0;
        container.style.top = 5;
        this.Add(container);
        container.onGUIHandler += OnIMGUI;
        // nameLabel = UXBuilder.Text(this, new UXBuilderTextStruct()
        // {
        //     style = new UXStyle()
        //     {
        //         alignSelf = Align.Center,
        //         fontSize = 12,
        //         color = Color.white,
        //         top = 60,
        //         unityTextAlign = TextAnchor.MiddleCenter,
        //         overflow = Overflow.Hidden,
        //         textOverflow = TextOverflow.Ellipsis,
        //         whiteSpace = WhiteSpace.NoWrap,
        //     },
        //     text = asset.ColorDefName,
        // });

        this.RegisterCallback<MouseEnterEvent>(OnHoverStateChang);
        this.RegisterCallback<MouseLeaveEvent>(OnHoverStateChang);
        //if (isPrefabRecent)
        //
    }

    private void OnIMGUI()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(uiColor.ColorDefName, GUILayout.Width(100));
        //EditorGUI.ColorField(new Rect(10, 5, 70, 50), GUIContent.none, uiColor.colorValue, false, true, false);
        //EditorGUILayout.ColorField(GUIContent.none, uiColor.colorValue, false, true, false, GUILayout.ExpandWidth(true));
        EditorGUILayout.GradientField(GUIContent.none, uiColor.colorValue, false, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        
        //Debug.Log(EditorGUILayout.GetControlRect());
        //EditorGUILayout.ColorField(GUIContent.none,list[i].colorValue,false,true,false);
    }
    private void OnHoverStateChang(EventBase e)
    {
        if (e.eventTypeId == MouseEnterEvent.TypeId())
        {
            style.backgroundColor = new Color(36f / 255f, 99f / 255f, 193f / 255f, 0.5f);
        }
        else if (e.eventTypeId == MouseLeaveEvent.TypeId())
        {
            style.backgroundColor = Color.clear;
        }
    }
    public void Selected(bool ok){
        if(ok){
            Color mycolor = new Color(27f / 255f, 150f / 255f, 233f / 255f, 1f);
            style.borderTopColor = mycolor;
            style.borderBottomColor = mycolor;
            style.borderLeftColor = mycolor;
            style.borderRightColor = mycolor;
            style.backgroundColor = new Color(36f / 255f, 99f / 255f, 193f / 255f, 0.5f);
        }
        else{
            Color mycolor =Color.clear;
            style.borderTopColor = mycolor;
            style.borderBottomColor = mycolor;
            style.borderLeftColor = mycolor;
            style.borderRightColor = mycolor;
            style.backgroundColor = mycolor;
        }
    }
}

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

public class GradientChooseWindow : EditorWindow
{
    public static GradientChooseWindow r_window;
    private UIGradientAsset gradientConfigScriptObject;
    private ScrollView scroll;
    List<UIGradientItem> colors;
    public Gradient selectedColor;
    public UnityEngine.UI.UXImage target;
    public static void OpenWindow()
    {
        int width = 500;
        int height = 220;
        r_window = GetWindow<GradientChooseWindow>();
        r_window.minSize = new Vector2(width, height);
        r_window.titleContent.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_选择渐变预设);
        UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.ColorConfig);
    }

    private void OnEnable()
    {
        InitWindowData();
        InitWindowUI();
    }
    void InitWindowData()
    {

    }
    void InitWindowUI()
    {
        colors = new List<UIGradientItem>();
        gradientConfigScriptObject = JsonAssetManager.GetAssets<UIGradientAsset>();

        VisualElement root = rootVisualElement;
        root.style.alignItems = Align.Stretch;
        root.style.display = DisplayStyle.Flex;

        VisualElement butt = new VisualElement();
        butt.style.position = Position.Relative;
        butt.style.flexGrow = 1;
        butt.style.maxHeight = 30;
        butt.style.minHeight = 30;
        //butt.style.width = scroll.style.width;
        root.Add(butt);
        Button button = new Button();
        butt.Add(button);
        button.style.position = Position.Absolute;
        button.style.top = 5;
        button.style.right = 0;
        button.style.width = 150;
        button.style.height = 20;
        button.text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_打开预设编辑器);
        button.clicked += () =>
        {
            UIColorConfigWindow.ShowObjectWindow();
            UIColorConfigWindow.select = 1;
        };


        scroll = UXBuilder.ScrollView(root, new UXBuilderScrollViewStruct());
        //scroll.style.bottom = 80;
        scroll.style.position = Position.Relative;
        scroll.style.flexGrow = 2;
        var ve = scroll.contentContainer;
        ve.style.flexDirection = FlexDirection.Column;
        ve.style.flexWrap = Wrap.NoWrap;
        ve.style.overflow = Overflow.Hidden;

        VisualElement bot = new VisualElement();
        bot.style.position = Position.Relative;
        bot.style.flexGrow = 1;
        bot.style.maxHeight = 80;
        bot.style.minHeight = 80;
        bot.style.width = scroll.style.width;
        bot.style.alignItems = Align.Center;
        //bot.style.flexDirection = FlexDirection.Column;
        root.Add(bot);

        foreach (UIGradient color in gradientConfigScriptObject.defList)
        {
            UIGradientItem colorItem = new UIGradientItem(color);
            colors.Add(colorItem);
            scroll.Add(colorItem);
        }
        for (int i = 0; i < colors.Count; i++)
        {
            int tmp = i;

            colors[i].RegisterCallback((MouseDownEvent e) =>
            {
                for (int j = 0; j < colors.Count; j++)
                    colors[j].Selected(false);
                colors[tmp].Selected(true);
                selectedColor = colors[tmp].uiColor.colorValue;
                bot.Clear();
                // IMGUIContainer con = new IMGUIContainer();
                // bot.Add(con);
                // bot.style.backgroundColor = new Color(32.0f / 255, 32.0f / 255, 32.0f / 255, 1);
                // con.onGUIHandler += () =>
                // {
                //     EditorGUI.GradientField(new Rect(15, 15, 50, 50), GUIContent.none, selectedColor, false);
                //     EditorGUI.LabelField(new Rect(80, 15, 100, 20), colors[tmp].uiColor.ColorDefName);
                //     EditorGUI.LabelField(new Rect(80, 45, 100, 20), colors[tmp].uiColor.ColorComment);
                // };
                Label deflabel = new Label(colors[tmp].uiColor.ColorDefName);
                deflabel.style.marginBottom = 5;
                deflabel.style.marginTop = 5;
                deflabel.style.fontSize = 16;

                bot.Add(deflabel);
                GradientField colorField = new GradientField();
                colorField.value = selectedColor;
                colorField.SetEnabled(false);
                //colorField.showEyeDropper = false;
                colorField.style.minWidth = 400;
                bot.Add(colorField);
                Label comlabel = new Label(colors[tmp].uiColor.ColorComment);
                comlabel.style.marginBottom = 5;
                comlabel.style.marginTop = 5;
                bot.Add(comlabel);
                bot.style.backgroundColor = new Color(32.0f / 255, 32.0f / 255, 32.0f / 255, 1);

                if (e.clickCount == 2)
                {
                    target.gradient = selectedColor;
                    target.gradient = new Gradient();
                    GradientAlphaKey[] akeys = new GradientAlphaKey[selectedColor.alphaKeys.Length];
                    for (int k = 0; k < selectedColor.alphaKeys.Length; k++)
                    {
                        GradientAlphaKey onekey = new GradientAlphaKey(selectedColor.alphaKeys[k].alpha, selectedColor.alphaKeys[k].time);
                        akeys[k] = onekey;
                    }
                    target.gradient.alphaKeys = akeys;

                    GradientColorKey[] ckeys = new GradientColorKey[selectedColor.colorKeys.Length];
                    for (int k = 0; k < selectedColor.colorKeys.Length; k++)
                    {
                        GradientColorKey twokey = new GradientColorKey(selectedColor.colorKeys[k].color, selectedColor.colorKeys[k].time);
                        ckeys[k] = twokey;
                    }
                    target.gradient.colorKeys = ckeys;
                    target.gradient.mode = selectedColor.mode;
                    EditorUtility.SetDirty(target);
                    this.Close();
                }
            });
        }



    }
    public void Refresh()
    {
        rootVisualElement.Clear();
        InitWindowUI();
    }
}

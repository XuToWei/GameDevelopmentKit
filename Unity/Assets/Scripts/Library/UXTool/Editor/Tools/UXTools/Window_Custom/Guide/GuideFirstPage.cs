using System.Collections.Generic;
using JetBrains.Annotations;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GuideFirstPage : VisualElement
{
    public GuideFirstPage(VisualElement parent)
    {
        style.width = 1120;
        style.height = 500;
        var rowLanguage = UXBuilder.Row(this, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 70 }
        });

        DrawRadios(rowLanguage);

        var rowContent = UXBuilder.Row(this, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.Center,
            style = new UXStyle() { height = 391 }
        });

        var rowBottom = UXBuilder.Row(this, new UXBuilderRowStruct()
        {
            align = Align.Center,
            justify = Justify.FlexEnd,
            style = new UXStyle() { height = 39 }
        });

        var image = new Image()
        { style = { width = 952, height = 391 } };
        var localizationSettings = JsonAssetManager.GetAssets<EditorLocalizationSettings>();

        var language = localizationSettings.LocalType.ToString();
        image.image = AssetDatabase.LoadAssetAtPath<Texture>($"{ThunderFireUIToolConfig.IconPath}ToolGuide_{language}.png");
        image.scaleMode = ScaleMode.ScaleToFit;
        rowContent.Add(image);

        UXBuilder.Button(rowBottom, new UXBuilderButtonStruct()
        {
            type = ButtonType.Text,
            text = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_下一步设置功能开关),
            style = new UXStyle() { fontSize = 16, unityFontStyleAndWeight = FontStyle.Italic, marginRight = 17 },
            OnClick = () =>
            {
                GuideWindow.pageNum = 1;
                GuideWindow.GetInstance().DrawPage(parent);
            }
        });
    }

    private void DrawRadios(VisualElement veParent)
    {
        // var radios = new RadioButtonGroup(null, new List<string>() { "简体中文", "繁體中文", "English", "日本語", "한국어" });
        var localizationSettings = JsonAssetManager.GetAssets<EditorLocalizationSettings>();

        var language = localizationSettings.LocalType;
        localizationSettings.ChangeLocalValue(language);

        AddToggle(veParent, "简体中文",
            language == EditorLocalName.Chinese,
            EditorLocalName.Chinese);
        AddToggle(veParent, "繁體中文", language == EditorLocalName.TraditionalChinese, EditorLocalName.TraditionalChinese);
        AddToggle(veParent, "English", language == EditorLocalName.English, EditorLocalName.English);
        AddToggle(veParent, "日本語", language == EditorLocalName.Japanese, EditorLocalName.Japanese);
        AddToggle(veParent, "한국어", language == EditorLocalName.Korean, EditorLocalName.Korean);
    }

    void AddToggle(VisualElement veParent, string veName, bool toggleValue, EditorLocalName index)
    {
        var toggle = new Toggle
        {
            value = toggleValue,
            style = { marginTop = Length.Percent(0), marginBottom = Length.Percent(0) },
            name = veName
        };
        toggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
        {
            if (evt.newValue)
            {
                var localizationSettings = //EditorLocalizationSettings.GetAssets();
                JsonAssetManager.GetAssets<EditorLocalizationSettings>();
                localizationSettings.ChangeLocalValue(index);

                GuideWindow.pageNum = 0;
                GuideWindow.GetInstance().DrawPage(parent);
            }
        });
        veParent.Add(toggle);
        var label = new Label(veName) { style = { fontSize = 14, marginLeft = 5, marginRight = 25 } };
        veParent.Add(label);
    }
}
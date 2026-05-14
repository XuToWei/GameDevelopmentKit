
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextBgStyle
{
    Default,
    WithTitle,
}

[System.Serializable]
public class GuideTextData : GuideWidgetData
{
    //引导面板显示文字
    public string guideTextContent;
    public string guideTextTitle;

    public TextBgStyle textBgStyle;

    public override string Serialize()
    {
        UpdateTransformData();
        GuideText guideText = GetComponent<GuideText>();
        if (textBgStyle == TextBgStyle.Default)
        {
            guideTextContent = guideText.defaultContent.GetComponent<Text>().text;
        }
        else
        {
            guideTextTitle = guideText.withTitleTitle.GetComponent<Text>().text;
            guideTextContent = guideText.withTitleContent.GetComponent<Text>().text;
        }

        string data = JsonUtility.ToJson(this);
        return data;
    }
}


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
        if (textBgStyle == TextBgStyle.Default)
        {
            guideTextContent = GetComponent<GuideText>().defaultContent.GetComponent<Text>().text;
        }
        else
        {
            guideTextTitle = GetComponent<GuideText>().withTitleTitle.GetComponent<Text>().text;
            guideTextContent = GetComponent<GuideText>().withTitleContent.GetComponent<Text>().text;
        }
        
        string data = JsonUtility.ToJson(this);
        return data;
    }
}

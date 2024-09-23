using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideText : GuideWidgetBase
{
    public GameObject defaultStyle;
    public GameObject withTitleStyle;
    public GameObject defaultContent;
    public GameObject withTitleTitle;
    public GameObject withTitleContent;

    public override void Init(GuideWidgetData data)
    {
        GuideTextData guideTextData = data as GuideTextData;

        if (guideTextData != null)
        {
            guideTextData.ApplyTransformData(transform);

            defaultStyle.SetActive(guideTextData.textBgStyle == TextBgStyle.Default);
            withTitleStyle.SetActive(guideTextData.textBgStyle == TextBgStyle.WithTitle);
            defaultContent.GetComponent<Text>().text = guideTextData.guideTextContent;
            withTitleTitle.GetComponent<Text>().text = guideTextData.guideTextTitle;
            withTitleContent.GetComponent<Text>().text = guideTextData.guideTextContent;
        }
    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();

        return list;
    }

    public override void Show()
    {

    }

    public override void Stop()
    {
    }
}

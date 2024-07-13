using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideSelfDefined : GuideWidgetBase
{
    public override void Init(GuideWidgetData data)
    {
        GuideSelfDefinedData guideSelfDefinedData = data as GuideSelfDefinedData;

        if (guideSelfDefinedData != null)
        {
            gameObject.SetActive(guideSelfDefinedData.active);

            guideSelfDefinedData.ApplyTransformData(transform);

            GetComponent<Text>().text = guideSelfDefinedData.text;

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
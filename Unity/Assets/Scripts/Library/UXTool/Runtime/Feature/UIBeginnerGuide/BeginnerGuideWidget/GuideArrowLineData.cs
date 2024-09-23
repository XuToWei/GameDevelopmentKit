
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuideArrowLineData : GuideWidgetData
{
    public LineType lineType;
    public string smallArrowData;
    public override string Serialize()
    {
        smallArrowData = "";
        if ((int)lineType != 0)
        {
            SmallArrowData arrowdata = transform.GetChild((int)lineType).GetChild(0).GetComponent<SmallArrowData>();
            smallArrowData = arrowdata.Serialize();
        }
        UpdateTransformData();
        string data = JsonUtility.ToJson(this);
        return data;
    }
}

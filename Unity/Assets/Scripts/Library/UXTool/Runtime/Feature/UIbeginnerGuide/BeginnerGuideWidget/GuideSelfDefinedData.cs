using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideSelfDefinedData : GuideWidgetData
{
    public bool active = true;
    public string text = "";
    public string parentPath = "";

    public override string Serialize()
    {
        UpdateTransformData();

        text = GetComponent<Text>().text;

        string data = JsonUtility.ToJson(this);
        return data;
    }
}

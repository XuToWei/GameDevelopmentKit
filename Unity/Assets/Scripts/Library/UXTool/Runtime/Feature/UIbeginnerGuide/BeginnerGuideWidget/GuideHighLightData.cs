
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuideHighLightData : GuideWidgetData
{
    public HighLightType highLightType; 
    public bool isVague;
    public bool UseCustomTarget;
    public Vector3 childPos;
    public Vector3 childScale;
    public Vector3 childRot;
    public Vector2 childSize;
    public RectTransform target;
    
    public override string Serialize()
    {
        childPos = transform.GetChild(0).localPosition;
        childRot = transform.GetChild(0).eulerAngles;
        childScale = transform.GetChild(0).localScale;
        childSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        UpdateTransformData();
        string data = JsonUtility.ToJson(this);
        return data;
        //throw new System.NotImplementedException();
    }
    public void SetTarget(GameObject go)
    {
        target = go.GetComponent<RectTransform>();
    }
}

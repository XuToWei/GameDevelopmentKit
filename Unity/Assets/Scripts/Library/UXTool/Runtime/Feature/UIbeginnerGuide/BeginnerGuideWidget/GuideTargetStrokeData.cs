using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum StrokeType
{
    Square,
    Circle,
}

public enum TargetType
{
    SelfDefined,
    Target,
}

[System.Serializable]
public class GuideTargetStrokeData : GuideWidgetData
{
    public StrokeType strokeType;
    public TargetType targetType;
    public GameObject targetGameObject;
    public bool playAnimator;
    
    public override string Serialize()
    {
        UpdateTransformData();
        // if (targetType == TargetType.Target && targetGameObject != null )
        // {
        //     //Pos = transform.position - targetGameObject.transform.position;

        // }
        string data = JsonUtility.ToJson(this);
        return data;
    }

    public void SetTarget(GameObject go)
    {
        targetGameObject = go;
    }
}

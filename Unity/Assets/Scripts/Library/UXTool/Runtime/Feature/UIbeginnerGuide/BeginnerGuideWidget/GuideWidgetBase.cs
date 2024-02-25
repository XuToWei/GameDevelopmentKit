using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GuideWidgetBase : MonoBehaviour
{
    public abstract void Init(GuideWidgetData data);
    //获取prefab本身自带的，且会发生变化的控件的InstanceID
    public abstract List<int> GetControlledInstanceIds();
    public abstract void Show();
    public abstract void Stop();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreUIAdapter : MonoBehaviour
{
    public UIAdapter UIScreenAdapter;
    RectTransform rectTrans;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    Vector3 GetOffset(float val, Vector3 left, Vector3 right)
    {
        if (val < 0.5f) { return left; }
        if (val > 0.5f) { return right; }
        return Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        Rect safeArea = UIAdapter.GetSafeArea();
        if (safeArea == LastSafeArea) return;
        LastSafeArea = safeArea;

        UnityEngine.Profiling.Profiler.BeginSample($"UXTool UIAdapter Ignore {gameObject.name}");
        rectTrans = GetComponent<RectTransform>();        
        if (UIScreenAdapter == null)
        {
            UIScreenAdapter = GetComponentInParent<UIAdapter>();
        }

        if (UIScreenAdapter == null || rectTrans == null)
        {
            return;
        }

        RectTransform parentTrans = rectTrans.parent as RectTransform;
        if (parentTrans == null)
        {
            return;
        }

        RectTransform safeAreaRectTrans = UIScreenAdapter.GetComponent<RectTransform>();
        if (safeAreaRectTrans == null)
        {
            return;
        }
        RectTransform safeParentRectTrans = safeAreaRectTrans.parent.GetComponent<RectTransform>();
        if (safeParentRectTrans == null)
        {
            return;
        }

        Rect maxRect = safeParentRectTrans.rect;
        Rect curRect = new Rect();

        curRect.xMin = maxRect.xMin * (safeAreaRectTrans.anchorMin.x - 0.5f) / (-0.5f);
        curRect.xMax = maxRect.xMax * (safeAreaRectTrans.anchorMax.x - 0.5f) / (0.5f);
        curRect.yMin = maxRect.yMin * (safeAreaRectTrans.anchorMin.y - 0.5f) / (-0.5f);
        curRect.yMax = maxRect.yMax * (safeAreaRectTrans.anchorMax.y - 0.5f) / (0.5f);

        Vector3 curWorldRightTop = safeParentRectTrans.TransformPoint(new Vector3(curRect.xMax, curRect.yMax));
        Vector3 maxWorldRightTop = safeParentRectTrans.TransformPoint(new Vector3(maxRect.xMax, maxRect.yMax));
        Vector3 curWorldLeftBottom = safeParentRectTrans.TransformPoint(new Vector3(curRect.xMin, curRect.yMin));
        Vector3 maxWorldLeftBottom = safeParentRectTrans.TransformPoint(new Vector3(maxRect.xMin, maxRect.yMin));

        float rightOffset = (maxWorldRightTop - curWorldRightTop).x;
        float leftOffset = (maxWorldLeftBottom - curWorldLeftBottom).x;
        float bottomOffset = (maxWorldLeftBottom - curWorldLeftBottom).y;
        float topOffset = (maxWorldRightTop - curWorldRightTop).y;

        var anchorMin = rectTrans.anchorMin;
        var anchorMax = rectTrans.anchorMax;

        Vector3 curRightTop = parentTrans.TransformPoint(new Vector3(anchorMax.x, anchorMax.y));
        Vector3 curLeftBottom = parentTrans.TransformPoint(new Vector3(anchorMin.x, anchorMin.y));

        curLeftBottom += GetOffset(anchorMin.x, new Vector3(leftOffset, 0, 0), new Vector3(rightOffset, 0, 0));
        curLeftBottom += GetOffset(anchorMin.y, new Vector3(0, bottomOffset, 0), new Vector3(0, topOffset, 0));
        curRightTop += GetOffset(anchorMax.x, new Vector3(leftOffset, 0, 0), new Vector3(rightOffset, 0, 0));
        curRightTop += GetOffset(anchorMax.y, new Vector3(0, bottomOffset, 0), new Vector3(0, topOffset, 0));

        Vector3 newPosMin = parentTrans.InverseTransformPoint(curLeftBottom);
        Vector3 newPosMax = parentTrans.InverseTransformPoint(curRightTop);
        var minOffset = newPosMin - new Vector3(anchorMin.x, anchorMin.y);
        var maxOffset = newPosMax - new Vector3(anchorMax.x, anchorMax.y);
        var anchorMinOffset = new Vector2(minOffset.x / parentTrans.rect.width, minOffset.y / parentTrans.rect.height);
        var anchorMaxOffset = new Vector2(maxOffset.x / parentTrans.rect.width, maxOffset.y / parentTrans.rect.height);

        rectTrans.anchorMin += anchorMinOffset;
        rectTrans.anchorMax += anchorMaxOffset;
        Debug.Log($"UXTool UIAdapter Ignore {gameObject.name} anchor: {rectTrans.anchorMin.x} {rectTrans.anchorMax.x}");
        UnityEngine.Profiling.Profiler.EndSample();
    }
}

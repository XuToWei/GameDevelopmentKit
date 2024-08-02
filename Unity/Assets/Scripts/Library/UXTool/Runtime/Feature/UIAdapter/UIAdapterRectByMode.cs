using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
public class UIAdapterRectByMode : MonoBehaviour
{
    // changed by gdk
    // 编辑器下展示其及其子节点样式
    // private bool _showEffect = false;

#if ODIN_INSPECTOR
    [Button("展示该节点及其子节点PC样式"), ShowIf("@_showEffect == false && !UnityEngine.Application.isPlaying")]
#endif
    private void SetAllPCData()
    {
        // changed by gdk
        //_showEffect = true;
        var ls = GetComponentsInChildren<UIAdapterRectByMode>();
        Array.Reverse(ls);
        foreach (var item in ls)
        {
            if (item.useEffect) item.SetPCData();
            LayoutRebuilder.ForceRebuildLayoutImmediate(item.transform as RectTransform);
        }
    }

#if ODIN_INSPECTOR
    [Button("隐藏该节点及其子节点PC样式"), ShowIf("@_showEffect == true && !UnityEngine.Application.isPlaying")]
#endif
    private void SetAllOriginData()
    {
        // changed by gdk
        //_showEffect = false;
        var ls = GetComponentsInChildren<UIAdapterRectByMode>();
        Array.Reverse(ls);
        foreach (var item in ls)
        {
            if (item.useEffect) item.SetOriginData();
            LayoutRebuilder.ForceRebuildLayoutImmediate(item.transform as RectTransform);
        }
    }

#if ODIN_INSPECTOR
    [Button("记录该节点及其子节点原始数据"), ShowIf("@!UnityEngine.Application.isPlaying")]
#endif
    private void RecordAllOriginData()
    {
        var ls = GetComponentsInChildren<UIAdapterRectByMode>();
        foreach (var item in ls)
            if (item.useEffect) item.RecordOriginDataManual();
    }

#if ODIN_INSPECTOR
    [Button("记录该节点及其子节点PC数据"), ShowIf("@!UnityEngine.Application.isPlaying")]
#endif
    private void RecordAllPCData()
    {
        var ls = GetComponentsInChildren<UIAdapterRectByMode>();
        foreach (var item in ls)
            if (item.useEffect) item.RecordPCDataManual();
    }

    private RectTransform _rectTransform;

#if ODIN_INSPECTOR
    [LabelText("该节点使用组件"), ShowIf("@!UnityEngine.Application.isPlaying")]
#endif
    public bool useEffect = true;

    private void SetPCData()
    {
        // changed by gdk
        //_showEffect = true;
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchorMax = anchorMax;
        _rectTransform.anchorMin = anchorMin;
        _rectTransform.pivot = pivot;
        _rectTransform.localRotation = Quaternion.Euler(rotation);
        _rectTransform.localScale = scale;
        _rectTransform.anchoredPosition = anchoredPosition;
        _rectTransform.sizeDelta = sizeDelta;
    }

    private void SetOriginData()
    {
        // changed by gdk
        //_showEffect = false;
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchorMax = oriAnchorMax;
        _rectTransform.anchorMin = oriAnchorMin;
        _rectTransform.pivot = oriPivot;
        _rectTransform.localRotation = Quaternion.Euler(oriRotation);
        _rectTransform.localScale = oriScale;
        _rectTransform.anchoredPosition = oriAnchoredPosition;
        _rectTransform.sizeDelta = oriSizeDelta;
    }

    private void RecordOriginDataManual()
    {
        RecordOriginData();
        // changed by gdk
        //_showEffect = false;
    }

    private void RecordPCDataManual()
    {
        RecordPCData();
        // changed by gdk
        //_showEffect = true;
    }

    private void Awake()
    {
        if (!useEffect) return;
        _rectTransform = GetComponent<RectTransform>();

        if (_rectTransform == null) return;

        if (!UXPlatform.CheckIsMobile && Application.isPlaying)
        {
            SetPCData();
        }
    }

    private void Reset()
    {
        RecordOriginData();
        RecordPCData();
    }

    private void OnDestroy()
    {
        SetOriginData();
    }

    private void RecordOriginData()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        oriAnchorMax = _rectTransform.anchorMax;
        oriAnchorMin = _rectTransform.anchorMin;
        oriPivot = _rectTransform.pivot;
        oriScale = _rectTransform.localScale;
        oriRotation = _rectTransform.rotation.eulerAngles;
        oriAnchoredPosition = _rectTransform.anchoredPosition;
        oriSizeDelta = _rectTransform.sizeDelta;
    }

    private void RecordPCData()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        anchorMax = _rectTransform.anchorMax;
        anchorMin = _rectTransform.anchorMin;
        pivot = _rectTransform.pivot;
        scale = _rectTransform.localScale;
        rotation = _rectTransform.rotation.eulerAngles;
        anchoredPosition = _rectTransform.anchoredPosition;
        sizeDelta = _rectTransform.sizeDelta;
    }

#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 anchoredPosition = new Vector2(0, 0);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 sizeDelta = new Vector2(0, 0);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 anchorMax = new Vector2(0.5f, 0.5f);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 anchorMin = new Vector2(0.5f, 0.5f);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 pivot = new Vector2(0.5f, 0.5f);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector3 rotation = new Vector3(0, 0, 0);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("PC数据"), ShowIf("@useEffect == true")]
#endif
    public Vector3 scale = new Vector3(1, 1, 1);

#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 oriAnchoredPosition = new Vector2(0, 0);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 oriSizeDelta = new Vector2(0, 0);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 oriAnchorMax = new Vector2(0.5f, 0.5f);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 oriAnchorMin = new Vector2(0.5f, 0.5f);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector2 oriPivot = new Vector2(0.5f, 0.5f);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector3 oriRotation = new Vector3(0, 0, 0);
#if ODIN_INSPECTOR
    [ReadOnly, FoldoutGroup("原始数据"), ShowIf("@useEffect == true")]
#endif
    public Vector3 oriScale = new Vector3(1, 1, 1);
}

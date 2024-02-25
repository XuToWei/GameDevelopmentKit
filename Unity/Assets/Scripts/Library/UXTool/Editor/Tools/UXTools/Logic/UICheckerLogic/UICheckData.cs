#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using TF_TableList;

[Serializable]
public class AtlasResolutionItem : IComparable
{
    [HideInInspector]
    public List<int> vals = new List<int>();
    [HideLabel]
    public string toString = "";
    public void Add(int v1, int v2)
    {
        vals.Add(v1);
        vals.Add(v2);
        toString += "[" + v1 + "X" + v2 + "]";
    }

    public int CompareTo(object obj)
    {
        AtlasResolutionItem other = obj as AtlasResolutionItem;
        if (other == null)
            return 0;
        if (vals.Count != other.vals.Count)
        {
            return vals.Count - other.vals.Count;
        }
        int thisV1 = vals[vals.Count - 2], thisV2 = vals[vals.Count - 1];
        int otherV1 = other.vals[other.vals.Count - 2], otherV2 = other.vals[other.vals.Count - 1];
        if (thisV1 * thisV2 != otherV1 * otherV2)
            return thisV1 * thisV2 - (otherV1 * otherV2);
        return thisV1 - otherV1;
    }
}
[Serializable]
public class WasteMemoryItem : IComparable
{
    [HideInInspector]
    public float val;
    [HideLabel]
    public string toString = "";

    public int CompareTo(object obj)
    {
        WasteMemoryItem other = obj as WasteMemoryItem;
        if (other == null)
            return 0;
        return val.CompareTo(other.val);
    }

    public void SetVal(float val)
    {
        this.val = val;
        toString = val.ToString("0.0") + "KB";
    }
}

public class SpriteAltasFillingInfo
{
    [ReadOnly, TF_TableListColumnName("贴图")]
    public UnityEngine.Object altas = null;
    [ReadOnly, ProgressBar(0, 1, Height = 17), TF_TableListColumnName("图集填充率")]
    public float realFillingRate;
    [ReadOnly, TF_TableListColumnName("图集分辨率")]
    public AtlasResolutionItem atlasResolution = new AtlasResolutionItem();
    [ReadOnly, TF_TableListColumnName("浪费内存")]
    public WasteMemoryItem wasteMemory = new WasteMemoryItem();

    public string getRealFillingRatePerPercentage()
    {
        return Mathf.RoundToInt(realFillingRate * 100).ToString() + "%";
    }
}

#endif
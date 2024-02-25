
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[Serializable]
public class UIValueAdapter : MonoBehaviour
{
#if UNITY_EDITOR
    [Serializable]
    public class EditorInputSlot
    {
        [LabelText("Input"), HorizontalGroup("slot"), OnValueChanged("EditorRefresh")]
        public string name;

        [HideLabel, HorizontalGroup("slot"), OnValueChanged("EditorRefresh")]
        public float value;

        [NonSerialized, HideInInspector]
        public UIValueAdapter adapter;

        void EditorRefresh()
        {
            adapter.EditorRefresh();
        }
    }
    [LabelText("Input Slots"), OnValueChanged("EditorRefresh")]
    public EditorInputSlot[] editorInputSlots;
#endif
    [SerializeField, HideInInspector]
    private string[] inputName;

    [SerializeField, HideInInspector]
    private float[] inputValue;

    [OnValueChanged("EditorRefresh")]
    public UIValueComponent[] components;

    [OnValueChanged("EditorRefresh")]
    public UIStateComponent[] states;




#if UNITY_EDITOR
    [OnInspectorGUI]
    private void EditorRefresh()
    {
        if (editorInputSlots == null || editorInputSlots.Length == 0)
        {
            return;
        }

        foreach (var slot in editorInputSlots)
        {
            slot.adapter = this;
        }

        inputName = new string[editorInputSlots.Length];
        inputValue = new float[editorInputSlots.Length];

        for (int i = 0; i < editorInputSlots.Length; i++)
        {
            inputName[i] = editorInputSlots[i].name;
        }
        for (int i = 0; i < editorInputSlots.Length; i++)
        {
            inputValue[i] = editorInputSlots[i].value;
        }

        foreach (var cp in components)
        {
            cp.EditorSetInputName(inputName);
            cp.SetValue(inputValue);
        }

        foreach (var s in states)
        {
            s.EditorSetInputName(inputName);
            s.SetValue(inputValue);
        }
    }
#endif

    public float GetValue(string key)
    {
        if (inputName == null || components == null)
        {
            return 0;
        }

        var index = inputName.ToList().IndexOf(key);
        if (index == -1)
        {
            Debug.Log($"{key} 不存在，看看是不是手误打错了, 看看和{gameObject.name}中的设置是否一致");
            return 0;
        }

        return inputValue[index];
    }

    public void SetValue(string key, float value, bool refresh_on_no_change = true)
    {
        if (inputName == null || components == null)
        {
            return;
        }

        var index = inputName.ToList().IndexOf(key);
        if (index == -1)
        {
            Debug.Log($"{key} 不存在，看看是不是手误打错了, 看看和{gameObject.name}中的设置是否一致");
            return;
        }

        if (inputValue[index] != value || refresh_on_no_change)
        {
            inputValue[index] = value;
            Refresh();
        }
    }

    public void Refresh()
    {
        if (components != null)
        {
            foreach (var cp in components)
            {
                cp.SetValue(inputValue);
            }
        }
        if (states != null)
        {
            foreach (var s in states)
            {
                s.SetValue(inputValue);
            }
        }
    }
}




//值运算单元，用来预处理程序传入的值
[Serializable]
public class ValueProcess
{
    public enum OperationType
    {
        Add,
        Minus,
        Multiply,
        Divide,
        Mod
    }

#if UNITY_EDITOR
    [HideInInspector]
    public string[] variableNames;

    [SerializeField, HideInInspector]
    public bool showNumA = true;
#endif

    [HorizontalGroup("A")]
    [ShowIf("@this.showNumA")]
    public bool variableA;

    [HorizontalGroup("A")]
    [HideLabel, ShowIf("@this.showNumA && !variableA")]
    public float a;

    [HorizontalGroup("A")]
    [HideLabel, ShowIf("@this.showNumA && variableA"), ValueDropdown("NameIndex")]
    public int indexA;

    public OperationType type;

    [HorizontalGroup("B")]
    public bool variableB;

    [HorizontalGroup("B")]
    [HideLabel, ShowIf("@!this.variableB")]
    public float b;

    [HorizontalGroup("B")]
    [HideLabel, ShowIf("@this.variableB"), ValueDropdown("NameIndex")]
    public int indexB;

    public float GetResult(float[] variableTable)
    {
        if (variableA)
        {
            a = variableTable[indexA];
        }

        if (variableB)
        {
            b = variableTable[indexB];
        }

        switch (type)
        {
            case OperationType.Add:
                return a + b;
            case OperationType.Minus:
                return a - b;
            case OperationType.Multiply:
                return a * b;
            case OperationType.Divide:
                return b == 0 ? 0 : a / b;
            case OperationType.Mod:
                return Mathf.RoundToInt(a) % Mathf.RoundToInt(b);
        }
        return 0;
    }

    public static float ProcessQue(IEnumerable<ValueProcess> que, float[] variableTable)
    {
        float res = 0;
        foreach (var proc in que)
        {
            proc.a = res;
            res = proc.GetResult(variableTable);
        }
        return res;
    }

#if UNITY_EDITOR
    private ValueDropdownList<int> NameIndex()
    {
        if (variableNames == null)
            return null;
        ValueDropdownList<int> valueDropdownItems = new ValueDropdownList<int>();
        for (int i = 0; i < variableNames.Length; i++)
        {
            valueDropdownItems.Add(variableNames[i], i);
        }
        return valueDropdownItems;
    }
#endif
}

public abstract class ValueProcType
{
    [SerializeField, ShowInInspector, OnValueChanged("EditorRefresh")]
    protected ValueProcess[] valueProcesses;

    public virtual void SetValue(float[] variableTable)
    {
        if (valueProcesses == null || valueProcesses.Length == 0)
            return;
        var v = ValueProcess.ProcessQue(valueProcesses, variableTable);
        SetFinalValue(v);
    }

    protected abstract void SetFinalValue(float finalValue);

#if UNITY_EDITOR
    protected string[] editorInputNames;
    public virtual void EditorSetInputName(string[] names)
    {
        editorInputNames = names;
        EditorRefresh();
    }

    public virtual void EditorRefresh()
    {
        if (valueProcesses != null && valueProcesses.Length > 0)
        {
            foreach (var vp in valueProcesses)
            {
                vp.variableNames = editorInputNames;
                vp.showNumA = false;
                vp.variableA = false;
            }
            valueProcesses.First().showNumA = true;
            valueProcesses.First().variableA = true;
        }
    }
#endif
}

//UI响应组件，接受程序输入的值，进行预处理后设置到相应UI组件的相应属性
[Serializable]
public class UIValueComponent : ValueProcType
{
    public enum UITargetType
    {
        RectTransform = 0,
        Image = 1,
        Text = 2
    }

    public enum RectTransformPropertyType
    {
        Scale_X,
        Scale_Y,
        Pos_X,
        Pos_Y,
        Rotation_Z,
        Width,
        Height
    }

    public enum ImagePropertyType
    {
        FillAmount,
        Color_R,
        Color_G,
        Color_B,
        Color_A
    }

    public enum TextPropertyType
    {
        Value_To_Text,
        Color_R,
        Color_G,
        Color_B,
        Color_A,
    }

    [SerializeField, ShowInInspector, OnValueChanged("EditorValidateInput")]
    public GameObject targetObject;

    [ValueDropdown("ValidTypes"), ShowIf("@this.targetObject != null"), OnValueChanged("EditorValidateInput")]
    public UITargetType targetType;

    [ShowIf("@this.targetObject != null && this.targetType == UITargetType.RectTransform")]
    public RectTransformPropertyType transformPropertyType;

    [ShowIf("@this.targetObject != null && this.targetType == UITargetType.Image")]
    public ImagePropertyType imagePropertyType;

    [ShowIf("@this.targetObject != null && this.targetType == UITargetType.Text")]
    public TextPropertyType textPropertyType;

    [ShowIf("@this.targetObject != null && this.targetType == UITargetType.Text")]
    public int intCount = 1;

    [ShowIf("@this.targetObject != null && this.targetType == UITargetType.Text")]
    public int floatCount = 0;

    [SerializeField, HideInInspector]
    private RectTransform rect;
    [SerializeField, HideInInspector]
    private UXImage image;
    [SerializeField, HideInInspector]
    private UXText lxText;
    [SerializeField, HideInInspector]
    private TextMeshPro tmpText;

    protected override void SetFinalValue(float value)
    {
        switch (targetType)
        {
            case UITargetType.RectTransform:
                RectValueModifier(transformPropertyType, value);
                break;
            case UITargetType.Image:
                ImageValueModifier(imagePropertyType, value);
                break;
            case UITargetType.Text:
                TextValueModifier(textPropertyType, value);
                break;
        }
    }

#if UNITY_EDITOR

    private UITargetType[] ValidTypes()
    {
        List<UITargetType> types = new List<UITargetType>();

        types.Add(UITargetType.RectTransform);

        if (targetObject == null)
        {
            types.Add(UITargetType.RectTransform);
            return types.ToArray();
        }

        if (targetObject.GetComponent<UXImage>() != null)
        {
            types.Add(UITargetType.Image);
        }

        if ((targetObject.GetComponent<UXText>() != null || targetObject.GetComponent<TextMeshPro>() != null))
        {
            types.Add(UITargetType.Text);
        }
        return types.ToArray();
    }

    public override void EditorRefresh()
    {
        base.EditorRefresh();
        EditorValidateInput();
    }

    private bool EditorValidateInput()
    {
        if (targetObject == null)
        {
            Reset();
            return false;
        }

        switch (targetType)
        {
            case UITargetType.RectTransform:
                rect = targetObject.GetComponent<RectTransform>();
                if (rect == null)
                {
                    targetObject = null;
                    targetType = UITargetType.RectTransform;
                    return false;
                }
                break;
            case UITargetType.Image:
                image = targetObject.GetComponent<UXImage>();
                if (image == null)
                {
                    Reset();
                    return false;
                }
                break;
            case UITargetType.Text:
                lxText = targetObject.GetComponent<UXText>();
                tmpText = targetObject.GetComponent<TextMeshPro>();
                if (lxText == null && tmpText == null)
                {
                    Reset();
                    return false;
                }
                break;
        }
        return true;
    }

    private void Reset()
    {
        tmpText = null;
        lxText = null;
        image = null;
        rect = null;
        targetType = UITargetType.RectTransform;
    }

#endif

    private void RectValueModifier(RectTransformPropertyType type, float value)
    {
        if (rect == null)
        {
            return;
        }
        switch (type)
        {
            case RectTransformPropertyType.Pos_X:
                rect.localPosition = new Vector3(value, rect.localPosition.y, rect.localPosition.z);
                //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
                break;

            case RectTransformPropertyType.Pos_Y:
                rect.localPosition = new Vector3(rect.localPosition.x, value, rect.localPosition.z);
                //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
                break;

            case RectTransformPropertyType.Width:
                rect.sizeDelta = new Vector2(value, rect.sizeDelta.y);
                //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
                break;
            case RectTransformPropertyType.Height:
                //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, value);
                break;
            case RectTransformPropertyType.Rotation_Z:
                var rot = rect.localRotation.eulerAngles;
                rot.z = value;
                rect.localRotation = Quaternion.Euler(rot);
                break;
            case RectTransformPropertyType.Scale_X:
                var scalex = rect.localScale;
                scalex.x = value;
                rect.localScale = scalex;
                break;
            case RectTransformPropertyType.Scale_Y:
                var scaley = rect.localScale;
                scaley.y = value;
                rect.localScale = scaley;
                break;
        }
    }

    private void ImageValueModifier(ImagePropertyType type, float value)
    {
        if (image == null)
        {
            return;
        }

        switch (type)
        {
            case ImagePropertyType.FillAmount:
                image.fillAmount = value;
                break;
            case ImagePropertyType.Color_R:
                image.color = new Color(value, image.color.g, image.color.b, image.color.a);
                break;
            case ImagePropertyType.Color_G:
                image.color = new Color(image.color.r, value, image.color.b, image.color.a);
                break;
            case ImagePropertyType.Color_B:
                image.color = new Color(image.color.r, image.color.g, value, image.color.a);
                break;
            case ImagePropertyType.Color_A:
                image.color = new Color(image.color.r, image.color.g, image.color.b, value);
                break;
        }
    }

    string GetFormatString(float v)
    {
        string d = new string('0', intCount > 0 ? intCount : 1);
        string f = new string('0', floatCount);
        if (floatCount == 0)
        {
            return string.Format("{0:" + d + "}", v);
        }
        else
        {
            return string.Format("{0:" + d + "." + f + "}", v);
        }
    }

    private void TextValueModifier(TextPropertyType type, float value)
    {
        if (lxText != null)
        {
            switch (type)
            {
                case TextPropertyType.Value_To_Text:
                    lxText.text = GetFormatString(value);
                    break;
                case TextPropertyType.Color_R:
                    lxText.color = new Color(value, lxText.color.g, lxText.color.b, lxText.color.a);
                    break;
                case TextPropertyType.Color_G:
                    lxText.color = new Color(lxText.color.r, value, lxText.color.b, lxText.color.a);
                    break;
                case TextPropertyType.Color_B:
                    lxText.color = new Color(lxText.color.r, lxText.color.g, value, lxText.color.a);
                    break;
                case TextPropertyType.Color_A:
                    lxText.color = new Color(lxText.color.r, lxText.color.g, lxText.color.b, value);
                    break;
            }
        }

        if (tmpText != null)
        {
            switch (type)
            {
                case TextPropertyType.Value_To_Text:
                    tmpText.text = GetFormatString(value);
                    break;
                case TextPropertyType.Color_R:
                    tmpText.color = new Color(value, tmpText.color.g, tmpText.color.b, tmpText.color.a);
                    break;
                case TextPropertyType.Color_G:
                    tmpText.color = new Color(tmpText.color.r, value, tmpText.color.b, tmpText.color.a);
                    break;
                case TextPropertyType.Color_B:
                    tmpText.color = new Color(tmpText.color.r, tmpText.color.g, value, tmpText.color.a);
                    break;
                case TextPropertyType.Color_A:
                    tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, value);
                    break;
            }
        }
    }

}

//动效响应组件，接受程序输入的值，进行预处理后判断当前应该触发哪些动效并执行触发
[Serializable]
public class UIStateComponent : ValueProcType
{
    [Serializable]
    public class StateRange
    {
        [NonSerialized, HideInInspector]
        public UIStateAnimator animator;
        public Vector2 targetRange;
        [ShowIf("@this.animator != null"), ValueDropdown("@this.animator.GetTriggerName()")]
        public string stateName;
#if UNITY_EDITOR
        public string[] stateNames;
#endif
    }

    [OnValueChanged("EditorSetAnimator")]
    public StateRange[] stateRanges;

    [OnValueChanged("EditorSetAnimator")]
    public UIStateAnimator animator;

    private string curState;

    protected override void SetFinalValue(float finalValue)
    {
        foreach (var sr in stateRanges)
        {
            if (finalValue >= sr.targetRange.x && finalValue < sr.targetRange.y && curState != sr.stateName && Application.isPlaying)
            {
                animator?.SetState(sr.stateName);
                curState = sr.stateName;
            }
        }
    }

#if UNITY_EDITOR
    public void EditorSetAnimator()
    {
        if (stateRanges == null) return;
        foreach (var sr in stateRanges)
        {
            if (sr.animator != animator)
            {
                sr.animator = animator;
                sr.stateNames = animator.GetTriggerName();
            }
        }
    }

    public override void EditorRefresh()
    {
        base.EditorRefresh();
        EditorSetAnimator();
    }
#endif
}
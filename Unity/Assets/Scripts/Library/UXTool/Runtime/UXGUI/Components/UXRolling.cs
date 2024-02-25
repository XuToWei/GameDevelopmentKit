using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  enum UXRollingDirection
{
    RollingLeft, 
    RollingRight
}

//这个比较通用, 直接代码里做动画了, 就不搞animation了, 滚动距离在animation里也不是很好判断
public class UXRolling: MonoBehaviour
{
    public bool repeat = true;
    [Range(0.2f, 2f)]
    public float rollingSpeed;

    public GameObject textToRoll;
    public UXRollingDirection rollingDirection = UXRollingDirection.RollingLeft;

    private Rect screen;
    private Vector3 repeatStartPosition;
    private bool rolling = false;
    private float rollingTime = 0f;
    public float RollingTime => rollingTime;

    Vector3[] tmpRect = new Vector3[4];

    private RectTransform textToRollTransform;

    private UXText text;
    private float textWidth;

    void Start()
    {
        //check settings
#if UNITY_EDITOR
        var parentMask = gameObject.GetComponentInParent<RectMask2D>();
        if (parentMask == null)
        {
            Debug.Log("需要一个RectMask2D遮罩裁切文字");
        }
#endif
        if(!rolling)
            ResetScreen();
    }

    float textScaleScreenToWorld = 1;
    public void ResetScreen()
    {
        var parentMask = gameObject.GetComponentInParent<RectMask2D>();
        // Grab the corners of our text rect tranform. 
        parentMask.GetComponent<RectTransform>().GetWorldCorners(tmpRect);
        screen = new Rect(tmpRect[0].x, tmpRect[0].y, tmpRect[2].x - tmpRect[0].x, tmpRect[2].y - tmpRect[0].y);
        textScaleScreenToWorld = screen.width / parentMask.rectTransform.rect.width;

        text = textToRoll.GetComponent<UXText>();
        text.OnTextWidthChanged += OnTextChanged;
        OnTextChanged(text);

        textToRollTransform = textToRoll.GetComponent<RectTransform>();
        var startPosition = (tmpRect[0] + tmpRect[1] + tmpRect[2] + tmpRect[3]) / 4;
        var offset = (tmpRect[3] - tmpRect[0] + new Vector3(textWidth, 0, 0)) / 2;
        if (rollingDirection == UXRollingDirection.RollingLeft)
            repeatStartPosition = startPosition + offset;
        if (rollingDirection == UXRollingDirection.RollingRight)
            repeatStartPosition = startPosition - offset;
        rollingTime = (tmpRect[3].x - tmpRect[0].x + textWidth) / rollingSpeed;
        textToRoll.transform.position = repeatStartPosition;
        rolling = true;
    }

    private void OnDestroy()
    {
        if(text != null)
        {
            text.OnTextWidthChanged -= OnTextChanged;
        }
    }

    void OnTextChanged(UXText fromText)
    {
        textWidth = fromText.DisplayTextPreferredWidth * textScaleScreenToWorld;
    }

    // Update is called once per frame
    void Update()
    {
        // if (textWidth >= screen.width)
        // {
        //     return;
        // }
        //Create an array of four values to store our text corners
        // Grab the corners of our text rect tranform. 
        textToRollTransform.GetWorldCorners(tmpRect);

        // Create a rectangle based on our text to scroll game object
        // the same as we did above
        //Rect rect = new Rect(tmpRect[0].x, tmpRect[0].y, tmpRect[2].x - tmpRect[0].x, tmpRect[2].y - tmpRect[0].y);
        Rect rect = new Rect(tmpRect[0].x, tmpRect[0].y, textWidth, tmpRect[2].y - tmpRect[0].y);


        // Check if it overlaps the canvas rect using the overlap function
        if (rolling || rect.Overlaps(screen))
        {
            Vector3 dir = Vector3.zero;
            if (rollingDirection == UXRollingDirection.RollingLeft)
            {
                dir = Vector3.left;
            }
            
        
            if (rollingDirection == UXRollingDirection.RollingRight)
            {
                dir = Vector3.right;
            }

            textToRollTransform.Translate(dir * (rollingSpeed * Time.deltaTime));
            rolling = !rect.Overlaps(screen);
        }
        else
        {
            if (repeat)
            {
                textToRoll.transform.position = repeatStartPosition;
                rolling = true;
            }
        }
    }
}

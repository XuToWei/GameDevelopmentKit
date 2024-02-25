#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    //CustomTextElement is a custom-built TextElement class with SetTextWithEllipsis
    public class CustomTextElement : TextElement
    {
        public int maxCharactersNum;
        public float maxPixelsNum;

        public string fullText;
        public int bottomShift;
        private string shortText = "";

        public enum CountCharacterMode
        {
            WithNumOfCharacters,
            WithNumOfPixels
        }

        //set text with ellipsis with number of characters
        public void SetTextWithEllipsis(CountCharacterMode countCharacterMode)
        {
            if (!string.IsNullOrEmpty(shortText))
            {
                int textHeight = Mathf.CeilToInt(MeasureTextSize(shortText, maxPixelsNum, MeasureMode.Exactly, 0f, MeasureMode.Undefined).y);
                style.bottom = bottomShift - textHeight;
                text = shortText;
            }
            else
            {
                string updateText = fullText;
                int ellipsisLength = Mathf.CeilToInt(MeasureTextSize("…", maxPixelsNum, MeasureMode.Undefined, 0f, MeasureMode.Undefined).x);
                if (countCharacterMode == CountCharacterMode.WithNumOfCharacters)
                {
                    if (text.Length > maxCharactersNum)
                    {
                        updateText = text.Substring(0, maxCharactersNum - 1);
                        updateText += "…";
                    }
                }
                else
                {
                    text = fullText;
                    if (Mathf.CeilToInt(MeasureTextSize(updateText, 0f, MeasureMode.Undefined, 0f, MeasureMode.Undefined).x) > maxPixelsNum)
                    {
                        int textLength, charIndex;
                        for (charIndex = 1; charIndex <= fullText.Length; charIndex++)
                        {
                            textLength = Mathf.CeilToInt(MeasureTextSize(fullText.Substring(0, charIndex), 0f, MeasureMode.Undefined, 0f, MeasureMode.Undefined).x);
                            if (textLength + ellipsisLength >= maxPixelsNum) { break; }
                        }
                        updateText = fullText.Substring(0, charIndex - 1);
                        updateText += "…";
                    }
                }
                int textHeight = Mathf.CeilToInt(MeasureTextSize(updateText, maxPixelsNum, MeasureMode.Exactly, 0f, MeasureMode.Undefined).y);
                style.bottom = bottomShift - textHeight;
                text = updateText;
                //shortText = updateText;
            }
        }

        public void SetWithFullText()
        {
            //Debug.Log(fullText);
            int textHeight = Mathf.CeilToInt(MeasureTextSize(fullText, maxPixelsNum, MeasureMode.Exactly, 0f, MeasureMode.Undefined).y);
            style.bottom = bottomShift - textHeight;
            text = fullText;
        }


    }
}
#endif
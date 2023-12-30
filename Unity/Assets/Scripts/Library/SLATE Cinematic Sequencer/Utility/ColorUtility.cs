using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slate
{

    public static class ColorUtility
    {

        ///<summary>A greyscale color</summary>
        public static Color Grey(float value) {
            return new Color(value, value, value);
        }

        ///<summary>The color, with alpha</summary>
        public static Color WithAlpha(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }
    }
}
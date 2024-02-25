using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderFireUnityEx
{
    public static class ArithmeticExt
    {
        public static bool ENABLE_EXCEPTION = false;
        public static float SafeDivide(this float a, float b)
        {
            if (b == 0)
            {
#if UNITY_EDITOR
                if (ENABLE_EXCEPTION)
                {
                    throw new System.InvalidOperationException("除0了！");
                }
#endif
                return float.Epsilon;
            }
            return a / b;
        }

        public static Vector3 SafeDivide(this Vector3 vec, float b)
        {
            if (b == 0)
            {
#if UNITY_EDITOR
                if (ENABLE_EXCEPTION)
                {
                    throw new System.InvalidOperationException("除0了！");
                }
#endif
                return Vector3.one * Vector3.kEpsilon;
            }
            return vec / b;
        }

        public static bool HasAnyNaN(this Vector3 vec)
        {
            return float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z);
        }

        public static bool HasAnyInf(this Vector3 vec)
        {
            return float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z);
        }
    }
}

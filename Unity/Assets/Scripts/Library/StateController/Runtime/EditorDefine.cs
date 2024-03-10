#if UNITY_EDITOR
using UnityEngine;

namespace StateController
{
    public static class EditorDefine
    {
        public const float Color = 0.75f;
        public static readonly Color RedColor = new Color(0.75f, 0, 0);
    }
}
#endif
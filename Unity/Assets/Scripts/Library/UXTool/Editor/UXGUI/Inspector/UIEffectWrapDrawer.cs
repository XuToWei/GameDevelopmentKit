using ThunderFireUITool;
using UnityEditor;
using ThunderFireUnityEx;

namespace UnityEngine.UI
{
    public class UIEffectWrapDrawer
    {
        public enum EffectDrawerTargetType
        {
            UXImage,
            UXText
        }

        static string LabelStr;
        static string ShadowStr;
        static string OutlineStr;

        static EffectDrawerTargetType targetType;
        static GameObject target;
        static bool hasOutline;
        static bool hasShadow;

        public static void InitEffectDrawer(EffectDrawerTargetType type, GameObject go)
        {
            LabelStr = EditorLocalization.GetLocalization("UXImage", "UXEffect");
            ShadowStr = EditorLocalization.GetLocalization("UXImage", "UXShadow");
            OutlineStr = EditorLocalization.GetLocalization("UXImage", "UXOutline");
            targetType = type;
            target = go;
            hasShadow = go.HasComponent<Shadow>();
            if(targetType == EffectDrawerTargetType.UXImage)
            {
                hasOutline = go.HasComponent<Outline>();
            }
            else if(targetType == EffectDrawerTargetType.UXText)
            {
                hasOutline = go.HasComponent<Outline>();
            }
        }

        public static Rect ClacButtonRect(int index, Rect position)
        {
            var buttonRect = new Rect(position)
            {
                x = position.x + EditorGUIUtility.labelWidth + (index - 1) * (60+ 10) + 2,
                width = 60
            };
            return buttonRect;
        }


        public static void Draw(Rect position)
        {
            var nameRect = new Rect(position)
            {
                width = EditorGUIUtility.labelWidth
            };
            EditorGUI.LabelField(nameRect, LabelStr);

            var outlineRect = ClacButtonRect(1, position);
            EditorGUI.BeginChangeCheck();
            hasOutline = GUI.Toggle(outlineRect, hasOutline, OutlineStr);
            if(EditorGUI.EndChangeCheck())
            {
                if(hasOutline)
                {
                    GenOutLineComponent(target);
                }
                else
                {
                    RemoveOutLineComponent(target);
                }
                
            }
            var shadowRect = ClacButtonRect(2, position);
            EditorGUI.BeginChangeCheck();
            hasShadow = GUI.Toggle(shadowRect, hasShadow, ShadowStr);
            if (EditorGUI.EndChangeCheck())
            {
                if (hasShadow)
                {
                    GenShadowComponent(target);
                }
                else
                {
                    RemoveShadowComponent(target);
                }
            }
        }

        private static void GenOutLineComponent(GameObject target)
        {
            if(targetType == EffectDrawerTargetType.UXImage)
            {
                target.TryAddComponent<Outline>();
            }
            else if(targetType == EffectDrawerTargetType.UXText)
            {
                target.TryAddComponent<Outline>();
            }
        }
        private static void RemoveOutLineComponent(GameObject target)
        {
            Outline s = target.TryGetComponent<Outline>();
            if (s != null)
            {
                Object.DestroyImmediate(s);
            }
        }

        private static void GenShadowComponent(GameObject target)
        {
            target.TryAddComponent<Shadow>();
        }

        private static void RemoveShadowComponent(GameObject target)
        {
            Shadow s = target.TryGetComponent<Shadow>();
            if(s != null)
            {
                Object.DestroyImmediate(s);
            }
        }
    }
}


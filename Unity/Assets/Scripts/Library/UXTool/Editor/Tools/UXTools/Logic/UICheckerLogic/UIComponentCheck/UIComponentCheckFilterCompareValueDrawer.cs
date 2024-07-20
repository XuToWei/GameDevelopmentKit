using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    public static class UIComponentCheckFilterCompareValueDrawer
    {
        public static int FilterMinusBtnWidth = 30;
        public static int FilterLabelWidth = 80;
        public static int FiletrFirstColoumWidth = 250;
        public static int FiletrColoumWidth = 200;
        public static Gradient gradientValue;
        public static Color colorValue = Color.white;
        public static ColorBlock colorBlockValue = new ColorBlock();

        public static Sprite spriteValue;
        public static Material materialValue;
        public static UnityEngine.Object objectValue;
        public static Graphic graphicValue;
        public static Rect rectValue;
        public static RectInt rectIntValue;
        public static Animator animatorValue;
        public static Animation animValue;
        public static MaskableGraphic maskableGraphicValue;
        public static Vector2 vector2Value;
        public static Vector2Int vector2IntValue;
        public static Vector3 vector3Value;
        public static Vector3Int vector3IntValue;
        public static Vector4 vector4Value;

        public static List<int> intListValue = new List<int>();
        public static List<float> floatListValue = new List<float>();
        public static List<Color> colorListValue = new List<Color>();





        public static string DrawFieldValueInput(string compareValue, FieldInfo selectFieldInfo)
        {
            Type fieldType = selectFieldInfo.FieldType;

            if (IsNumbericType(fieldType))
            {
                compareValue = EditorGUILayout.TextField(compareValue, GUILayout.Width(FiletrColoumWidth));
            }
            else
            {
                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type listElementType = fieldType.GetGenericArguments()[0];
                    if (listElementType == typeof(int))
                    {
                        DrawListField(intListValue);
                        compareValue = string.Join(",", intListValue);
                    }
                    else if (listElementType == typeof(float))
                    {
                        DrawListField(floatListValue);
                        compareValue = string.Join(",", floatListValue);
                    }
                    else if (listElementType == typeof(Color))
                    {
                        DrawListField(colorListValue);
                        compareValue = string.Join(",", colorListValue.Select(ColorUtility.ToHtmlStringRGBA));
                    }
                }
                else
                {
                    switch (fieldType)
                    {
                        case Type t when t == typeof(Gradient):
                            if (gradientValue == null)
                            {
                                gradientValue = new Gradient();
                            }
                            gradientValue = EditorGUILayout.GradientField("Gradient", gradientValue, GUILayout.Width(FiletrColoumWidth));
                            compareValue = SerializeGradient(gradientValue);
                            break;

                        case Type t when t == typeof(Color):
                            colorValue = EditorGUILayout.ColorField("Color", colorValue, GUILayout.Width(FiletrColoumWidth));
                            compareValue = colorValue.ToString();
                            break;

                        case Type t when t == typeof(ColorBlock):
                            EditorGUILayout.BeginVertical();
                            colorBlockValue.normalColor = EditorGUILayout.ColorField("Normal", colorBlockValue.normalColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            colorBlockValue.highlightedColor = EditorGUILayout.ColorField("Highlighted", colorBlockValue.highlightedColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            colorBlockValue.pressedColor = EditorGUILayout.ColorField("Pressed", colorBlockValue.pressedColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            colorBlockValue.selectedColor = EditorGUILayout.ColorField("Selected", colorBlockValue.selectedColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            colorBlockValue.disabledColor = EditorGUILayout.ColorField("Disabled", colorBlockValue.disabledColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            colorBlockValue.colorMultiplier = EditorGUILayout.FloatField("Multiplier", colorBlockValue.colorMultiplier, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            colorBlockValue.fadeDuration = EditorGUILayout.FloatField("Fade Duration", colorBlockValue.fadeDuration, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            EditorGUILayout.EndVertical();
                            compareValue = SerializeColorBlock(colorBlockValue);
                            break;

                        case Type t when t == typeof(bool):
                            string[] booleanOption = new string[] { "True", "False" };
                            int booleanIndex = compareValue == "True" ? 0 : 1;
                            booleanIndex = EditorGUILayout.Popup("Value", booleanIndex, booleanOption, GUILayout.Width(FiletrColoumWidth));
                            compareValue = booleanOption[booleanIndex];
                            break;

                        case Type t when t.IsEnum:
                            string[] enumNames = Enum.GetNames(fieldType);
                            int currentIndex = compareValue == null ? 0 : Array.IndexOf(enumNames, compareValue);
                            currentIndex = currentIndex == -1 ? 0 : currentIndex;
                            int newIndex = EditorGUILayout.Popup(selectFieldInfo.Name, currentIndex, enumNames, GUILayout.Width(FiletrColoumWidth));
                            compareValue = enumNames[newIndex];
                            break;

                        case Type t when t == typeof(Sprite):
                            spriteValue = EditorGUILayout.ObjectField("Sprite", spriteValue, typeof(Sprite), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
                            compareValue = spriteValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(spriteValue));
                            break;

                        case Type t when t == typeof(Material):
                            materialValue = EditorGUILayout.ObjectField("Material", materialValue, typeof(Material), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Material;
                            compareValue = materialValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(materialValue));
                            break;

                        case Type t when t == typeof(UnityEngine.Object):
                            objectValue = EditorGUILayout.ObjectField("Object", objectValue, typeof(UnityEngine.Object), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as UnityEngine.Object;
                            compareValue = objectValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(objectValue));
                            break;

                        case Type t when t == typeof(Graphic):
                            graphicValue = EditorGUILayout.ObjectField("Graphic", graphicValue, typeof(Graphic), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Graphic;
                            compareValue = graphicValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(graphicValue));
                            break;

                        case Type t when t == typeof(MaskableGraphic):
                            maskableGraphicValue = EditorGUILayout.ObjectField("Image", maskableGraphicValue, typeof(MaskableGraphic), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as MaskableGraphic;
                            compareValue = maskableGraphicValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(maskableGraphicValue));
                            break;

                        case Type t when t == typeof(Rect):
                            rectValue = EditorGUILayout.RectField("Rect", rectValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            compareValue = rectValue.ToString();
                            break;

                        case Type t when t == typeof(RectInt):
                            rectIntValue = EditorGUILayout.RectIntField("RectInt", rectIntValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            compareValue = rectIntValue.ToString();
                            break;

                        case Type t when t == typeof(Animator):
                            animatorValue = EditorGUILayout.ObjectField("Animator", animatorValue, typeof(Animator), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Animator;
                            compareValue = animatorValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animatorValue));
                            break;

                        case Type t when t == typeof(Animation):
                            animValue = EditorGUILayout.ObjectField("Animation", animValue, typeof(Animation), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Animation;
                            compareValue = animValue == null ? "null" : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animValue));
                            break;

                        case Type t when t == typeof(Vector2):
                            vector2Value = EditorGUILayout.Vector2Field("Vector2", vector2Value, GUILayout.Width(FiletrColoumWidth+ FilterLabelWidth));
                            compareValue = vector2Value.ToString();
                            break;

                        case Type t when t == typeof(Vector2Int):
                            vector2IntValue = EditorGUILayout.Vector2IntField("Vector2Int", vector2IntValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            compareValue = vector2IntValue.ToString();
                            break;

                        case Type t when t == typeof(Vector3):
                            vector3Value = EditorGUILayout.Vector3Field("Vector3", vector3Value, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            compareValue = vector3Value.ToString();
                            break;

                        case Type t when t == typeof(Vector3Int):
                            vector3IntValue = EditorGUILayout.Vector3IntField("Vector3Int", vector3IntValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            compareValue = vector3IntValue.ToString();
                            break;

                        case Type t when t == typeof(Vector4):
                            vector4Value = EditorGUILayout.Vector4Field("Vector4", vector4Value, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                            compareValue = vector4Value.ToString();
                            break;

                        default:
                            compareValue = EditorGUILayout.TextField(compareValue, GUILayout.Width(FiletrColoumWidth));
                            break;
                    }
                }
            }
            return compareValue;
        }

        private static void DrawListField<T>(List<T> list)
        {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (typeof(T) == typeof(int))
                {
                    list[i] = (T)(object)EditorGUILayout.IntField((int)(object)list[i], GUILayout.Width(FiletrColoumWidth));
                }
                else if (typeof(T) == typeof(float))
                {
                    list[i] = (T)(object)EditorGUILayout.FloatField((float)(object)list[i], GUILayout.Width(FiletrColoumWidth));
                }
                else if (typeof(T) == typeof(Color))
                {
                    list[i] = (T)(object)EditorGUILayout.ColorField((Color)(object)list[i], GUILayout.Width(FiletrColoumWidth));
                }

                if (GUILayout.Button("-", GUILayout.Width(FilterMinusBtnWidth)))
                {
                    list.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add", GUILayout.Width(FiletrColoumWidth)))
            {
                if (typeof(T) == typeof(int))
                {
                    list.Add((T)(object)0);
                }
                else if (typeof(T) == typeof(float))
                {
                    list.Add((T)(object)0f);
                }
                else if (typeof(T) == typeof(Color))
                {
                    list.Add((T)(object)Color.white);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static ColorBlock DrawColorBlockField(ColorBlock colorBlock)
        {
            EditorGUILayout.BeginVertical();
            colorBlock.normalColor = EditorGUILayout.ColorField("Normal", colorBlock.normalColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
            colorBlock.highlightedColor = EditorGUILayout.ColorField("Highlighted", colorBlock.highlightedColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
            colorBlock.pressedColor = EditorGUILayout.ColorField("Pressed", colorBlock.pressedColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
            colorBlock.disabledColor = EditorGUILayout.ColorField("Disabled", colorBlock.disabledColor, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
            colorBlock.colorMultiplier = EditorGUILayout.FloatField("Multiplier", colorBlock.colorMultiplier, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
            colorBlock.fadeDuration = EditorGUILayout.FloatField("Fade Duration", colorBlock.fadeDuration, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
            EditorGUILayout.EndVertical();
            return colorBlock;
        }

        public static string SerializeFieldValue(object value, Type fieldType)
        {
            if (value == null) return "null";

            if (fieldType == typeof(Gradient))
            {
                return SerializeGradient(value as Gradient);
            }
            if (fieldType == typeof(Sprite) || fieldType == typeof(Material) || fieldType == typeof(UnityEngine.Object) || fieldType == typeof(Graphic) || fieldType == typeof(Animation))
            {
                string result = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value as UnityEngine.Object));
                return String.IsNullOrEmpty(result) ? "null" : result;
            }
            if (fieldType == typeof(ColorBlock))
            {
                return SerializeColorBlock((ColorBlock)value);
            }
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listType = fieldType.GetGenericArguments()[0];
                if (listType == typeof(int))
                {
                    return string.Join(",", ((List<int>)value).Select(v => v.ToString()));
                }
                if (listType == typeof(float))
                {
                    return string.Join(",", ((List<float>)value).Select(v => v.ToString()));
                }
                if (listType == typeof(Color))
                {
                    return string.Join(",", ((List<Color>)value).Select(v => ColorUtility.ToHtmlStringRGBA(v)));
                }
            }
            return value.ToString();
        }

        private static string SerializeGradient(Gradient gradient)
        {
            var colorKeys = gradient.colorKeys.Select(ck => $"{ColorUtility.ToHtmlStringRGBA(ck.color)}_{ck.time}");
            var alphaKeys = gradient.alphaKeys.Select(ak => $"{ak.alpha}_{ak.time}");
            return string.Join("|", colorKeys) + "#" + string.Join("|", alphaKeys);
        }

        private static string SerializeColorBlock(ColorBlock colorBlock)
        {
            return $"{ColorUtility.ToHtmlStringRGBA(colorBlock.normalColor)},{ColorUtility.ToHtmlStringRGBA(colorBlock.highlightedColor)},{ColorUtility.ToHtmlStringRGBA(colorBlock.pressedColor)},{ColorUtility.ToHtmlStringRGBA(colorBlock.selectedColor)},{ColorUtility.ToHtmlStringRGBA(colorBlock.disabledColor)},{colorBlock.colorMultiplier},{colorBlock.fadeDuration}";
        }

        private static bool IsNumbericType(Type type)
        {
            return type.IsPrimitive && type != typeof(bool) && type != typeof(char);
        }




    }
}
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    public class UIComponentCheckFilterCompareValueDrawer
    {
        public int FilterMinusBtnWidth = 30;
        public int FilterLabelWidth = 80;
        public int FiletrFirstColoumWidth = 250;
        public int FiletrColoumWidth = 200;

        public int intValue;
        public float floatValue;
        public long longValue;
        public double doubleValue;
        public decimal decimalValue;
        public byte byteValue;
        public char charValue;
        public uint uintValue;
        public Gradient gradientValue;
        public Color colorValue = Color.white;
        public ColorBlock colorBlockValue = new ColorBlock();

        public Sprite spriteValue;
        public Material materialValue;
        public UnityEngine.Object objectValue;
        public Graphic graphicValue;
        public Rect rectValue;
        public RectInt rectIntValue;
        public Animator animatorValue;
        public Animation animValue;
        public MaskableGraphic maskableGraphicValue;
        public Vector2 vector2Value;
        public Vector2Int vector2IntValue;
        public Vector3 vector3Value;
        public Vector3Int vector3IntValue;
        public Vector4 vector4Value;
        public object optValue;
        public bool boolValue;
        public string textValue;
        public object result;

        public List<int> intListValue = new List<int>();
        public List<float> floatListValue = new List<float>();
        public List<Color> colorListValue = new List<Color>();





        public object DrawFieldValueInput(object compareValue, FieldInfo selectFieldInfo)
        {
            Type fieldType = selectFieldInfo.FieldType;

            
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listElementType = fieldType.GetGenericArguments()[0];
                if (listElementType == typeof(int))
                {
                    DrawListField(intListValue);
                    compareValue = intListValue;
                }
                else if (listElementType == typeof(float))
                {
                    DrawListField(floatListValue);
                    compareValue = floatListValue;
                }
                else if (listElementType == typeof(Color))
                {
                    DrawListField(colorListValue);
                    compareValue = colorListValue;
                }
            }
            else
            {
                switch (fieldType)
                {
                    case Type t when t == typeof(int):
                        intValue = EditorGUILayout.IntField("int",intValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = intValue;
                        break;

                    case Type t when t == typeof(float):
                        floatValue = EditorGUILayout.FloatField("float", floatValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = floatValue;
                        break;

                    case Type t when t == typeof(long):
                        longValue = EditorGUILayout.LongField("long", longValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = longValue;
                        break;

                    case Type t when t == typeof(double):
                        doubleValue = EditorGUILayout.DoubleField("double",doubleValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = doubleValue;
                        break; 

                    case Type t when t == typeof(decimal):
                        decimalValue = (decimal)EditorGUILayout.DoubleField("decimal",(double)decimalValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = decimalValue;
                        break;

                    case Type t when t == typeof(byte):
                        byteValue = (byte)EditorGUILayout.IntField("Byte",byteValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = byteValue;
                        break;

                    case Type t when t == typeof(char):
                        charValue = EditorGUILayout.TextField("char",charValue.ToString(), GUILayout.Width(FiletrColoumWidth))[0];
                        compareValue = charValue;
                        break;

                    case Type t when t == typeof(uint):
                        uintValue = (uint)EditorGUILayout.LongField("uint",uintValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = uintValue;
                        break;

                    case Type t when t == typeof(Gradient):
                        if (gradientValue == null)
                        {
                            gradientValue = new Gradient();
                        }
                        gradientValue = EditorGUILayout.GradientField("Gradient", gradientValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = gradientValue;
                        break;

                    case Type t when t == typeof(Color):
                        colorValue = EditorGUILayout.ColorField("Color", colorValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = colorValue;
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
                        compareValue = colorBlockValue;
                        break;

                    case Type t when t == typeof(bool):
                        string[] booleanOption = new string[] { "True", "False" };
                        int booleanIndex = boolValue == true ? 0 : 1;
                        // booleanIndex = EditorGUILayout.Popup("Value", booleanIndex, booleanOption, GUILayout.Width(FiletrColoumWidth));
                        booleanIndex = EditorGUILayout.Popup(booleanIndex, booleanOption, GUILayout.Width(FiletrColoumWidth));
                        boolValue = booleanIndex == 0 ? true : false;
                        compareValue = boolValue;
                        break;

                    case Type t when t.IsEnum:
                        string[] enumNames = Enum.GetNames(fieldType);
                        int currentIndex = compareValue == null ? 0 : Array.IndexOf(enumNames, compareValue.ToString());
                        currentIndex = currentIndex == -1 ? 0 : currentIndex;
                        // int newIndex = EditorGUILayout.Popup(selectFieldInfo.Name, currentIndex, enumNames, GUILayout.Width(FiletrColoumWidth));
                        int newIndex = EditorGUILayout.Popup(currentIndex, enumNames, GUILayout.Width(FiletrColoumWidth));
                        optValue = Enum.Parse(fieldType, enumNames[newIndex]);
                        compareValue = optValue;
                        break;

                    case Type t when t == typeof(Sprite):
                        spriteValue = EditorGUILayout.ObjectField("Sprite", spriteValue, typeof(Sprite), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
                        compareValue = spriteValue;
                        break;

                    case Type t when t == typeof(Material):
                        materialValue = EditorGUILayout.ObjectField("Material", materialValue, typeof(Material), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Material;
                        compareValue = materialValue;
                        break;

                    case Type t when t == typeof(UnityEngine.Object):
                        objectValue = EditorGUILayout.ObjectField("Object", objectValue, typeof(UnityEngine.Object), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as UnityEngine.Object;
                        compareValue = objectValue;
                        break;

                    case Type t when t == typeof(Graphic):
                        graphicValue = EditorGUILayout.ObjectField("Graphic", graphicValue, typeof(Graphic), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Graphic;
                        compareValue = graphicValue;
                        break;

                    case Type t when t == typeof(MaskableGraphic):
                        maskableGraphicValue = EditorGUILayout.ObjectField("Image", maskableGraphicValue, typeof(MaskableGraphic), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as MaskableGraphic;
                        compareValue = maskableGraphicValue;
                        break;

                    case Type t when t == typeof(Rect):
                        rectValue = EditorGUILayout.RectField("Rect", rectValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                        compareValue = rectValue;
                        break;

                    case Type t when t == typeof(RectInt):
                        rectIntValue = EditorGUILayout.RectIntField("RectInt", rectIntValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                        compareValue = rectIntValue;
                        break;

                    case Type t when t == typeof(Animator):
                        animatorValue = EditorGUILayout.ObjectField("Animator", animatorValue, typeof(Animator), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Animator;
                        compareValue = animatorValue;
                        break;

                    case Type t when t == typeof(Animation):
                        animValue = EditorGUILayout.ObjectField("Animation", animValue, typeof(Animation), false, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Animation;
                        compareValue = animValue;
                        break;

                    case Type t when t == typeof(Vector2):
                        vector2Value = EditorGUILayout.Vector2Field("Vector2", vector2Value, GUILayout.Width(FiletrColoumWidth+ FilterLabelWidth));
                        compareValue = vector2Value;
                        break;

                    case Type t when t == typeof(Vector2Int):
                        vector2IntValue = EditorGUILayout.Vector2IntField("Vector2Int", vector2IntValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                        compareValue = vector2IntValue;
                        break;

                    case Type t when t == typeof(Vector3):
                        vector3Value = EditorGUILayout.Vector3Field("Vector3", vector3Value, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                        compareValue = vector3Value;
                        break;

                    case Type t when t == typeof(Vector3Int):
                        vector3IntValue = EditorGUILayout.Vector3IntField("Vector3Int", vector3IntValue, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                        compareValue = vector3IntValue;
                        break;

                    case Type t when t == typeof(Vector4):
                        vector4Value = EditorGUILayout.Vector4Field("Vector4", vector4Value, GUILayout.Width(FiletrColoumWidth + FilterLabelWidth));
                        compareValue = vector4Value;
                        break;

                    default:
                        textValue = EditorGUILayout.TextField(textValue, GUILayout.Width(FiletrColoumWidth));
                        compareValue = textValue;
                        break;
                }
                
            }
            result = compareValue;
            return compareValue;
        }

        private void DrawListField<T>(List<T> list)
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

        private ColorBlock DrawColorBlockField(ColorBlock colorBlock)
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




    }
}
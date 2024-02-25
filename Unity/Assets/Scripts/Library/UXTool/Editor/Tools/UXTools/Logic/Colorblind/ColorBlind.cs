#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

public enum ColorBlindType
{
    Normal,
    Red,
    Green,
    Blue
}

public class ColorBlind : Editor
{
    public static void ToColorBlindMode(ColorBlindType colorType)
    {
        switch(colorType)
        {
            case ColorBlindType.Red:
                ToRed();
                break;
            case ColorBlindType.Green:
                ToGreen();
                break;
            case ColorBlindType.Blue:
                ToBlue();
                break;
            case ColorBlindType.Normal:
                ToNormal();
                break;
            default:
                ToNormal();
                break;
        }
    }

    public static string GetColorLocalizationName(ColorBlindType colorType)
    {
        switch (colorType)
        {
            case ColorBlindType.Red:
                return EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_红色);
            case ColorBlindType.Green:
                return EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_绿色);
            case ColorBlindType.Blue:
                return EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_蓝色);
            case ColorBlindType.Normal:
                return EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_正常);
            default:
                return "";
        }
    }

    public static ColorBlindType GetColorMode(string colorName)
    {
        foreach(ColorBlindType type in ColorBlindType.GetValues(typeof(ColorBlindType)))
        {
            if(colorName == type.ToString())
            {
                return type;
            }
        }
        return ColorBlindType.Normal;
    }

    private static ColorBlindnessEffect AddorGetComponent()
    {
        var effect = Camera.main.GetComponent<ColorBlindnessEffect>();
        if(effect != null) return effect;
        effect = Camera.main.gameObject.AddComponent<ColorBlindnessEffect>();
        effect.colorAlterationShader = Shader.Find("UXTool/Color Blindness Effect");
        return effect;
    }

    //[MenuItem("ThunderFireUXTool/Color/Red")]
    public static void ToRed()
    {
        AddorGetComponent().mode = ColorModification.Protanopia;
        //var m = FindMaterial();
        //m.SetInt("type",1);
    }
    
    //[MenuItem("ThunderFireUXTool/Color/Green")]
    public static void ToGreen()
    {
        AddorGetComponent().mode = ColorModification.Deuteranopia;
        //var m = FindMaterial();
        //m.SetInt("type",2);
    }
    
    //[MenuItem("ThunderFireUXTool/Color/Blue")]
    public static void ToBlue()
    {
        AddorGetComponent().mode = ColorModification.Tritanopia;
        //var m = FindMaterial();
        //m.SetInt("type",3);
    }
    
    //[MenuItem("ThunderFireUXTool/Color/Normal")]
    public static void ToNormal()
    {
        DestroyImmediate(AddorGetComponent());
        //var m = FindMaterial();
        //m.SetInt("type",0);
    }

    private static Material FindMaterial()
    {
        //var m = AssetDatabase.LoadAssetAtPath<Material>(UIConfig.UIDefaultMat);
        //return m;
        return null;
    }
}
#endif
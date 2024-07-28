#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

public class UIColorConfigWindow : EditorWindow
{
    public static int select = 0;
    private string[] names = new string[2];
    private UIColorAsset colorConfigScriptObject;
    private UIGradientAsset gradientConfigScriptObject;
    private Editor colorConfigEditor;
    private Editor gradientConfigEditor;

    private Vector2 scrollPos;

    private string SaveString;

    private string InfoString;

    [MenuItem(ThunderFireUIToolConfig.Menu_UIColor, false, 53)]
    public static void ShowObjectWindow()
    {
        var window = GetWindow<UIColorConfigWindow>(true, EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色预设编辑器), true);
        window.minSize = new Vector2(550, 450);
        select = 0;
        UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.ColorConfig);
    }

    private void OnEnable()
    {
        names[0] = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色);
        names[1] = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_渐变);
        SaveString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_保存);
        InfoString = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色配置命名规定);

        colorConfigScriptObject = JsonAssetManager.GetAssets<UIColorAsset>();//AssetDatabase.LoadAssetAtPath<UIColorAsset>(UIColorConfig.ColorConfigPath + UIColorConfig.ColorConfigName + ".asset");
        gradientConfigScriptObject = JsonAssetManager.GetAssets<UIGradientAsset>();//AssetDatabase.LoadAssetAtPath<UIGradientAsset>(UIColorConfig.ColorConfigPath + UIColorConfig.GradientConfigName + ".asset");

        colorConfigEditor = Editor.CreateEditor(colorConfigScriptObject);
        gradientConfigEditor = Editor.CreateEditor(gradientConfigScriptObject);
    }

    private void OnDestroy()
    {
        DestroyImmediate(colorConfigScriptObject);
        DestroyImmediate(gradientConfigScriptObject);
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        select = GUILayout.Toolbar(select, names, GUILayout.Width(120), GUILayout.Height(25));
        EditorGUILayout.LabelField(InfoString);
        //Debug.Log(select);
        if (select == 0)
        {
            colorConfigEditor.OnInspectorGUI();
        }
        else
        {
            gradientConfigEditor.OnInspectorGUI();
        }
        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button(SaveString))
        {
            if (select == 0)
            {
                SaveColor();
                if (ColorChooseWindow.r_window != null)
                {
                    ColorChooseWindow.r_window.Refresh();
                }
            }
            else
            {
                SaveGradient();
                if (GradientChooseWindow.r_window != null)
                {
                    GradientChooseWindow.r_window.Refresh();
                }
            }

        }
    }

    private void SaveColor()
    {
        var colorNames = new List<string>();
        foreach (var defColor in colorConfigScriptObject.defList)
        {
            colorNames.Add(defColor.ColorDefName);
        }
        if(!NameValid(colorNames)) return;
     

        JsonAssetManager.SaveAssets(colorConfigScriptObject);
        colorConfigScriptObject.GenColorDefScript();
    }

    private void SaveGradient()
    {
        var gradientNames = new List<string>();
        foreach (var defGradient in gradientConfigScriptObject.defList)
        {
            gradientNames.Add(defGradient.ColorDefName);
        }
        if (!NameValid(gradientNames)) return;

        JsonAssetManager.SaveAssets(gradientConfigScriptObject);
        gradientConfigScriptObject.GenGradientScript();
    }

    private bool NameValid(List<string> colorNames)
    {
        var recordNames = new List<string>();
        var invalidNames = "";
        var sameNames = "";
        
        foreach (var colorName in colorNames)
        {
            // 是否空值
            if (string.IsNullOrEmpty(colorName))
            {
                invalidNames += colorName + "\n";
                continue;
            }
            // 是否重复
            if (recordNames.Contains(colorName))
            {
                sameNames += colorName + "\n";
                continue;
            }
            // 是否合法
            if (!Regex.IsMatch(colorName, "^[a-zA-Z0-9_]+$"))
            {
                invalidNames += colorName + "\n";
                continue;
            }
            recordNames.Add(colorName);
        }

        if (string.IsNullOrEmpty(invalidNames) && string.IsNullOrEmpty(sameNames)) return true;

        var info = "";
        if (!string.IsNullOrEmpty(invalidNames))
        {
            info += $"{EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色配置命名非法)}\n{invalidNames}\n";
        }
        if (!string.IsNullOrEmpty(sameNames))
        {
            info += $"{EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_颜色配置命名重复)}\n{sameNames}\n";
        }
        EditorUtility.DisplayDialog("Warning", $"{info}", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
        return false;
    }

}
#endif
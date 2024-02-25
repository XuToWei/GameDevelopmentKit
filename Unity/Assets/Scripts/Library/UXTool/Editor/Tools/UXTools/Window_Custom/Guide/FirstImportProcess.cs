using System;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

[UXInitialize(100)]
public class FirstImportProcess
{
    static FirstImportProcess()
    {
        var firstTime = FirstTimeImport();
        if (!firstTime) return;
        EditorPrefs.SetBool("UXToolGuide", false);

        //UXToolsSwitcher.CreateAllAssets();
        EditorApplication.delayCall += OpenGuideWindow;
    }

    public static bool FirstTimeImport()
    {
        var firstTime = EditorPrefs.GetBool("UXToolGuide", true);
        return firstTime;
    }


    private static void OpenGuideWindow()
    {
        UXToolUsed.InitUXToolUsed();
        GuideWindow.OpenWindow();
    }
}

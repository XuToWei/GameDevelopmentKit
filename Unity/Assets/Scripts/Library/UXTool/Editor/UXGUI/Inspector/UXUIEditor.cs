using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ThunderFireUITool;

public class UXUIEditor : Editor
{
    [MenuItem("GameObject/UI/UXImage")]
    public static void CreateUXImage(MenuCommand menuCommand)
    {
        Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
        Utils.InvokeMethod(MenuOptionsType, "AddImage", new object[] { menuCommand });
        GameObject obj = Selection.activeGameObject;
        obj.name = "UXImage";
        DestroyImmediate(obj.GetComponent<Image>());
        var image = obj.AddComponent<UXImage>();
        //image.material = AssetDatabase.LoadAssetAtPath<Material>(UXGUIConfig.UXImageDefaultMatPath);
    }


    [MenuItem("GameObject/UI/UXText")]
    public static void CreateUXText(MenuCommand menuCommand)
    {
        Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
        Utils.InvokeMethod(MenuOptionsType, "AddText", new object[] { menuCommand });
        GameObject obj = Selection.activeGameObject;
        obj.name = "UXText";
        DestroyImmediate(obj.GetComponent<Text>());
        var text = obj.AddComponent<UXText>();
        //text.material = AssetDatabase.LoadAssetAtPath<Material>(UXGUIConfig.UXTextDefaultMatPath);
    }
    [MenuItem("GameObject/UI/UXTextMeshPro")]
    private static void CreateUXTextMeshPro(MenuCommand menuCommand)
    {
        Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
        Utils.InvokeMethod(MenuOptionsType, "AddText", new object[] { menuCommand });
        GameObject obj = Selection.activeGameObject;
        obj.name = "UXTextMeshPro";
        DestroyImmediate(obj.GetComponent<Text>());
        obj.AddComponent<UXTextMeshPro>();
    }

    [MenuItem("GameObject/UI/UXToggle")]
    public static void CreateUXToggle(MenuCommand menuCommand)
    {
        Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
        Utils.InvokeMethod(MenuOptionsType, "AddToggle", new object[] { menuCommand });
        GameObject obj = Selection.activeGameObject;
        obj.name = "UXToggle";
        DestroyImmediate(obj.GetComponent<Toggle>());
        obj.AddComponent<UXToggle>();
    }

    [MenuItem("GameObject/UI/UXScrollView")]
    public static void CreateUXScrollView(MenuCommand menuCommand)
    {
        Type MenuOptionsType = typeof(UnityEditor.UI.ImageEditor).Assembly.GetType("UnityEditor.UI.MenuOptions");
        Utils.InvokeMethod(MenuOptionsType, "AddScrollView", new object[] { menuCommand });
        GameObject obj = Selection.activeGameObject;
        obj.name = "UXScrollView";
        DestroyImmediate(obj.GetComponent<ScrollRect>());
        UXScrollRect scroll = obj.AddComponent<UXScrollRect>();
        scroll.content = scroll.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        scroll.viewport = scroll.transform.Find("Viewport").GetComponent<RectTransform>();
        scroll.horizontalScrollbar = scroll.transform.Find("Scrollbar Horizontal").GetComponent<Scrollbar>();
        scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scroll.horizontalScrollbarSpacing = -3;
        scroll.verticalScrollbar = scroll.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
        scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scroll.verticalScrollbarSpacing = -3;
        scroll.content.gameObject.AddComponent<GridLayoutGroup>();
        var contentFitter = scroll.content.gameObject.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}

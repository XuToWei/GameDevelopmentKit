using System;
using System.Collections.Generic;
using ThunderFireUITool;
using UnityEditor;
using UnityEngine;

public class QuickBackgroundData
{
    public List<QuickBackgroundDataSingle> list = new List<QuickBackgroundDataSingle>();

    [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.BackGround + "/QuickBackGoundData", false, -44)]
    public static void Create()
    {
        JsonAssetManager.CreateAssets<QuickBackgroundData>(ThunderFireUIToolConfig.QuickBackgroundDataPath);
    }
}
[Serializable]
public class QuickBackgroundDataSingle
{
    public string name;
    public string guid;
    public QuickBackgroundDetail detail = new QuickBackgroundDetail();

}
[Serializable]
public class QuickBackgroundDetail
{
    public bool isOpen = false;
    public Vector3 position = default;
    public Vector3 rotation = default;
    public Vector3 scale = Vector3.one;
    public Vector2 size = new Vector2(1920, 1080);
    public Color color = Color.white;
    public string spriteId = null;
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUserData : ScriptableObject
{
    //use for QuickBackground
    public int canvasid;
    public bool isOpen;
    public Vector3 position = default;
    public Vector3 rotation = default;
    public Vector3 scale = Vector3.one;
    public Vector2 size = default;
    //Size = GetComponent<RectTransform>().sizeDelta;
    public Color color = Color.white;
    public Sprite sprite = default;

    public string trans;
    public string img;

}

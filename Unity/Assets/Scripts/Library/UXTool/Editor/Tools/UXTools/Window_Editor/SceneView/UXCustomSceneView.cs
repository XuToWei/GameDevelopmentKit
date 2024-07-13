#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace ThunderFireUITool
{

    /// <summary>
    /// 在多窗口下容易引起冲突的OnSceneGui的容器
    /// </summary>
    public class UXCustomSceneView
    {
        static private List<Action<SceneView>> list = new List<Action<SceneView>>();
        static private List<Action<SceneView>> Templist = new List<Action<SceneView>>();

        [InitializeOnLoadMethod]
        static void Init()
        {
            list.Clear();
            Templist.Clear();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static public void AddDelegate(Action<SceneView> method)
        {
            Templist.Add(method);
        }

        static public void RemoveDelegate(Action<SceneView> method)
        {
            var index = Templist.FindIndex(i => i == method);
            if (index >= 0)
            {
                Templist.RemoveAt(index);
            }
        }

        static public void OnSceneGUI(SceneView sceneView)
        {
            foreach (Action<SceneView> method in list)
            {
                method.Invoke(sceneView);
            }
            list.Clear();
            foreach (Action<SceneView> method in Templist)
            {
                list.Add(method);
            }
        }

        static public void ClearDelegate()
        {
            Templist.Clear();
        }
    }
}
#endif

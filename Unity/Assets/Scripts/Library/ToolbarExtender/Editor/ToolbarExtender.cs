using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private static readonly List<(int, Action)> s_LeftToolbarGUI = new List<(int, Action)>();
        private static readonly List<(int, Action)> s_RightToolbarGUI = new List<(int, Action)>();

        public static void AddLeftToolbarGUI(int priority, Action onGUIAction)
        {
            s_LeftToolbarGUI.Add((priority, onGUIAction));
            s_LeftToolbarGUI.Sort((tuple1, tuple2) => tuple1.Item1 - tuple2.Item1);
        }

        public static void AddRightToolbarGUI(int priority, Action onGUIAction)
        {
            s_RightToolbarGUI.Add((priority, onGUIAction));
            s_RightToolbarGUI.Sort((tuple1, tuple2) => tuple2.Item1 - tuple1.Item1);
        }

        static ToolbarExtender()
        {
            ToolbarCallback.OnToolbarGUILeft = GUILeft;
            ToolbarCallback.OnToolbarGUIRight = GUIRight;
        }

        private static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            foreach (var handler in s_LeftToolbarGUI)
            {
                handler.Item2();
            }

            GUILayout.EndHorizontal();
        }

        private static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in s_RightToolbarGUI)
            {
                handler.Item2();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [CustomEditor(typeof(GuideHighLight))]
    public class GuideHighLightEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            GUI.enabled=false;
            base.OnInspectorGUI();
            GUI.enabled=true;

        }
    }
}
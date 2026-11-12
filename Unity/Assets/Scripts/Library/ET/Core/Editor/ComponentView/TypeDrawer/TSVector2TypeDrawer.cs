using System;
using TrueSync;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    [TypeDrawer]
    public class TSVector2TypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (TSVector2);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            TSVector2 vector = (TSVector2)value;
            EditorGUI.BeginChangeCheck();
            Vector2 edited = EditorGUILayout.Vector2Field(memberName, new Vector2(vector.x.AsFloat(), vector.y.AsFloat()));
            return EditorGUI.EndChangeCheck() ? new TSVector2(edited.x, edited.y) : value;
        }
    }
}

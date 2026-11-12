using System;
using TrueSync;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    [TypeDrawer]
    public class TSVector4TypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (TSVector4);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            TSVector4 vector = (TSVector4)value;
            EditorGUI.BeginChangeCheck();
            Vector4 edited = EditorGUILayout.Vector4Field(memberName, new Vector4(vector.x.AsFloat(), vector.y.AsFloat(), vector.z.AsFloat(), vector.w.AsFloat()));
            return EditorGUI.EndChangeCheck() ? new TSVector4(edited.x, edited.y, edited.z, edited.w) : value;
        }
    }
}

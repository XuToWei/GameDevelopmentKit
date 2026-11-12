using System;
using TrueSync;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    [TypeDrawer]
    public class TSVectorTypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (TSVector);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            TSVector vector = (TSVector)value;
            EditorGUI.BeginChangeCheck();
            Vector3 edited = EditorGUILayout.Vector3Field(memberName, new Vector3(vector.x.AsFloat(), vector.y.AsFloat(), vector.z.AsFloat()));
            return EditorGUI.EndChangeCheck() ? new TSVector(edited.x, edited.y, edited.z) : value;
        }
    }
}

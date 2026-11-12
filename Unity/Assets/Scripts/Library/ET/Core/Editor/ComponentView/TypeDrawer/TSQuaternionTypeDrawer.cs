using System;
using TrueSync;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    [TypeDrawer]
    public class TSQuaternionTypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (TSQuaternion);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            TSQuaternion rotation = (TSQuaternion)value;
            EditorGUI.BeginChangeCheck();
            Vector4 edited = EditorGUILayout.Vector4Field(memberName, new Vector4(rotation.x.AsFloat(), rotation.y.AsFloat(), rotation.z.AsFloat(), rotation.w.AsFloat()));
            return EditorGUI.EndChangeCheck() ? new TSQuaternion(edited.x, edited.y, edited.z, edited.w) : value;
        }
    }
}

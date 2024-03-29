#if UNITY_ET_VIEW

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    [TypeDrawer]
    public class ISceneTypeDrawer: ITypeDrawer
    {
        public bool HandlesType(Type type)
        {
            return type == typeof (IScene);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            Entity iScene = (Entity)value;
            EditorGUILayout.ObjectField(memberName, iScene.ViewGO, memberType, true);
            return value;
        }
    }
}
#endif
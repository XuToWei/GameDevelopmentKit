using UnityEngine;
using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class OnInspectorGUIAttribute : PropertyAttribute
    {
        public OnInspectorGUIAttribute()
        {

        }
    }
}
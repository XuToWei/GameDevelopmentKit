using UnityEngine;
using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ShowInInspectorAttribute : PropertyAttribute
    {
        public ShowInInspectorAttribute()
        {
        }
    }
}
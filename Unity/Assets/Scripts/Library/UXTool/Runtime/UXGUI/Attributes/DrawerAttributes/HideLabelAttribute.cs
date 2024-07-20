using UnityEngine;
using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HideLabelAttribute : PropertyAttribute
    {
        public HideLabelAttribute()
        {
        }
    }
}

using System;
using UnityEngine;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PropertySpaceAttribute : PropertyAttribute
    {
        public int SpaceBefore;
        public int SpaceAfter;
        public PropertySpaceAttribute()
        {
            this.SpaceBefore = 8;
            this.SpaceAfter = 0;
        }
        public PropertySpaceAttribute(int spaceBefore)
        {
            this.SpaceBefore = spaceBefore;
            this.SpaceAfter = 0;
        }

        public PropertySpaceAttribute(int spaceBefore, int spaceAfter)
        {
            this.SpaceBefore = spaceBefore;
            this.SpaceAfter = spaceAfter;
        }
    }
}

#if UNITY_EDITOR && ODIN_INSPECTOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

namespace TF_TableList
{
    public class DistinctFilter : RowFilter
    {
        private string lable = null;
        [OnInspectorGUI]
        public void Draw()
        {
            if (lable == null)
                lable = $"distinct by {PropertyName}";
            EditorGUILayout.LabelField(lable);
        }

        private HashSet<object> valueCache = new HashSet<object>();
        public override void Prepare(object table)
        {
            valueCache.Clear();
        }

        public override bool Success(object target)
        {
            if (valueCache.Contains(target))
                return false;
            valueCache.Add(target);
            return true;
        }
    }
}
#endif
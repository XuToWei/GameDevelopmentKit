using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor.Tools
{
    public static class PrefabUtil
    {
        public static void RecordPrefabInstancePropertyModificationsFullyRecursive(GameObject objRoot)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(objRoot);
            foreach (Component comp in objRoot.GetComponents<Component>())
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(comp);
            }

            for (int i = 0; i < objRoot.transform.childCount; i++)
            {
                Transform child = objRoot.transform.GetChild(i);
                RecordPrefabInstancePropertyModificationsFullyRecursive(child.gameObject);
            }
        }
    }
}

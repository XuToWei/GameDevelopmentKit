#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;


namespace ThunderFireUITool
{
    public static class WidgetGenerator
    {

        //创建UX UI前都会创建一个GameObject来挂载Component

        private static GameObject CreateUIObjWithParent(string name)
        {
            RectTransform parent;
            bool haveParent = Utils.TryGetSelectionRectTransform(out parent);
            if (haveParent)
            {
                var obj = new GameObject(name);
                obj.layer = LayerMask.NameToLayer("UI");
                var rectTransform = obj.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(200, 200);
                obj.transform.SetParent(parent.transform);
                obj.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                Undo.RegisterCreatedObjectUndo(obj.gameObject, "Create" + obj.name);
                return obj;
            }
            else
            {
                EditorUtility.DisplayDialog("messageBox",
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_请先选择一个父节点),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return null;
            }
        }

        public static GameObject CreateUIObj(string name)
        {
            var obj = new GameObject(name);
            obj.layer = LayerMask.NameToLayer("UI");
            var rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos">LocalPosition</param>
        /// <param name="size"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public static GameObject CreateUIObj(string name, Vector3 pos, Vector3 size, GameObject[] selection)
        {
            name = "UX" + name;
            var obj = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(obj, "");
            obj.layer = LayerMask.NameToLayer("UI");
            Transform parent;
            parent = FindContainerLogic.GetObjectParent(selection);
            Undo.SetTransformParent(obj.transform, parent, "");
            obj.transform.SetParent(parent);

            var rectTransform = Undo.AddComponent<RectTransform>(obj);
            rectTransform.sizeDelta = size;
            obj.transform.localPosition = pos;
            obj.transform.localScale = Vector3.one;
            Undo.SetCurrentGroupName("Create " + name);
            return obj;
        }
    }
}
#endif
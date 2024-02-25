#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    public static class CombineWidgetLogic
    {
        /// <summary>
        /// 组合节点
        /// </summary>
        /// <param name="rects">需要组合的对象 需要所有对象在同一层级</param>
        /// <returns>组合之后的父节点</returns>
        public static GameObject GenCombineRootRect(List<RectTransform> rects)
        {

            float left = rects.GetMinLeft();
            float right = rects.GetMaxRight();
            float top = rects.GetMaxTop();
            float bottom = rects.GetMinBottom();

            float width = right - left;
            float height = top - bottom;

            GameObject root = WidgetGenerator.CreateUIObj("root");
            RectTransform rootRect = root.GetComponent<RectTransform>();

            Transform parent = rects[0].transform.parent;
            root.transform.SetParent(parent);
            rootRect.localScale = Vector3.one;

            rootRect.sizeDelta = new Vector2(width, height);
            rootRect.anchoredPosition = new Vector2(left + width / 2, bottom + height / 2);


            rects.Sort((a, b) =>
            {
                return a.GetSiblingIndex() - b.GetSiblingIndex();
            });

            CombineCommand cmd = new CombineCommand(root, rects.ToArray());
            cmd.Execute();

            return root;
        }
        /// <summary>
        /// 组合节点
        /// </summary>
        /// <param name="rects"></param>
        /// <returns></returns>
        public static GameObject GenCombineRootRect(GameObject[] objs)
        {
            List<RectTransform> rects = objs.ToList().Select(a => a.GetComponent<RectTransform>()).ToList();

            return GenCombineRootRect(rects);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rects"></param>
        /// <returns></returns>
        private static bool AllHaveSameParent(GameObject[] objs)
        {
            if (objs.Length == 1)
            {
                return true;
            }

            Transform parent = objs[0].transform.parent;
            for (int i = 1; i < objs.Length; i++)
            {
                if (objs[i].transform.parent != parent)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AllHaveRectTransform(GameObject[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetComponent<RectTransform>() == null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CanCombine(GameObject[] objs)
        {
            if (objs == null || objs.Length <= 1)
            {
                return false;
            }

            bool SameLevelUI = false;
            if (AllHaveSameParent(objs))
            {
                SameLevelUI = true;
            }

            bool UIInPrefab = false;
            foreach (GameObject go in objs)
            {
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    UIInPrefab = true;
                }
            }

            if (SameLevelUI && !UIInPrefab)
            {
                return true;
            }

            return false;
        }
    }
}
#endif
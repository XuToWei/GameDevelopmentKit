#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    public class FindContainerLogic
    {
        /// <summary>
        /// 只选中一个非 Root的节点时,拖动出来的节点应该和该节点同层级
        /// 未选中或者选中 根Canvas 节点，拖动出来的节点都在 根Canvas 子节点层级
        /// 选中多个时 拖动出来的节点在 根Canvas 子节点层级
        /// </summary>
        public static Transform GetObjectParent(GameObject[] selection)
        {
            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                //Prefab编辑模式下,需要额外区分是否是Canvas (Environment)
                if (selection.Length == 1
                    && !selection[0].name.Equals("Canvas (Environment)")
                    && selection[0].transform != prefabStage.prefabContentsRoot.transform)
                {
                    return selection[0].transform.parent.transform;
                }
                else
                {
                    return prefabStage.prefabContentsRoot.transform;
                }
            }
            else
            {
                if (selection.Length == 1)
                {
                    if (selection[0].transform == selection[0].transform.root)
                    {
                        return selection[0].transform.root;
                    }
                    else
                    {
                        return selection[0].transform.parent.transform;
                    }
                }
                else
                {
                    if (Object.FindObjectsOfType<Canvas>().Length == 0)
                    {
                        new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                    }
                    return Object.FindObjectsOfType<Canvas>()[0].transform;
                }
            }
        }
    }
}
#endif

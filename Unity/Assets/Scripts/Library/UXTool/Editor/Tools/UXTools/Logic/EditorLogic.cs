#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    //UXTools中负责管理常驻Logic的manager,生命周期和SceneViewToolBar相同
    //常驻Logic: 拥有Update功能或者需要存储内存数据的Logic
    public class EditorLogic
    {
        public void Init()
        {
            //Snap Logic
            LocationLineLogic.Instance.Init();
            LocationLine.Init();
            if (!SwitchSetting.CheckValid(SwitchSetting.SwitchType.AlignSnap)) return;
            EdgeSnapLineLogic.Instance.Init();
            IntervalLineLogic.Instance.Init();

            LocationLineLogic.Instance.InitAfter();
            EdgeSnapLineLogic.Instance.InitAfter();
            IntervalLineLogic.Instance.InitAfter();
        }

        public void Close()
        {
            IntervalLineLogic.Instance.CloseBefore();
            EdgeSnapLineLogic.Instance.CloseBefore();
            LocationLineLogic.Instance.CloseBefore();

            LocationLine.Close();
            IntervalLineLogic.Instance.Close();
            EdgeSnapLineLogic.Instance.Close();
            LocationLineLogic.Instance.Close();
        }

        public static bool ObjectFit(GameObject obj)
        {
            if (obj == null) return false;
            //Graphic[] components = obj.GetComponents<Graphic>();
            //if (components == null || components.Length == 0) return false;
            return obj.activeInHierarchy && obj.GetComponent<RectTransform>() != null;
        }
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderFireUnityEx
{
    public static class RectTransformEx
    {
        #region RectTransform
        //获取一个对象的真实中心坐标X(去掉缩放和pivot的影响)
        public static float GetRealPostionX(this RectTransform rect)
        {
            float w = rect.sizeDelta.x * rect.lossyScale.x;//计算实际宽度
            float x = rect.position.x + (0.5f - rect.pivot.x) * w; //消除中心点并非pivot非（0.5，0.5）影响
            return x;
        }


        //获取一个对象的真实中心坐标Y(去掉缩放和pivot的影响)
        public static float GetRealPostionY(this RectTransform rect)
        {
            float h = rect.sizeDelta.y * rect.lossyScale.y;//计算实际高度
            float y = rect.position.y + (0.5f - rect.pivot.y) * h; //消除中心点并非pivot非（0.5，0.5）影响
            return y;
        }


        //获取一个对象的边缘的坐标(去掉缩放和pivot的影响)
        //左边x
        public static float GetLeftWithoutScaleAndPivot(this RectTransform rect)
        {
            float w = rect.rect.width * rect.lossyScale.x;//计算实际宽度
            float x = rect.anchoredPosition.x - rect.pivot.x * w; //消除中心点并非pivot非（0.5，0.5）影响
            return x;
        }

        //右边x
        public static float GetRightWithoutScaleAndPivot(this RectTransform rect)
        {
            float w = rect.rect.width * rect.lossyScale.x;//计算实际宽度
            float x = rect.anchoredPosition.x + (1 - rect.pivot.x) * w; //消除中心点并非pivot非（0.5，0.5）影响
            return x;
        }

        //上边y
        public static float GetTopWithoutScaleAndPivot(this RectTransform rect)
        {
            float h = rect.rect.height * rect.lossyScale.y;//计算实际高度
            float y = rect.anchoredPosition.y + (1 - rect.pivot.y) * h; //消除中心点并非pivot非（0.5，0.5）影响
            return y;
        }

        //下边y
        public static float GetBottomWithoutScaleAndPivot(this RectTransform rect)
        {
            float h = rect.rect.height * rect.lossyScale.y;//计算实际宽度
            float y = rect.anchoredPosition.y - rect.pivot.y * h; //消除中心点并非pivot非（0.5，0.5）影响
            return y;
        }

        //获取一个对象的边缘的世界坐标(去掉缩放和pivot的影响)
        public static float GetLeftWorldPosition(this RectTransform rect)
        {
            return rect.TransformPoint(new Vector3(rect.rect.xMin, 0, 0)).x;
            //TODO 临时解决prefab界面根节点没有parent的问题 之后需要重新修改正确逻辑
            // float w = rect.rect.width * rect.localScale.x; //计算实际宽度
            // float x = rect.anchoredPosition.x - rect.pivot.x * w; //消除中心点并非pivot非（0.5，0.5）影响
            // if (rect.parent != null)
            // {
            //     RectTransform parentRect = rect.parent.GetComponentInParent<RectTransform>();
            //     if (parentRect?.rect == null) return x;
            //     x += parentRect.rect.width * ((rect.anchorMin.x + rect.anchorMax.x) / 2 - 0.5f); //消除Anchor的影响
            //     x += rect.position.x - rect.localPosition.x; //补上本地坐标到世界坐标的差
            //     return x;
            // }
            // else
            // {
            //     return x;
            // }

        }

        //右边x
        public static float GetRightWorldPosition(this RectTransform rect)
        {
            return rect.TransformPoint(new Vector3(rect.rect.xMax, 0, 0)).x;
            // float w = rect.rect.width * rect.localScale.x;//计算实际宽度
            // float x = rect.anchoredPosition.x + (1 - rect.pivot.x) * w; //消除中心点并非pivot非（0.5，0.5）影响
            // if (rect.parent != null)
            // {
            //     RectTransform parentRect = rect.parent.GetComponentInParent<RectTransform>();
            //     if (parentRect?.rect == null) return x;
            //     x += parentRect.rect.width * ((rect.anchorMin.x + rect.anchorMax.x) / 2 - 0.5f); //消除Anchor的影响
            //     x += rect.position.x - rect.localPosition.x; //补上本地坐标到世界坐标的差
            //     return x;
            // }
            // else
            // {
            //     return x;
            // }

        }

        //上边y
        public static float GetTopWorldPosition(this RectTransform rect)
        {
            return rect.TransformPoint(new Vector3(0, rect.rect.yMax, 0)).y;
            // float h = rect.rect.height * rect.localScale.y;//计算实际高度
            // float y = rect.anchoredPosition.y + (1 - rect.pivot.y) * h; //消除中心点并非pivot非（0.5，0.5）影响

            // if (rect.parent != null)
            // {
            //     RectTransform parentRect = rect.parent.GetComponentInParent<RectTransform>();
            //     if (parentRect?.rect == null) return y;
            //     y += parentRect.rect.height * ((rect.anchorMin.y + rect.anchorMax.y) / 2 - 0.5f); //消除Anchor的影响
            //     y += rect.position.y - rect.localPosition.y; //补上本地坐标到世界坐标的差
            //     return y;
            // }
            // else
            // {
            //     return y;
            // }

        }

        //下边y
        public static float GetBottomWorldPosition(this RectTransform rect)
        {
            return rect.TransformPoint(new Vector3(0, rect.rect.yMin, 0)).y;
            // float h = rect.rect.height * rect.localScale.y;//计算实际宽度
            // float y = rect.anchoredPosition.y - rect.pivot.y * h; //消除中心点并非pivot非（0.5，0.5）影响
            // if (rect.parent != null)
            // {
            //     RectTransform parentRect = rect.parent.GetComponentInParent<RectTransform>();
            //     if (parentRect?.rect == null) return y;
            //     y += parentRect.rect.height * ((rect.anchorMin.y + rect.anchorMax.y) / 2 - 0.5f); //消除Anchor的影响
            //     y += rect.position.y - rect.localPosition.y; //补上本地坐标到世界坐标的差
            //     return y;
            // }
            // else
            // {
            //     return y;
            // }

        }
        #endregion

        #region List<RectTransform>
        //获取一组中最左侧边缘的X
        public static float GetMinLeft(this List<RectTransform> rects)
        {
            float minX = GetLeftWithoutScaleAndPivot(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetLeftWithoutScaleAndPivot(rects[i]);
                if (temp < minX)
                    minX = temp;
            }
            return minX;
        }
        //获取一组中最右侧边缘的X
        public static float GetMaxRight(this List<RectTransform> rects)
        {
            float maxX = GetRightWithoutScaleAndPivot(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetRightWithoutScaleAndPivot(rects[i]);
                if (temp > maxX)
                    maxX = temp;
            }
            return maxX;
        }
        //获取一组中最上侧边缘的Y
        public static float GetMaxTop(this List<RectTransform> rects)
        {
            float maxY = GetTopWithoutScaleAndPivot(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetTopWithoutScaleAndPivot(rects[i]);
                if (temp > maxY)
                    maxY = temp;
            }
            return maxY;
        }
        //获取一组中最下侧边缘的Y
        public static float GetMinBottom(this List<RectTransform> rects)
        {
            float minY = GetBottomWithoutScaleAndPivot(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetBottomWithoutScaleAndPivot(rects[i]);
                if (temp < minY)
                    minY = temp;
            }
            return minY;
        }

        //获取一组中最小的中心坐标X
        public static float GetMinX(this List<RectTransform> rects)
        {
            float minX = rects[0].position.x;
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = rects[i].position.x;
                if (temp < minX)
                    minX = temp;
            }
            return minX;
        }

        //获取一组中最小的真实中心坐标X
        public static float GetRealMinX(this List<RectTransform> rects)
        {
            float minX = GetRealPostionX(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetRealPostionX(rects[i]);
                if (temp < minX)
                    minX = temp;
            }
            return minX;
        }

        //获取一组中最大的中心坐标X
        public static float GetMaxX(this List<RectTransform> rects)
        {
            float MaxX = rects[0].position.x;
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = rects[i].position.x;
                if (temp > MaxX)
                    MaxX = temp;
            }
            return MaxX;
        }

        //获取一组中最大的真实中心坐标X
        public static float GetRealMaxX(this List<RectTransform> rects)
        {
            float MaxX = GetRealPostionX(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetRealPostionX(rects[i]);
                if (temp > MaxX)
                    MaxX = temp;
            }
            return MaxX;
        }

        //获取一组中最小的中心坐标Y
        public static float GetMinY(this List<RectTransform> rects)
        {
            float minY = rects[0].position.y;
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = rects[i].position.y;
                if (temp < minY)
                    minY = temp;
            }
            return minY;
        }

        //获取一组中最小的真实中心坐标Y
        public static float GetRealMinY(this List<RectTransform> rects)
        {
            float minY = GetRealPostionY(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetRealPostionY(rects[i]);
                if (temp < minY)
                    minY = temp;
            }
            return minY;
        }

        //获取一组中最大的中心坐标Y
        public static float GetMaxY(this List<RectTransform> rects)
        {
            float maxY = rects[0].position.y;
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = rects[i].position.y;
                if (temp > maxY)
                    maxY = temp;
            }
            return maxY;
        }

        //获取一组中最大的真实中心坐标Y
        public static float GetRealMaxY(this List<RectTransform> rects)
        {
            float MaxY = GetRealPostionY(rects[0]);
            for (int i = 1; i < rects.Count; i++)
            {
                float temp = GetRealPostionY(rects[i]);
                if (temp > MaxY)
                    MaxY = temp;
            }
            return MaxY;
        }
        #endregion
    }
}
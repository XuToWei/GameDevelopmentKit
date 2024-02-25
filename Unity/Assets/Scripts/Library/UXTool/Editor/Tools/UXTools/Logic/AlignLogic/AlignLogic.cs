#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderFireUITool
{
    public enum AlignType
    {
        Top,
        Left,
        Right,
        Bottom,
        Horizontal,
        Vertical
    }

    public enum GridType
    {
        Horizontal,
        Vertical,
        Ring
    }
    public static class AlignLogic
    {
        public static long GetStringKeyByAlignType(AlignType type)
        {
            switch (type)
            {
                case AlignType.Bottom:
                    return EditorLocalizationStorage.Def_下对齐;
                case AlignType.Top:
                    return EditorLocalizationStorage.Def_上对齐;
                case AlignType.Left:
                    return EditorLocalizationStorage.Def_左对齐;
                case AlignType.Right:
                    return EditorLocalizationStorage.Def_右对齐;
                case AlignType.Vertical:
                    return EditorLocalizationStorage.Def_垂直居中对齐;
                case AlignType.Horizontal:
                    return EditorLocalizationStorage.Def_水平居中对齐;
                default:
                    return EditorLocalizationStorage.Def_测试;
            }
        }

        public static long GetStringKeyByGridType(GridType type)
        {
            switch (type)
            {
                case GridType.Vertical:
                    return EditorLocalizationStorage.Def_竖向分布;
                case GridType.Horizontal:
                    return EditorLocalizationStorage.Def_横向分布;
                case GridType.Ring:
                    return EditorLocalizationStorage.Def_环形分布;
                default:
                    return EditorLocalizationStorage.Def_测试;
            }
        }

        public static bool CanAlign(List<RectTransform> rects = null)
        {
            if (rects == null)
                rects = Utils.GetAllSelectionRectTransform();

            //有layout不能对齐
            foreach (var rect in rects)
            {
                LayoutGroup layout = rect.GetComponentInParent<LayoutGroup>();
                if (layout != null)
                {
                    return false;
                }
            }

            //2个以下不能对齐
            if (rects.Count < 2)
            {
                return false;
            }
            return true;
        }

        public static void Align(AlignType type)
        {
            List<RectTransform> rects = Utils.GetAllSelectionRectTransform();
            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.LayoutTools);
            if (CanAlign(rects))
            {
                Align(type, rects);
            }
        }

        static void Align(AlignType type, List<RectTransform> rects)
        {
            AlignCommand cmd = new AlignCommand(rects.ToArray());
            cmd.Execute();
            switch (type)
            {
                case AlignType.Top://上对齐
                    float maxY = GetMaxTop(rects);
                    for (int i = 0; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float th = trans.sizeDelta.y * trans.localScale.y;
                        Vector3 pos = trans.position;
                        pos.y = maxY - th + trans.pivot.y * th;
                        trans.position = pos;
                    }
                    break;
                case AlignType.Left://左对齐
                    float minX = GetMinLeft(rects);
                    for (int i = 0; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float tw = trans.sizeDelta.x * trans.lossyScale.x;
                        Vector3 pos = trans.position;
                        pos.x = minX + tw * trans.pivot.x;
                        trans.position = pos;
                    }
                    break;
                case AlignType.Right://右对齐
                    float maxX = GetMaxRight(rects);
                    for (int i = 0; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float tw = trans.sizeDelta.x * trans.lossyScale.x;
                        Vector3 pos = trans.position;
                        pos.x = maxX - tw + tw * trans.pivot.x;
                        trans.position = pos;
                    }
                    break;
                case AlignType.Bottom://下对齐
                    float minY = GetMinBottom(rects);
                    for (int i = 0; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        float th = trans.sizeDelta.y * trans.localScale.y;
                        Vector3 pos = trans.position;
                        pos.y = minY + th * trans.pivot.y;
                        trans.position = pos;
                    }
                    break;
                case AlignType.Horizontal://水平对齐(根据真实坐标)
                    float Ymax = GetRealMaxY(rects);
                    float Ymin = GetRealMinY(rects);
                    float Ymid = (Ymax + Ymin) / 2;
                    for (int i = 0; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        Vector3 pos = trans.position;
                        pos.y = pos.y + Ymid - GetRealPostionY(rects[i]);
                        trans.position = pos;
                    }
                    break;
                case AlignType.Vertical://垂直对齐(根据真实坐标)
                    float Xmax = GetRealMaxX(rects);
                    float Xmin = GetRealMinX(rects);
                    float Xmid = (Xmax + Xmin) / 2;
                    for (int i = 0; i < rects.Count; i++)
                    {
                        RectTransform trans = rects[i];
                        Vector3 pos = trans.position;
                        pos.x = pos.x + Xmid - GetRealPostionX(rects[i]);
                        trans.position = pos;
                    }
                    break;
            }
        }

        public static bool CanGrid(List<RectTransform> rects = null)
        {
            if (rects == null)
                rects = Utils.GetAllSelectionRectTransform();


            //3个以下不能阵列
            if (rects.Count < 3)
            {
                return false;
            }

            //有layout不能阵列
            foreach (var rect in rects)
            {
                LayoutGroup layout = rect.GetComponentInParent<LayoutGroup>();
                if (layout != null)
                {
                    return false;
                }
            }
            return true;
        }

        //阵列暂时只考虑中心点坐标 不考虑由于缩放、pivot偏移引起的视觉位置变化
        //因为目前感觉需要被阵列的对象大小应该是一致的
        //TODO: 后续根据实际使用反馈情况再进行优化
        public static void Grid(GridType type)
        {
            List<RectTransform> rects = Utils.GetAllSelectionRectTransform();
            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.LayoutTools);
            if (CanGrid(rects))
            {
                Grid(type, rects);
            }
        }

        static void Grid(GridType type, List<RectTransform> rects)
        {
            AlignCommand cmd = new AlignCommand(rects.ToArray());
            cmd.Execute();

            int count = rects.Count;
            switch (type)
            {
                //case GridType.Horizontal:
                //    //水平阵列 一横排
                //    float minX = GetMinX(rects);
                //    float maxX = GetMaxX(rects);
                //    float intervalX = (maxX - minX) / (count - 1);
                //    rects.Sort((a, b) => { return (int)(a.position.x - b.position.x); });
                //    for (int i = 0; i < count; i++)
                //    {
                //        rects[i].position = new Vector3(minX + intervalX * i, rects[i].position.y, rects[i].position.z);
                //    }
                //    break;
                case GridType.Horizontal:
                    //水平阵列 一横排(不受缩放、pivot偏移影响)
                    float minX = GetMinLeft(rects);
                    float maxX = GetMaxRight(rects);
                    float TotalIntervalX = maxX - minX;
                    for (int i = 0; i < count; i++)
                    {
                        TotalIntervalX -= rects[i].sizeDelta.x * rects[i].lossyScale.x;
                    }
                    float intervalX = TotalIntervalX / (count - 1);
                    rects.Sort((a, b) => { return (int)(GetRealPostionX(a) - GetRealPostionX(b)); });
                    float RealPostionX = minX + rects[0].sizeDelta.x * rects[0].lossyScale.x / 2;
                    for (int i = 0; i < count; i++)
                    {
                        rects[i].position = new Vector3(RealPostionX - GetRealPostionX(rects[i]) + rects[i].position.x, rects[i].position.y, rects[i].position.z);
                        if (i + 1 == count) { continue; }
                        RealPostionX += intervalX + rects[i].sizeDelta.x * rects[i].lossyScale.x / 2 + rects[i + 1].sizeDelta.x * rects[i + 1].lossyScale.x / 2;
                    }
                    break;
                //case GridType.Vertical:
                //    //垂直阵列 一竖排
                //    float minY = GetMinY(rects);
                //    float maxY = GetMaxY(rects);
                //    float intervalY = (maxY - minY) / (count - 1);
                //    rects.Sort((a, b) => { return (int)(a.position.y - b.position.y); });
                //    for (int i = 0; i < count; i++)
                //    {
                //        rects[i].position = new Vector3(rects[i].position.x, minY + intervalY * i, rects[i].position.z);
                //    }
                //    break;
                case GridType.Vertical:
                    //垂直阵列 一竖排(不受缩放、pivot偏移影响)
                    float minY = GetMinBottom(rects);
                    float maxY = GetMaxTop(rects);
                    float TotalIntervalY = maxY - minY;
                    for (int i = 0; i < count; i++)
                    {
                        TotalIntervalY -= rects[i].sizeDelta.y * rects[i].lossyScale.y;
                    }
                    float intervalY = TotalIntervalY / (count - 1);
                    rects.Sort((a, b) => { return (int)(GetRealPostionY(a) - GetRealPostionY(b)); });
                    float RealPostionY = minY + rects[0].sizeDelta.y * rects[0].lossyScale.y / 2;
                    for (int i = 0; i < count; i++)
                    {
                        rects[i].position = new Vector3(rects[i].position.x, RealPostionY - GetRealPostionY(rects[i]) + rects[i].position.y, rects[i].position.z);
                        if (i + 1 == count) { continue; }
                        RealPostionY += intervalY + rects[i].sizeDelta.y * rects[i].lossyScale.y / 2 + rects[i + 1].sizeDelta.y * rects[i + 1].lossyScale.y / 2;
                    }
                    break;
                case GridType.Ring:
                    //环形阵列 一圈
                    float left = GetMinX(rects);
                    float right = GetMaxX(rects);
                    float bottom = GetMinY(rects);
                    float top = GetMaxY(rects);

                    float middleX = left + (right - left) / 2;
                    float middleY = left + (top - bottom) / 2;

                    float r = (right - left) / 2;
                    float angle = 360 / count;

                    for (int i = 0; i < count; i++)
                    {
                        float x = middleX + r * Mathf.Cos((90 - angle * i) * Mathf.PI / 180);
                        float y = middleY + r * Mathf.Sin((90 - angle * i) * Mathf.PI / 180);
                        rects[i].position = new Vector3(x, y, rects[i].position.z);
                    }
                    break;
                default:
                    break;
            }
        }


        //获取一个对象的真实中心坐标X(去掉缩放和pivot的影响)
        static float GetRealPostionX(RectTransform rect)
        {
            float w = rect.sizeDelta.x * rect.lossyScale.x;//计算实际宽度
            float x = rect.position.x + (0.5f - rect.pivot.x) * w; //消除中心点并非pivot非（0.5，0.5）影响
            return x;
        }


        //获取一个对象的真实中心坐标Y(去掉缩放和pivot的影响)
        static float GetRealPostionY(RectTransform rect)
        {
            float h = rect.sizeDelta.y * rect.localScale.y;//计算实际高度
            float y = rect.position.y + (0.5f - rect.pivot.y) * h; //消除中心点并非pivot非（0.5，0.5）影响
            return y;
        }


        //获取一个对象的边缘的坐标(去掉缩放和pivot的影响)
        //左边x
        static float GetLeftWithoutScaleAndPivot(RectTransform rect)
        {
            float w = rect.sizeDelta.x * rect.lossyScale.x;//计算实际宽度
            float x = rect.position.x - rect.pivot.x * w; //消除中心点并非pivot非（0.5，0.5）影响
            return x;
        }

        //右边x
        static float GetRightWithoutScaleAndPivot(RectTransform rect)
        {
            float w = rect.sizeDelta.x * rect.lossyScale.x;//计算实际宽度
            float x = rect.position.x + (1 - rect.pivot.x) * w; //消除中心点并非pivot非（0.5，0.5）影响
            return x;
        }

        //上边y
        static float GetTopWithoutScaleAndPivot(RectTransform rect)
        {
            float h = rect.sizeDelta.y * rect.localScale.y;//计算实际高度
            float y = rect.position.y + (1 - rect.pivot.y) * h; //消除中心点并非pivot非（0.5，0.5）影响
            return y;
        }

        //下边y
        static float GetBottomWithoutScaleAndPivot(RectTransform rect)
        {
            float h = rect.sizeDelta.y * rect.localScale.y;//计算实际宽度
            float y = rect.position.y - rect.pivot.y * h; //消除中心点并非pivot非（0.5，0.5）影响
            return y;
        }

        //获取一组中最左侧边缘的X
        static float GetMinLeft(List<RectTransform> rects)
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
        static float GetMaxRight(List<RectTransform> rects)
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
        static float GetMaxTop(List<RectTransform> rects)
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
        static float GetMinBottom(List<RectTransform> rects)
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
        static float GetMinX(List<RectTransform> rects)
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
        static float GetRealMinX(List<RectTransform> rects)
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
        static float GetMaxX(List<RectTransform> rects)
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
        static float GetRealMaxX(List<RectTransform> rects)
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
        static float GetMinY(List<RectTransform> rects)
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
        static float GetRealMinY(List<RectTransform> rects)
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
        static float GetMaxY(List<RectTransform> rects)
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
        static float GetRealMaxY(List<RectTransform> rects)
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

    }
}
#endif
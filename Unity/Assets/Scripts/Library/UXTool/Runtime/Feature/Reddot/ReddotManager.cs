using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ReddotManager
{
    private sealed class ReddotData
    {
        public ReddotData parent { get; private set; }
        private List<ReddotData> m_Children;

        public string key;
        public bool Show { get; private set; }

        public void AddChild(ReddotData reddot)
        {
            if (m_Children == null)
            {
                m_Children = new List<ReddotData>();
            }

            if (m_Children.Contains(reddot)) return;

            if (reddot.parent != null)
            {
                reddot.parent.RemoveChild(reddot);
            }

            m_Children.Add(reddot);
            reddot.parent = this;
        }
        public void RemoveChild(ReddotData reddot)
        {
            if (m_Children == null) return;
            if (m_Children.Remove(reddot))
            {
                reddot.parent = null;
            }
        }
        public bool TryGetChild(string key, out ReddotData child)
        {
            child = null;
            if (m_Children == null) return false;
            child = m_Children.Find(x => x.key == key);
            return child != null;
        }

        /// <summary>
        /// 代码动态设置时只有叶子节点才设置 其他节点直接返回
        /// 递归设置时都设置
        /// </summary>
        /// <param name="shown"></param>
        /// <param name="manual">是否是手动设置</param>
        public void SetShown(bool shown, bool manual = true)
        {
            if (manual && m_Children != null && m_Children.Count > 0)
            {
                return;
            }

            Show = shown;
            SetReddotUIShow();

            parent?.RefreshShown();
        }
        /// <summary>
        /// 叶子节点设置Shown后,沿着key向上刷新
        /// </summary>
        public void RefreshShown()
        {
            if (m_Children == null)
            {
                SetShown(Show, false);
                return;
            }

            foreach (var child in m_Children)
            {
                if (child.Show)
                {
                    SetShown(true, false);
                    return;
                }
            }
            SetShown(false, false);
        }

        /// <summary>
        /// 根据数据设置红点的实际UI表现
        /// </summary>
        private void SetReddotUIShow()
        {
            if (s_ReddotDic.TryGetValue(key, out var list) && list != null && list.Count > 0)
            {
                foreach (var child in list)
                {
                    child.SetReddotShow(Show);
                }
            }
        }
    }

    #region 红点UI
    /// <summary>
    /// 缓存所有在生命周期中的红点组件
    /// 用来在红点树更新之后重新设置节点时, 查找对应Key的UI对象
    /// </summary>
    private static readonly Dictionary<string, List<Reddot>> s_ReddotDic = new Dictionary<string, List<Reddot>>();

    /// <summary>
    /// 注册红点元件。
    /// </summary>
    /// <param name="reddot">红点对象,包含key和flag</param>
    public static void RegisterRedDotUI(Reddot reddot)
    {
        string key = reddot.Path;
        if (!s_ReddotDic.TryGetValue(key, out var list))
        {
            list = new List<Reddot>();
            s_ReddotDic[key] = list;
        }
        if (!list.Contains(reddot))
        {
            list.Add(reddot);
        }
    }
    /// <summary>
    /// 注销红点UI对象
    /// </summary>
    /// <param name="reddot">红点对象</param>
    public static void UnRegisterRedDotUI(Reddot reddot)
    {
        string key = reddot.Path;
        if (!s_ReddotDic.TryGetValue(key, out var list)) return;
        if (list != null)
        {
            list.Remove(reddot);
            if (list.Count == 0)
            {
                s_ReddotDic.Remove(key);
            }
        }
    }

    #endregion

    #region 红点树
    /// <summary>
    /// 缓存所有红点树的根节点, 叶子节点通过根节点的Children查找
    /// </summary>
    private static readonly Dictionary<string, ReddotData> s_ReddotDataDic = new Dictionary<string, ReddotData>();

    private static readonly char PathSLASH = '/';

    public static void ClearReddotData()
    {
        s_ReddotDic.Clear();
        s_ReddotDataDic.Clear();
    }
    public static void SetRedDotData(bool isShown, string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        string[] paths = path.Split(PathSLASH);

        var root = paths[0];
        if (s_ReddotDataDic.TryGetValue(root, out var value))
        {
            if (BuildNode(isShown, value, paths, 1) == null)
            {
                value.SetShown(isShown);
            }
        }
        else
        {
            s_ReddotDataDic[root] = BuildNode(isShown, null, paths, 0);
        }
    }

    public static void RefreshShown(string path)
    {
        ReddotData data = FindReddotData(path);
        if (data != null)
        {
            data.RefreshShown();
        }
    }
    private static ReddotData FindReddotData(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        string[] paths = path.Split(PathSLASH);

        ReddotData parent;
        ReddotData child = null;
        if (s_ReddotDataDic.TryGetValue(paths[0], out parent))
        {
            for (int i = 1; i < paths.Length; i++)
            {
                var childKey = string.Join("/", paths.Take(i + 1).ToArray());
                if (parent.TryGetChild(childKey, out child))
                {
                    parent = child;
                }
            }
        }
        return parent;
    }

    /// <summary>
    /// 递归创建一条path上的全部节点 
    /// </summary>
    /// <param name="isShown"></param>
    /// <param name="parent"></param>
    /// <param name="path"></param>
    /// <param name="index">path的index,在递归中递增</param>
    /// <returns></returns>
    private static ReddotData BuildNode(bool isShown, ReddotData parent, string[] paths, int index)
    {
        if (index >= paths.Length) return null;
        var childKey = string.Join("/", paths.Take(index + 1).ToArray());
        ReddotData child = null;
        if (parent == null)
        {
            //根节点直接创建
            child = new ReddotData
            {
                key = childKey
            };
            if (BuildNode(isShown, child, paths, index + 1) == null)
            {
                //叶子结点直接设置shown
                child.SetShown(isShown);
            }
            return child;
        }
        else
        {
            if (parent.TryGetChild(childKey, out child))
            {
            }
            else
            {
                child = new ReddotData
                {
                    key = childKey,
                };
                parent.AddChild(child);
            }
            if (BuildNode(isShown, child, paths, index + 1) == null)
            {
                child.SetShown(isShown);
            }
            return child;
        }
    }

    #endregion
}
using System;
using System.Runtime.InteropServices;

namespace GameFramework
{
    /// <summary>
    /// 名称和类型的组合值。
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct NameTypePair : IEquatable<NameTypePair>
    {
        private readonly Type m_Type;
        private readonly string m_Name;

        /// <summary>
        /// 初始化名称和类型的组合值的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        public NameTypePair(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// 初始化名称和类型的组合值的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="type">类型。</param>
        public NameTypePair(string name, Type type)
        {
            if (name == null)
            {
                throw new GameFrameworkException("Name is invalid.");
            }

            m_Name = name;
            m_Type = type;
        }

        /// <summary>
        /// 获取名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取类型。
        /// </summary>
        public Type Type
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// 获取名称和类型的组合值字符串。
        /// </summary>
        /// <returns>名称和类型的组合值字符串。</returns>
        public override string ToString()
        {
            if (m_Name == null)
            {
                throw new GameFrameworkException("Name is invalid.");
            }

            return m_Type == null ? m_Name : Utility.Text.Format("{0}.{1}", m_Name, m_Type.FullName);
        }

        /// <summary>
        /// 获取对象的哈希值。
        /// </summary>
        /// <returns>对象的哈希值。</returns>
        public override int GetHashCode()
        {
            return m_Type == null ? m_Name.GetHashCode() : m_Name.GetHashCode() ^ m_Type.GetHashCode();
        }

        /// <summary>
        /// 比较对象是否与自身相等。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>被比较的对象是否与自身相等。</returns>
        public override bool Equals(object obj)
        {
            return obj is NameTypePair && Equals((NameTypePair)obj);
        }

        /// <summary>
        /// 比较对象是否与自身相等。
        /// </summary>
        /// <param name="value">要比较的对象。</param>
        /// <returns>被比较的对象是否与自身相等。</returns>
        public bool Equals(NameTypePair value)
        {
            return m_Type == value.m_Type && m_Name == value.m_Name;
        }

        /// <summary>
        /// 判断两个对象是否相等。
        /// </summary>
        /// <param name="a">值 a。</param>
        /// <param name="b">值 b。</param>
        /// <returns>两个对象是否相等。</returns>
        public static bool operator ==(NameTypePair a, NameTypePair b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// 判断两个对象是否不相等。
        /// </summary>
        /// <param name="a">值 a。</param>
        /// <param name="b">值 b。</param>
        /// <returns>两个对象是否不相等。</returns>
        public static bool operator !=(NameTypePair a, NameTypePair b)
        {
            return !(a == b);
        }
    }
}

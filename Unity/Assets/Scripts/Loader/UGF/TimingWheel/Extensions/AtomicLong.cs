using System;
using System.Threading;

namespace TimingWheel.Extensions
{
    /// <summary>
    /// 原子long
    /// </summary>
    public class AtomicLong : IComparable, IComparable<long>, IEquatable<long>
    {
        private long _value;

        public AtomicLong(long value = default)
        {
            _value = value;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public long Get()
        {
            // long是64位整型，在64位系统中读取是原子操作，但32位系统不是，所以这里统一用Read原子读
            return Interlocked.Read(ref _value);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        public void Set(long value)
        {
            Interlocked.Exchange(ref _value, value);
        }

        /// <summary>
        /// 设置并返回旧值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public long GetAndSet(long value)
        {
            return Interlocked.Exchange(ref _value, value);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is AtomicLong value)
            {
                return Get().CompareTo(value.Get());
            }

            throw new ArgumentException($"Object is not a {nameof(AtomicLong)}");
        }

        public int CompareTo(long other)
        {
            return Get().CompareTo(other);
        }

        public bool Equals(long other)
        {
            return Get().Equals(other);
        }
    }
}
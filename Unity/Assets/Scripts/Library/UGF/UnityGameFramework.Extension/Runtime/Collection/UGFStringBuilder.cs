using System;
using System.Collections.Generic;
using System.Text;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public sealed class UGFStringBuilder : IReference, IDisposable
    {
        private readonly StringBuilder m_StringBuilder = new StringBuilder();
        public void Clear()
        {
            m_StringBuilder.Clear();
        }

        public void Append(string value)
        {
            m_StringBuilder.Append(value);
            
        }

        public void AppendLine(string value)
        {
            m_StringBuilder.AppendLine(value);
        }

        public void AppendJoin(string separator, string[] values)
        {
            m_StringBuilder.AppendJoin(separator, values);
        }

        public void AppendJoin(string separator, object[] values)
        {
            m_StringBuilder.AppendJoin(separator, values);
        }

        public void AppendJoin<T>(char separator, IEnumerable<T> values)
        {
            m_StringBuilder.AppendJoin(separator, values);
        }

        public void AppendJoin<T>(string separator, IEnumerable<T> values)
        {
            m_StringBuilder.AppendJoin(separator, values);
        }

        public void AppendFormat(string format, object arg0)
        {
            m_StringBuilder.AppendFormat(format, arg0);
        }
        
        public void AppendFormat(string format, object arg0, object arg1)
        {
            m_StringBuilder.AppendFormat(format, arg0, arg1);
        }

        public void AppendFormat(string format, object arg0, object arg1, object arg2)
        {
            m_StringBuilder.AppendFormat(format, arg0, arg1, arg2);
        }

        public void AppendFormat(string format, params object[] args)
        {
            m_StringBuilder.AppendFormat(format, args);
        }
        
        public override string ToString()
        {
            return m_StringBuilder.ToString();
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static UGFStringBuilder Create()
        {
            UGFStringBuilder stringBuilder = ReferencePool.Acquire<UGFStringBuilder>();
            return stringBuilder;
        }
    }
}

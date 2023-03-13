using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace QFSW.QC
{
    public class LogStorage : ILogStorage
    {
        private readonly List<ILog> _consoleLogs = new List<ILog>(10);
        private readonly StringBuilder _logTraceBuilder = new StringBuilder(2048);

        public int MaxStoredLogs { get; set; }
        public IReadOnlyList<ILog> Logs => _consoleLogs;

        public LogStorage(int maxStoredLogs = -1)
        {
            MaxStoredLogs = maxStoredLogs;
        }

        public void AddLog(ILog log)
        {
            _consoleLogs.Add(log);
            
            int logLength = _logTraceBuilder.Length + log.Text.Length;
            if (log.NewLine && _logTraceBuilder.Length > 0)
            {
                logLength += Environment.NewLine.Length;
            }
            
            if (MaxStoredLogs > 0)
            {
                while (_consoleLogs.Count > MaxStoredLogs)
                {
                    int junkLength = _consoleLogs[0].Text.Length;
                    if (_consoleLogs.Count > 1 &&_consoleLogs[1].NewLine)
                    {
                        junkLength += Environment.NewLine.Length;
                    }
                    junkLength = Mathf.Min(junkLength, _logTraceBuilder.Length);
                    logLength -= junkLength;
                    
                    _logTraceBuilder.Remove(0, junkLength);
                    _consoleLogs.RemoveAt(0);
                }
            }

            int capacity = _logTraceBuilder.Capacity;
            while (capacity < logLength)
            {
                capacity *= 2;
            }

            _logTraceBuilder.EnsureCapacity(capacity);

            if (log.NewLine && _logTraceBuilder.Length > 0)
            {
                _logTraceBuilder.Append(Environment.NewLine);
            }
            _logTraceBuilder.Append(log.Text);
        }

        public void RemoveLog()
        {
            if (_consoleLogs.Count > 0)
            {
                ILog log = _consoleLogs[_consoleLogs.Count - 1];
                _consoleLogs.RemoveAt(_consoleLogs.Count - 1);

                int removeLength = log.Text.Length;
                if (log.NewLine && _consoleLogs.Count > 0)
                {
                    removeLength += Environment.NewLine.Length;
                }

                _logTraceBuilder.Remove(_logTraceBuilder.Length - removeLength, removeLength);
            }
        }

        public void Clear()
        {
            _consoleLogs.Clear();
            _logTraceBuilder.Length = 0;
        }

        public string GetLogString()
        {
            return _logTraceBuilder.ToString();
        }
    }
}
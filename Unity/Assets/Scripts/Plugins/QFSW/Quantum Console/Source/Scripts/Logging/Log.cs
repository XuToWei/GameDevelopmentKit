using UnityEngine;

namespace QFSW.QC
{
    public readonly struct Log : ILog
    {
        public string Text { get; }
        public LogType Type { get; }
        public bool NewLine { get; }

        public Log(string text, LogType type = LogType.Log, bool newLine = true)
        {
            Text = text;
            Type = type;
            NewLine = newLine;
        }
    }
}

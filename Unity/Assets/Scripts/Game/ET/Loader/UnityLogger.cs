using System;
using GFLog = UnityGameFramework.Runtime.Log;

namespace ET
{
    public class UnityLogger: ILog
    {
        public void Trace(string msg)
        {
            GFLog.Debug(msg);
        }

        public void Debug(string msg)
        {
            GFLog.Debug(msg);
        }

        public void Info(string msg)
        {
            GFLog.Info(msg);
        }

        public void Warning(string msg)
        {
            GFLog.Warning(msg);
        }

        public void Error(string msg)
        {
            GFLog.Error(msg);
        }

        public void Error(Exception e)
        {
            GFLog.Error(e);
        }

        public void Trace(string message, params object[] args)
        {
            GFLog.Debug(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            GFLog.Warning(message, args);
        }

        public void Info(string message, params object[] args)
        {
            GFLog.Info(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            GFLog.Debug(message, args);
        }

        public void Error(string message, params object[] args)
        {
            GFLog.Error(message, args);
        }
    }
}
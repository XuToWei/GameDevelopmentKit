namespace ET
{
    public static class OpcodeHelper
    {
        [StaticField]
        private static IOpcodeIgnoreDebugLog iOpcodeIgnoreDebugLog;

        public static IOpcodeIgnoreDebugLog IOpcodeIgnoreDebugLog
        {
            set
            {
                iOpcodeIgnoreDebugLog = value;
            }
        }

        private static bool IsNeedLogMessage(ushort opcode)
        {
            if (iOpcodeIgnoreDebugLog.IgnoreDebugLogMessageSet.Contains(opcode))
            {
                return false;
            }

            return true;
        }

        public static bool IsOuterMessage(ushort opcode)
        {
            return opcode < OpcodeRangeDefine.OuterMaxOpcode;
        }

        public static bool IsInnerMessage(ushort opcode)
        {
            return opcode >= OpcodeRangeDefine.InnerMinOpcode;
        }

        public static void LogMsg(int zone, object message)
        {
            ushort opcode = NetServices.Instance.GetOpcode(message.GetType());
            if (!IsNeedLogMessage(opcode))
            {
                return;
            }
            
            Logger.Instance.Debug("zone: {0} {1}", zone, message);
        }
        
        public static void LogMsg(long actorId, object message)
        {
            ushort opcode = NetServices.Instance.GetOpcode(message.GetType());
            if (!IsNeedLogMessage(opcode))
            {
                return;
            }
            
            Logger.Instance.Debug("actorId: {0} {1}", actorId, message);
        }
    }
}
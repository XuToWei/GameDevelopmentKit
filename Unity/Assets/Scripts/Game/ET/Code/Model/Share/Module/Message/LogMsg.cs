using System.Collections.Generic;

namespace ET
{
    public class LogMsg: Singleton<LogMsg>, ISingletonAwake
    {
        private readonly HashSet<ushort> ignore = new()
        {
            Message_ET_Client.C2G_Ping, 
            Message_ET_Client.G2C_Ping, 
            Message_ET_Client.C2G_Benchmark, 
            Message_ET_Client.G2C_Benchmark,
        };

        public void Awake()
        {
        }

        public void Debug(Fiber fiber, object msg)
        {
            ushort opcode = OpcodeType.Instance.GetOpcode(msg.GetType());
            if (this.ignore.Contains(opcode))
            {
                return;
            }
            fiber.Log.Debug(msg.ToString());
        }
    }
}
using System;

namespace ET
{
    public enum Proto2CSCodeType
    {
        ET,
        Game
    }
    
    
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;
    }

    public static partial class Proto2CS
    {
        public static void Export(Proto2CSCodeType codeType)
        {
            switch (codeType)
            {
                case Proto2CSCodeType.ET:
                    //Proto2CS_ET.Proto2CS();
                    break;
                case Proto2CSCodeType.Game:
                    break;
            }
            Console.WriteLine("proto2cs succeed!");
        }
    }
}
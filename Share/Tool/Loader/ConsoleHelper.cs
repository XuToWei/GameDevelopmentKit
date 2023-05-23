using System;

namespace ET
{
    public static class ConsoleHelper
    {
        public static void WriteErrorLine(string error)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine(error);
            Console.ForegroundColor = oldColor;
        }
    }
}
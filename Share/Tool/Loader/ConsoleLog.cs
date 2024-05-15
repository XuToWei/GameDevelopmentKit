using System;

namespace ET
{
    public class ConsoleLog : ILog
    {
        public void Trace(string message)
        {
            Console.WriteLine(message);
        }

        public void Warning(string message)
        {
            lock (Console.Error)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine(message);
                Console.ForegroundColor = oldColor;
            }
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            lock (Console.Error)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(message);
                Console.ForegroundColor = oldColor;
            }
        }

        public void Error(Exception e)
        {
            Error(e.ToString());
        }

        public void Trace(ref System.Runtime.CompilerServices.DefaultInterpolatedStringHandler message)
        {
            Trace(message.ToStringAndClear());
        }

        public void Warning(ref System.Runtime.CompilerServices.DefaultInterpolatedStringHandler message)
        {
            Warning(message.ToStringAndClear());
        }

        public void Info(ref System.Runtime.CompilerServices.DefaultInterpolatedStringHandler message)
        {
            Info(message.ToStringAndClear());
        }

        public void Debug(ref System.Runtime.CompilerServices.DefaultInterpolatedStringHandler message)
        {
            Debug(message.ToStringAndClear());
        }

        public void Error(ref System.Runtime.CompilerServices.DefaultInterpolatedStringHandler message)
        {
            Error(message.ToStringAndClear());
        }
    }
}

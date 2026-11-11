using System;

namespace Game.Editor
{
    /// <summary>Domain errors retain their code in the CLI's error message.</summary>
    public sealed class GameCommandException : Exception
    {
        public const string InvalidParams = "INVALID_PARAMS";

        public string Code { get; }

        public GameCommandException(string code, string message) : base($"{code}: {message}")
        {
            Code = code;
        }
    }
}

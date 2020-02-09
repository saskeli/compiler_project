using System;

namespace mpl.Exceptions
{
    public class UnexpectedCharacterException : Exception
    {
        public int Line { get; }
        public int Position { get; }
        public UnexpectedCharacterException(string message, int line, int pos)
            : base(message)
        {
            Line = line;
            Position = pos;
        }

    }
}

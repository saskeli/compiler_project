using System;

namespace mpl.Exceptions
{
    public class InvalidSyntaxException : Exception
    {
        public int Line { get; }
        public int Position { get; }
        public InvalidSyntaxException(string message, int line, int pos)
            : base(message)
        {
            Line = line;
            Position = pos;
        }

    }
}

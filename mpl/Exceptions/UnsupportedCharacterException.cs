using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.Exceptions
{
    public class UnsupportedCharacterException : Exception
    {
        public int Line { get; }
        public int Position { get; }
        public UnsupportedCharacterException(string message, int line, int pos)
            : base(message)
        {
            Line = line;
            Position = pos;
        }
    }
}

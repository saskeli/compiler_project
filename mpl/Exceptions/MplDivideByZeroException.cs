using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.Exceptions
{
    public class MplDivideByZeroException : Exception
    {
        public int Line { get; }
        public int Position { get; }

        public MplDivideByZeroException(string message, int line, int pos)
            : base(message)
        {
            Line = line;
            Position = pos;
        }
    }
}

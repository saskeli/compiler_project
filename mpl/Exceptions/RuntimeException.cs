using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.Exceptions
{
    class RuntimeException : Exception
    {

        public int Line { get; }
        public int Position { get; }

        public RuntimeException(string message, int line, int pos)
            : base(message)
        {
            Line = line;
            Position = pos;
        }
    }
}

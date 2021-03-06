﻿using System;

namespace mpl.Exceptions
{
    public class RuntimeException : Exception
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

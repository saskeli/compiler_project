using System;
using System.Collections.Generic;

namespace mpl.Exceptions
{
    public class MultiException : Exception
    {
        public readonly List<Exception> Exceptions;

        public MultiException(List<Exception> exceptions)
            : base("Multi exception")
        {
            Exceptions = exceptions;
        }
    }
}

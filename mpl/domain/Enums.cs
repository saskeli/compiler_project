using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.domain
{
    public enum TokenType
    {
        Name,
        String,
        Number,
        Control
    }

    public enum Type
    {
        Int,
        String,
        Bool
    }

    public enum Ctype
    {
        Num,
        Char,
        Control,
        Whitespace
    }
}

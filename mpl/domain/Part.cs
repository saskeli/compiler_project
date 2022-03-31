using System.Collections.Generic;

namespace mpl.domain
{
    public abstract class Part
    {
        public static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "integer",
            "real",
            "string",
            "boolean",
            "var",
            "for",
            "in",
            "do",
            "end",
            "read",
            "print",
            "writeln",
            "assert",
            "begin",
            "while",
            "do",
            "program",
            "function",
            "procedure",
            "array", 
            "of",
            "return",
            "if",
            "then",
            "else",
            "not",
            "or",
            "and",
            "false",
            "true",
        };
        public abstract void Run();
        public abstract Part GetParent();
        public abstract void Add(Token token);
        public abstract bool Exit();
        public abstract Definition GetDefinition(string name);
    }
}

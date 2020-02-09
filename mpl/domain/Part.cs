using System.Collections.Generic;

namespace mpl.domain
{
    public abstract class Part
    {
        public static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "int",
            "string",
            "bool",
            "var",
            "for",
            "in",
            "do",
            "end",
            "read",
            "print",
            "assert"
        };
        public abstract void Run();
        public abstract Part GetParent();
        public abstract void Add(Token token);
        public abstract bool Exit();
        public abstract Definition GetDefinition(string name);
    }
}

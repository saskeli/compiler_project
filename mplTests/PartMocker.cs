using System;
using System.Collections.Generic;
using System.Text;
using mpl.domain;

namespace mplTests.domain
{
    class PartMocker : Part
    {
        public int RunCalls;
        public int GetParentCalls;
        public int AddCalls;
        public int ExitCalls;
        public int GetDefinitionCalls;

        public List<Token> TokenList = new List<Token>();
        public List<string> DefinitionCalls = new List<string>();

        public HashSet<string> Scope = new HashSet<string>();
        public Dictionary<string, Definition> Definitions = new Dictionary<string, Definition>();

        public bool ExitValue;

        public override void Run() => RunCalls++;

        public override Part GetParent()
        {
            GetParentCalls++;
            return this;
        }

        public override void Add(Token token)
        {
            TokenList.Add(token);
            AddCalls++;
        }

        public override bool Exit()
        {
            ExitCalls++;
            return ExitValue;
        }

        public override Definition GetDefinition(string name)
        {
            DefinitionCalls.Add(name);
            GetDefinitionCalls++;
            return Definitions.GetValueOrDefault(name);
        }
    }
}

using System.Collections.Generic;
using mpl;
using mpl.domain;

namespace mplTests
{
    public class ParserMock : TokenParser
    {
        public List<Token> Tokens = new List<Token>();
        public ParserMock(int debug, bool verbose) : base(debug, verbose) { }

        public override void ParseToken(Token token)
        {
            Tokens.Add(token);
        }
    }
}

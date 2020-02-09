using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using mpl;
using mpl.domain;

namespace mplTests
{
    class ParserMock : TokenParser
    {
        public List<Token> Tokens = new List<Token>();
        public ParserMock(int debug, bool verbose) : base(debug, verbose) { }

        public override void ParseToken(Token token)
        {
            Tokens.Add(token);
        }
    }
}

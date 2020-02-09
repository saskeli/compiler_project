using System;
using System.Collections.Generic;
using System.Text;
using mpl.domain;

namespace mpl
{
    public class TokenParser
    {
        private readonly int _debug;
        private readonly bool _verbose;
        private bool _called;
        private Program Prog { get; }

        public TokenParser(int debug, bool verbose)
        {
            _debug = debug;
            _verbose = verbose;
            Prog = new Program();
        }

        public virtual void ParseToken(Token token)
        {
            if (!_called)
            {
                if (_verbose || _debug > 0) Console.WriteLine("Token parsing started");
                _called = true;
            }
            if (_debug > 0) Console.WriteLine($"Received token {token.token} - {token.tokenType}.");
            Prog.Add(token);
        }

        public Program GetProgram()
        {
            return Prog.GetProgram();
        }
    }
}

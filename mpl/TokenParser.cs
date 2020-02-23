using System;
using mpl.domain;
using mpl.Exceptions;

namespace mpl
{
    public class TokenParser
    {
        private readonly int _debug;
        private readonly bool _verbose;
        private bool _called;
        private bool _runnable = true;
        private readonly Program _prog;

        public TokenParser(int debug, bool verbose)
        {
            _debug = debug;
            _verbose = verbose;
            _prog = new Program();
        }

        public virtual void ParseToken(Token token)
        {
            if (!_called)
            {
                if (_verbose || _debug > 0) Console.WriteLine("Token parsing started");
                _called = true;
            }
            if (_debug > 0) Console.WriteLine($"Received TokenString {token.TokenString} - {token.TokenType}.");
            try
            {
                _prog.Add(token);
            }
            catch (Exception)
            {
                _runnable = false;
                _prog.NullOut();
                throw;
            }
        }

        public Program GetProgram()
        {
            if (!_runnable)
                throw new RuntimeException("Invalid program retrieved for execution", 0, 0);
            return _prog.GetProgram();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.VisualBasic;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Assertion : Part
    {
        private enum AssState
        {
            Empty,
            Building,
            Done
        }
        private readonly Part _parent;
        private Assignment _operation;
        private int _parens;
        private AssState _state = AssState.Empty;
        public Assertion(Part parent)
        {
            _parent = parent;
        }

        public override void Run()
        {
            _operation.Run();
            if (!((MplBoolean)_operation.Value).Val)
                throw new AssertionException("Assertion failed", 0, 0);
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            switch (_state)
            {
                case AssState.Empty:
                    if (token.tokenType != TokenType.Control || token.token != "(")
                        throw new InvalidSyntaxException($"Invalid token for start of assertion body: {token.token}", token.line, token.position);
                    _parens++;
                    _operation = new Assignment(this);
                    _state = AssState.Building;
                    return;
                case AssState.Building:
                    if (token.tokenType == TokenType.Control && token.token == "(")
                        _parens++;
                    else if (token.tokenType == TokenType.Control && token.token == ")")
                    {
                        _parens--;
                        if (_parens == 0) 
                        { 
                            if (_operation.Exit())
                            {
                                _state = AssState.Done;
                                return;
                            }
                            throw new InvalidSyntaxException("Unexpected closing paren detected.", token.line, token.position);
                        }
                    }
                    _operation.Add(token);
                    return;
                case AssState.Done:
                    throw new InvalidSyntaxException($"Line terminator expected. Got {token.token}.", token.line, token.position);

            }
        }

        public override bool Exit() => _state == AssState.Done;

        public override Definition GetDefinition(string name) => _parent.GetDefinition(name);
    }
}

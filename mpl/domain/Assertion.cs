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
        private readonly int _position;
        private readonly int _line;

        public Assertion(Part parent, int line, int position)
        {
            _parent = parent;
            _position = position;
            _line = line;
        }

        public override void Run()
        {
            _operation.Run();
            if (!((MplBoolean)_operation.Value).Val)
                throw new AssertionException("Assertion failed", _line, _position);
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            switch (_state)
            {
                case AssState.Empty:
                    if (token.TokenType != TokenType.Control || token.TokenString != "(")
                        throw new InvalidSyntaxException($"Invalid TokenString for start of assertion body: {token.TokenString}", token.Line, token.Position);
                    _parens++;
                    _operation = new Assignment(this, token.Line, token.Position);
                    _state = AssState.Building;
                    return;
                case AssState.Building:
                    if (token.TokenType == TokenType.Control && token.TokenString == "(")
                        _parens++;
                    else if (token.TokenType == TokenType.Control && token.TokenString == ")")
                    {
                        _parens--;
                        if (_parens == 0) 
                        { 
                            if (_operation.Exit())
                            {
                                _state = AssState.Done;
                                return;
                            }
                            throw new InvalidSyntaxException("Unexpected closing paren detected.", token.Line, token.Position);
                        }
                    }
                    _operation.Add(token);
                    return;
                case AssState.Done:
                    throw new InvalidSyntaxException($"Line terminator expected. Got {token.TokenString}.", token.Line, token.Position);

            }
        }

        public override bool Exit() => _state == AssState.Done;

        public override Definition GetDefinition(string name) => _parent.GetDefinition(name);
    }
}

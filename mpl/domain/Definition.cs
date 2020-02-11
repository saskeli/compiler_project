using mpl.Exceptions;

namespace mpl.domain
{
    public class Definition : Part
    {
        private enum DState
        {
            Empty,
            Named,
            Typing,
            Typed,
            Assigning
        }

        private DState _state = DState.Empty;
        private IValue _val;
        public string Name;
        private readonly Part _parent;
        private Assignment _assignment;
        public bool Locked = false;
        public int Line;
        public int Position;

        public Definition(Part parent, int line, int position)
        {
            _parent = parent;
            Line = line;
            Position = position;
        }

        public Definition(Part parent, int v, int line, int position)
        {
            _parent = parent;
            _val = new MplInteger(v, line, position);
            Line = line;
            Position = position;
        }

        public Definition(Part parent, string v, int line, int position)
        {
            _parent = parent;
            Line = line;
            Position = position;
            _val = new MplString(v, line, position);
        }

        public override void Run()
        {
            if (_assignment == null)
            {
                _val = _val switch
                {
                    MplBoolean _ => new MplBoolean(false, Line, Position),
                    MplInteger _ => new MplInteger(0, Line, Position),
                    _ => new MplString("", Line, Position),
                };
            } else _assignment.Run();
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            switch (_state)
            {
                case DState.Empty:
                    SetName(token);
                    return;
                case DState.Named:
                    if (token.TokenType != TokenType.Control
                        || !token.TokenString.Equals(":"))
                        throw new InvalidSyntaxException("':' required after defining a variable name", token.Line, token.Position);
                    _state = DState.Typing;
                    return;
                case DState.Typing:
                    SetType(token);
                    return;
                case DState.Typed:
                    BuildAssignment(token);
                    return;
                case DState.Assigning:
                    _assignment.Add(token);
                    return;
            }
        }

        private void BuildAssignment(Token token)
        {
            if (token.TokenType != TokenType.Control || token.TokenString != ":=")
                throw new InvalidSyntaxException($"Expected assignment TokenString or line terminator. Got {token.TokenString}", token.Line, token.Position);
            _assignment = new Assignment(this, this, token.Line, token.Position);
            _state = DState.Assigning;
        }

        private void SetType(Token token)
        {
            if (token.TokenType != TokenType.Name)
                throw new InvalidSyntaxException("Type declaration required", token.Line, token.Position);
            _val = token.TokenString switch
            {
                "int" => new MplInteger(0, token.Line, token.Position),
                "bool" => new MplBoolean(false, token.Line, token.Position),
                "string" => new MplString("", token.Line, token.Position),
                _ => throw new InvalidSyntaxException($"Invalid type declaration: {token.TokenString}", token.Line, token.Position)
            };
            _state = DState.Typed;
        }

        private void SetName(Token token)
        {
            if (token.TokenType != TokenType.Name)
                throw new InvalidSyntaxException("Expected a name after variable declaration", token.Line, token.Position);
            if (Keywords.Contains(token.TokenString))
                throw new InvalidSyntaxException($"Unable to re-define reserved keyword {token.TokenString}", token.Line, token.Position);
            Definition d = GetDefinition(token.TokenString);
            if (d != null)
                throw new InvalidSyntaxException($"Unable to re-define variable {token.TokenString}", token.Line, token.Position);
            Name = token.TokenString;
            _state = DState.Named;
        }

        public override bool Exit()
        {
            if (_state == DState.Typed) return true;
            return _assignment != null && _assignment.Exit();
        }

        public override Definition GetDefinition(string name)
        {
            return _parent.GetDefinition(name);
        }

        public IValue GetValue() => _val;

        public void SetValue(IValue v)
        {
            if (Locked) throw new RuntimeException($"Assignment to locked variable {Name}", v.GetLine(), v.GetPosition());
            _val = v;
        }

        public new Type GetType()
        {
            return _val switch
            {
                MplBoolean _ => Type.Bool,
                MplInteger _ => Type.Int,
                _ => Type.String
            };
        }
    }
}

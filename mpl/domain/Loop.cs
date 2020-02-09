using System.Collections.Generic;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Loop : Part
    {
        private enum LState
        {
            Empty,
            Variabled,
            InGot,
            RangeGot,
            Body,
            End,
            For
        }
        private readonly List<Part> _subParts = new List<Part>();
        private readonly Dictionary<string, int> _definitions = new Dictionary<string, int>();
        private readonly Part _parent;
        private Definition _loopvar;
        private readonly Assignment _sass;
        private readonly Assignment _eass;
        private Part _current;
        private LState _state = LState.Empty;
        private bool _newAssign;
        private readonly int _line;
        private readonly int _position;

        public Loop(Part parent, int line, int position)
        {
            _parent = parent;
            _line = line;
            _position = position;
            _sass = new Assignment(this, line, position);
            _eass = new Assignment(this, line, position);
        }

        public override void Run()
        {
            int[] range = GetRange();
            foreach (int i in range)
            {
                _loopvar.SetValue(new MplInteger(i, _line, _position));
                _loopvar.Locked = true;
                foreach (Part subPart in _subParts)
                {
                    subPart.Run();
                }

                _loopvar.Locked = false;
            }
        }

        private int[] GetRange()
        {
            _sass.Run();
            int start = ((MplInteger)_sass.Value).Val;
            _eass.Run();
            int end = ((MplInteger)_eass.Value).Val;

            int len = (start < end ? end - start : start - end) + 1;
            int[] a = new int[len];
            int v = start;
            int i = 0;
            while (i < len)
            {
                a[i] = v;
                i++;
                v += start < end ? 1 : -1;
            }

            return a;
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            //TODO: Ensure definitions are possible in loops
            switch (_state)
            {
                case LState.Empty:
                    EmptyState(token);
                    break;
                case LState.Variabled:
                    VariabledState(token);
                    break;
                case LState.InGot:
                    InGotState(token);
                    break;
                case LState.RangeGot:
                    RangeGotState(token);
                    break;
                case LState.Body:
                    BodyState(token);
                    break;
                case LState.End:
                    if (token.TokenType != TokenType.Name || token.TokenString != "for")
                        throw new InvalidSyntaxException($"No {token.TokenString} to end", token.Line, token.Position);
                    _state = LState.For;
                    break;
                case LState.For:
                    throw new InvalidSyntaxException($"Expected line termination. Got {token.TokenString}", token.Line, token.Position);
            }
        }

        private void EmptyState(Token token)
        {
            if (token.TokenType != TokenType.Name)
                throw new InvalidSyntaxException($"Expected loop variable id. Got {token.TokenString}", token.Line, token.Position);
            if (Keywords.Contains(token.TokenString))
                throw new InvalidSyntaxException($"Loop variable can not be a reserved keyword. Got {token.TokenString}", token.Line, token.Position);
            _loopvar = GetDefinition(token.TokenString);
            if (_loopvar == null)
                throw new InvalidSyntaxException($"Use of undefined variable as loop variable. {token.TokenString}", token.Line, token.Position);
            if (!(_loopvar.GetValue() is MplInteger))
                throw new InvalidSyntaxException($"Loop variable {token.TokenString} is not an integer", token.Line, token.Position);
            _state = LState.Variabled;
        }

        private void VariabledState(Token token)
        {
            if (token.TokenType != TokenType.Name || token.TokenString != "in")
                throw new InvalidSyntaxException($"Expected \"in\" keyword. Got {token.TokenString}", token.Line, token.Position);
            _state = LState.InGot;
        }

        private void InGotState(Token token)
        {
            if (token.TokenType == TokenType.Control && token.TokenString == "..")
            {
                if (!_sass.Exit())
                    throw new InvalidSyntaxException("Invalid start of range definition", token.Line, token.Position);
                _state = LState.RangeGot;
                return;
            }
            _sass.Add(token);
        }


        private void RangeGotState(Token token)
        {
            if (token.TokenType == TokenType.Name && token.TokenString == "do")
            {
                if (!_eass.Exit())
                    throw new InvalidSyntaxException("Invalid end of range definition", token.Line, token.Position);
                _state = LState.Body;
                return;
            }
            _eass.Add(token);
        }


        private void BodyState(Token token)
        {
            if (_newAssign)
            {
                if (token.TokenType != TokenType.Control || token.TokenString != ":=")
                    throw new InvalidSyntaxException($"Expected assignment. Got {token.TokenString}", token.Line, token.Position);
                _newAssign = false;
                return;
            }
            if (_current != null)
            {
                _current.Add(token);
                return;
            }

            if (token.TokenType != TokenType.Name)
            {
                throw new InvalidSyntaxException("Expected keyword or variable identifier", token.Line, token.Position);
            }

            if (Keywords.Contains(token.TokenString))
            {
                AddKey(token);
                return;
            }

            Definition def = GetDefinition(token.TokenString);
            if (def ==null)
                throw new InvalidSyntaxException($"Use of undeclared variable {token.TokenString}", token.Line, token.Position);

            _current = new Assignment(def, this, token.Line, token.Position);
            _newAssign = true;
        }

        private void AddKey(Token token)
        {
            _current = token.TokenString switch
            {
                "end" => null,
                "var" => new Definition(this, token.Line, token.Position),
                "for" => new Loop(this, token.Line, token.Position),
                "read" => new Reader(this, token.Line, token.Position),
                "print" => new Printer(this, token.Line, token.Position),
                "assert" => new Assertion(this, token.Line, token.Position),
                _ => throw new InvalidSyntaxException($"Invalid keyword for start of expression {token.TokenString}",
                    token.Line, token.Position)
            };
            if (token.TokenString == "end")
            {
                _state = LState.End;
            }
        }

        public override bool Exit()
        {
            if (_current != null && _current.Exit())
            {
                _subParts.Add(_current);
                _current = null;
            }

            return _state == LState.For;
        }

        public override Definition GetDefinition(string name) =>
            _definitions.ContainsKey(name) ? (Definition)_subParts[_definitions[name]] : _parent.GetDefinition(name);
    }
}

using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
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

        public Loop(Part parent)
        {
            _parent = parent;
            _sass = new Assignment(this);
            _eass = new Assignment(this);
        }

        public override void Run()
        {
            int[] range = GetRange();
            foreach (int i in range)
            {
                _loopvar.SetValue(new MplInteger(i));
                _loopvar.locked = true;
                foreach (Part subPart in _subParts)
                {
                    subPart.Run();
                }

                _loopvar.locked = false;
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
                    if (token.tokenType != TokenType.Name || token.token != "for")
                        throw new InvalidSyntaxException($"No {token.token} to end", token.line, token.position);
                    _state = LState.For;
                    break;
                case LState.For:
                    throw new InvalidSyntaxException($"Expected line termination. Got {token.token}", token.line, token.position);
            }
        }

        private void EmptyState(Token token)
        {
            if (token.tokenType != TokenType.Name)
                throw new InvalidSyntaxException($"Expected loop variable id. Got {token.token}", token.line, token.position);
            if (Keywords.Contains(token.token))
                throw new InvalidSyntaxException($"Loop variable can not be a reserved keyword. Got {token.token}", token.line, token.position);
            _loopvar = GetDefinition(token.token);
            if (_loopvar == null)
                throw new InvalidSyntaxException($"Use of undefined variable as loop variable. {token.token}", token.line, token.position);
            if (!(_loopvar.GetValue() is MplInteger))
                throw new InvalidSyntaxException($"Loop variable {token.token} is not an integer", token.line, token.position);
            _state = LState.Variabled;
        }

        private void VariabledState(Token token)
        {
            if (token.tokenType != TokenType.Name || token.token != "in")
                throw new InvalidSyntaxException($"Expected \"in\" keyword. Got {token.token}", token.line, token.position);
            _state = LState.InGot;
        }

        private void InGotState(Token token)
        {
            if (token.tokenType == TokenType.Control && token.token == "..")
            {
                if (!_sass.Exit())
                    throw new InvalidSyntaxException($"Invalid start of range definition", token.line, token.position);
                _state = LState.RangeGot;
                return;
            }
            _sass.Add(token);
        }


        private void RangeGotState(Token token)
        {
            if (token.tokenType == TokenType.Name && token.token == "do")
            {
                if (!_eass.Exit())
                    throw new InvalidSyntaxException($"Invalid end of range definition", token.line, token.position);
                _state = LState.Body;
                return;
            }
            _eass.Add(token);
        }


        private void BodyState(Token token)
        {
            if (_newAssign)
            {
                if (token.tokenType != TokenType.Control || token.token != ":=")
                    throw new InvalidSyntaxException($"Expected assignment. Got {token.token}", token.line, token.position);
                _newAssign = false;
                return;
            }
            if (_current != null)
            {
                _current.Add(token);
                return;
            }

            if (token.tokenType != TokenType.Name)
            {
                throw new InvalidSyntaxException("Expected keyword or variable identifier", token.line, token.position);
            }

            if (Keywords.Contains(token.token))
            {
                AddKey(token);
                return;
            }

            Definition def = GetDefinition(token.token);
            if (def ==null)
                throw new InvalidSyntaxException($"Use of undeclared variable {token.token}", token.line, token.position);

            _current = new Assignment(def, this);
            _newAssign = true;
        }

        private void AddKey(Token token)
        {
            _current = token.token switch
            {
                "end" => null,
                "var" => new Definition(this),
                "for" => new Loop(this),
                "read" => new Reader(this),
                "print" => new Printer(this),
                "assert" => new Assertion(this),
                _ => throw new InvalidSyntaxException($"Invalid keyword for start of expression {token.token}",
                    token.line, token.position)
            };
            if (token.token == "end")
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

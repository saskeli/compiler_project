using System;
using System.Collections.Generic;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Function : Part
    {
        private Part _current;
        private readonly List<Part> _subparts = new List<Part>();
        public Dictionary<string, int> Scope = new Dictionary<string, int>();
        private int _line;
        private int _pos;
        private bool _newAssign;
        private string _name = null;

        public override void Run()
        {
            foreach (Part part in _subparts)
            {
                part.Run();
            }
        }

        internal void Output(string output)
        {
            throw new NotImplementedException();
        }

        public override Part GetParent() => this;
        public override void Add(Token token)
        {
            if (_newAssign)
            {
                if (token.TokenType != TokenType.Control || token.TokenString != ":=")
                    throw new InvalidSyntaxException($"Expected assignment. Got {token.TokenString}", token.Line, token.Position);
                _newAssign = false;
                return;
            }
            _line = token.Line;
            _pos = token.Position;
            if (token.TokenType == TokenType.Control && token.TokenString.Equals(";"))
            {
                if (_current == null)
                {
                    throw new InvalidSyntaxException("Empty statement. Nothing to terminate", token.Line, token.Position);
                }

                if (!_current.Exit()) return;
                if (_current is Definition definition)
                {
                    Scope.Add(definition.Name, _subparts.Count);
                }
                _subparts.Add(_current);
                _current = null;
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

            if (!Scope.ContainsKey(token.TokenString))
                throw new InvalidSyntaxException($"Use of undeclared variable {token.TokenString}", token.Line, token.Position);

            _current = new Assignment((Definition)_subparts[Scope[token.TokenString]], this, token.Line, token.Position);
            _newAssign = true;
            _subparts.Add(_current);
        }

        public override bool Exit() => false;
        public override Definition GetDefinition(string name)
        {
            if (Scope.ContainsKey(name))
            {
                return (Definition)_subparts[Scope[name]];
            }

            return null;
        }


        private void AddKey(Token token)
        {
            _current = token.TokenString switch
            {
                "var" => new Definition(this, token.Line, token.Position),
                "for" => new Loop(this, token.Line, token.Position),
                "read" => new Reader(this, token.Line, token.Position),
                "print" => new Printer(this, token.Line, token.Position),
                "assert" => new Assertion(this, token.Line, token.Position),
                _ => throw new InvalidSyntaxException($"Invalid keyword for start of expression {token.TokenString}",
                    token.Line, token.Position)
            };
        }

        public void NullOut()
        {
            _current = null;
            _newAssign = false;
        }

    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Program : Part
    {
        private Part _current = null;
        private readonly List<Part> _subparts = new List<Part>();
        public Dictionary<string, int> Scope = new Dictionary<string, int>();
        private int _line;
        private int _pos;
        private bool _newAssign;

        public override void Run()
        {
            foreach (Part part in _subparts)
            {
                part.Run();
            }
        }

        public override Part GetParent() => this;
        public override void Add(Token token)
        {
            if (_newAssign)
            {
                if (token.tokenType != TokenType.Control || token.token != ":=")
                    throw new InvalidSyntaxException($"Expected assignment. Got {token.token}", token.line, token.position);
                _newAssign = false;
                return;
            }
            _line = token.line;
            _pos = token.position;
            if (token.tokenType == TokenType.Control && token.token.Equals(";"))
            {
                if (_current == null)
                {
                    throw new InvalidSyntaxException("Empty statement. Nothing to terminate", token.line, token.position);
                }

                if (_current.Exit())
                {
                    if (_current is Definition)
                    {
                        Scope.Add(((Definition)_current).Name, _subparts.Count);
                    }
                    _subparts.Add(_current);
                    _current = null;
                }
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

            if (!Scope.ContainsKey(token.token))
                throw new InvalidSyntaxException($"Use of undeclared variable {token.token}", token.line, token.position);

            _current = new Assignment((Definition)_subparts[Scope[token.token]], this);
            _newAssign = true;
            _subparts.Add(_current);
        }

        public override bool Exit() => false;
        public override Definition GetDefinition(string name)
        {
            if (Scope.ContainsKey(name))
            {
                return (Definition) _subparts[Scope[name]];
            }

            return null;
        }


        private void AddKey(Token token)
        {
            _current = token.token switch
            {
                "var" => new Definition(this),
                "for" => new Loop(this),
                "read" => new Reader(this),
                "print" => new Printer(this),
                "assert" => new Assertion(this),
                _ => throw new InvalidSyntaxException($"Invalid keyword for start of expression {token.token}",
                    token.line, token.position)
            };
        }

        public Program GetProgram()
        {
            if (_current != null)
                throw new InvalidSyntaxException("End of file reached while parsing. Missing ;?", _line, _pos);
            return this;
        }
    }

    
}

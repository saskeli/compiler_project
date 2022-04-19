using System;
using System.Collections.Generic;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Program : Part
    {
        private Part _current = null;
        private readonly List<Part> _main = new List<Part>();
        private readonly List<Part> _functions = new List<Part>();
        public Dictionary<string, int> Scope = new Dictionary<string, int>();
        private int _line;
        private int _pos;
        private bool _got_prog = false;
        private string _name = null;
        private bool _got_name = false;
        private bool _terminated = false;
        private bool _in_main = false;

        public override void Run()
        {
            if (_terminated)
            {
                foreach (Part p in _main)
                {
                    p.Run();
                }
            }
            else
            {
                throw new InvalidSyntaxException("Unexpected end of file, program was not terminated", 0, 0);
            }
        }

        internal void Output(string file, string output)
        {
            if (_terminated)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new InvalidSyntaxException("Unexpected end of file, program was not terminated", 0, 0);
            }
        }

        public override Part GetParent() => this;
        public override void Add(Token token)
        {
            if (!_got_prog)
            {
                if (token.TokenString != "program")
                {
                    throw new InvalidSyntaxException($"For whatever reason, file must start with \"program\".", token.Line, token.Position);
                }
                _got_prog = true;
                return;
            }
            if (_name == null)
            {
                if (token.TokenType != TokenType.Name || Keywords.Contains(token.TokenString))
                {
                    throw new InvalidSyntaxException($"{token.TokenString} is an invalid program identifier.", token.Line, token.Position);
                }
                _name = token.TokenString;
                return;
            }
            if (!_got_name)
            {
                if (token.TokenType != TokenType.Control || token.TokenString != ";")
                {
                    throw new InvalidSyntaxException($"Program name must be terminated by \";\". Got {token.TokenString}.", token.Line, token.Position);
                }
                _got_name = true;
                return;
            }
            if (_in_main)
            {

            }
            if (_current == null)
            {
                if (token.TokenType == TokenType.Name)
                {
                    if (token.TokenString.Equals("function"))
                    {
                        _current = new Function(this, token.Line, token.Position, true);
                    }
                    else if (token.TokenString.Equals("Procedure"))
                    {
                        _current = new Function(this, token.Line, token.Position, false);
                    }
                    else if (token.TokenString.Equals("begin"))
                    {
                        _in_main = true;
                    }
                    else
                    {
                        throw new InvalidSyntaxException(
                            $"program blocks must start with \"function\", \"procedure\" or \"begin\". Got {token.TokenString}",
                            token.Line, token.Position);
                    }
                }
            }
        }

        public override bool Exit() => false;
        public override Definition GetDefinition(string name)
        {
            if (Scope.ContainsKey(name))
            {
                return (Definition)_main[Scope[name]];
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
        }

        public Program GetProgram()
        {
            if (_current != null)
                throw new InvalidSyntaxException("End of file reached while parsing. Missing ;?", _line, _pos);
            return this;
        }
    }


}

using System;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Reader : Part
    {
        private readonly Part _parent;
        private Definition _def;
        private readonly int _line;
        private readonly int _position;

        public Reader(Part parent, int line, int position)
        {
            _parent = parent;
            _line = line;
            _position = position;
        }

        public override void Run()
        {
            string inp = Console.ReadLine();
            switch (_def.GetValue())
            {
                case MplInteger _:
                    _def.SetValue(new MplInteger(int.Parse(inp), _line, _position));
                    break;
                case MplBoolean _:
                    _def.SetValue(new MplBoolean(bool.Parse(inp), _line, _position));
                    break;
                default:
                    _def.SetValue(new MplString(inp, _line, _position));
                    break;
            }
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            if (_def != null)
                throw new InvalidSyntaxException($"Expected line terminator. Got {token.TokenString}", token.Line, token.Position);
            if (token.TokenType != TokenType.Name)
                throw new InvalidSyntaxException($"Expected variable identifier. Got {token.TokenString}", token.Line, token.Position);
            if (Keywords.Contains(token.TokenString))
                throw new InvalidSyntaxException($"{token.TokenString} is not a valid variable identifier", token.Line, token.Position);
            _def = GetDefinition(token.TokenString);
            if (_def == null)
                throw new InvalidSyntaxException($"Use of uninitialized variable {token.TokenString}", token.Line, token.Position);
        }

        public override bool Exit() => _def != null;

        public override Definition GetDefinition(string name) => _parent.GetDefinition(name);
    }
}

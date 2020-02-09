using System;
using System.Collections.Generic;
using System.Text;
using mpl.Exceptions;

namespace mpl.domain
{
    public class Reader : Part
    {
        private readonly Part _parent;
        private Definition _def;
        public Reader(Part parent)
        {
            _parent = parent;
        }

        public override void Run()
        {
            string inp = Console.ReadLine();
            switch (_def.GetValue())
            {
                case MplInteger _:
                    _def.SetValue(new MplInteger(int.Parse(inp)));
                    break;
                case MplBoolean _:
                    _def.SetValue(new MplBoolean(bool.Parse(inp)));
                    break;
                default:
                    _def.SetValue(new MplString(inp));
                    break;
            }
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            if (_def != null)
                throw new InvalidSyntaxException($"Expected line terminator. Got {token.token}", token.line, token.position);
            if (token.tokenType != TokenType.Name)
                throw new InvalidSyntaxException($"Expected variable identifier. Got {token.token}", token.line, token.position);
            if (Keywords.Contains(token.token))
                throw new InvalidSyntaxException($"{token.token} is not a valid variable identifier", token.line, token.position);
            _def = GetDefinition(token.token);
            if (_def == null)
                throw new InvalidSyntaxException($"Use of uninitialized variable {token.token}", token.line, token.position);
        }

        public override bool Exit() => _def != null;

        public override Definition GetDefinition(string name) => _parent.GetDefinition(name);
    }
}

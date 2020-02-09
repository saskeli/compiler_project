using System;
using System.Collections.Generic;
using System.Text;
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
        private IValue val = null;
        public string Name;
        private readonly Part _parent;
        private Assignment assignment;
        public bool locked = false;

        public Definition(Part parent)
        {
            _parent = parent;
        }

        public Definition(Part parent, int v)
        {
            _parent = parent;
            val = new MplInteger(v);
        }

        public Definition(Part parent, string v)
        {
            _parent = parent;
            val = new MplString(v);
        }

        public override void Run()
        {
            assignment?.Run();
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
                    if (token.tokenType != TokenType.Control
                        || !token.token.Equals(":"))
                        throw new InvalidSyntaxException("':' required after defining a variable name", token.line, token.position);
                    _state = DState.Typing;
                    return;
                case DState.Typing:
                    SetType(token);
                    return;
                case DState.Typed:
                    BuildAssignment(token);
                    return;
                case DState.Assigning:
                    assignment.Add(token);
                    return;
            }
        }

        private void BuildAssignment(Token token)
        {
            if (token.tokenType != TokenType.Control || token.token != ":=")
                throw new InvalidSyntaxException($"Expected assignment token or line terminator. Got {token.token}", token.line, token.position);
            assignment = new Assignment(this, this);
            _state = DState.Assigning;
        }

        private void SetType(Token token)
        {
            if (token.tokenType != TokenType.Name)
                throw new InvalidSyntaxException("Type declaration required", token.line, token.position);
            val = token.token switch
            {
                "int" => new MplInteger(0),
                "bool" => new MplBoolean(false),
                "string" => new MplString(""),
                _ => throw new InvalidSyntaxException($"Invalid type declaration: {token.token}", token.line, token.position)
            };
            _state = DState.Typed;
        }

        private void SetName(Token token)
        {
            if (token.tokenType != TokenType.Name)
                throw new InvalidSyntaxException("Expected a name after variable declaration", token.line, token.position);
            if (Keywords.Contains(token.token))
                throw new InvalidSyntaxException($"Unable to re-define reserved keyword {token.token}", token.line, token.position);
            Definition d = GetDefinition(token.token);
            if (d != null)
                throw new InvalidSyntaxException($"Unable to re-define variable {token.token}", token.line, token.position);
            Name = token.token;
            _state = DState.Named;
        }

        public override bool Exit()
        {
            if (_state == DState.Typed) return true;
            return assignment != null && assignment.Exit();
        }

        public override Definition GetDefinition(string name)
        {
            return _parent.GetDefinition(name);
        }

        public IValue GetValue() => val;

        public void SetValue(IValue v)
        {
            if (locked) throw new RuntimeException($"Assignment to locked variable {Name}", 0, 0);
            val = v;
        }

        public new Type GetType()
        {
            return val switch
            {
                MplBoolean _ => Type.Bool,
                MplInteger _ => Type.Int,
                _ => Type.String
            };
        }
    }
}

using mpl.Exceptions;

namespace mpl.domain
{
    public class Assignment : Part
    {
        private enum AState
        {
            Empty,
            Unary,
            Operanded,
            Operatored,
            Done,
            UnarySubPart,
            Op1SubPart,
            Op2SubPart,
        }

        private enum Operator
        {
            Addition,
            Subtraction,
            Multiplication,
            Division,
            Less,
            Equality,
            And,
            Negation
        }

        private readonly Definition _definition;
        private readonly Part _parent;
        public IValue Value;
        private Assignment _sub1;
        private Assignment _sub2;
        private Definition _def1;
        private Definition _def2;
        private AState _state = AState.Empty;
        private Operator _operator;
        private int _parens;
        private readonly int _position;
        private readonly int _line;

        public Assignment(Definition definition, Part parent, int line, int position)
        {
            _definition = definition;
            _parent = parent;
            _line = line;
            _position = position;
        }

        public Assignment(Part parent, int line, int position)
        {
            _parent = parent;
            _line = line;
            _position = position;
        }

        public override void Run()
        {
            IValue val1;
            if (_sub1 != null)
            {
                _sub1.Run();
                val1 = _sub1.Value;
            }
            else
            {
                val1 = _def1.GetValue();
            }

            if (_operator == Operator.Negation)
            {
                Value = !(MplBoolean)val1;
                _definition?.SetValue(Value);
                return;
            }

            if (_state == AState.Operanded)
            {
                Value = val1;
                _definition?.SetValue(Value);
                return;
            }

            IValue val2;
            if (_sub2 != null)
            {
                _sub2.Run();
                val2 = _sub2.Value;
            }
            else
            {
                val2 = _def2.GetValue();
            }

            // Negation was handled before.
#pragma warning disable 8509
            Value = _operator switch
#pragma warning restore 8509
            {
                Operator.Less => CalcLess(val1, val2),
                Operator.Addition => CalcAdd(val1, val2),
                Operator.And => (MplBoolean)val1 & (MplBoolean)val2,
                Operator.Division => (MplInteger)val1 / (MplInteger)val2,
                Operator.Equality => CalcEquality(val1, val2),
                Operator.Subtraction => (MplInteger)val1 - (MplInteger)val2,
                Operator.Multiplication => (MplInteger)val1 * (MplInteger)val2,
            };
            _definition?.SetValue(Value);
        }

        private IValue CalcEquality(IValue val1, IValue val2)
        {
            return val1 switch
            {
                MplInteger integer => (integer == (MplInteger) val2),
                MplBoolean boolean => (boolean == (MplBoolean) val2),
                _ => ((MplString) val1 == (MplString) val2)
            };
        }

        private IValue CalcAdd(IValue val1, IValue val2)
        {
            return val1 switch
            {
                MplInteger integer => (integer + (MplInteger) val2),
                _ => ((MplString) val1 + (MplString) val2)
            };
        }

        private IValue CalcLess(IValue val1, IValue val2)
        {
            return val1 switch
            {
                MplBoolean boolean => (boolean < (MplBoolean) val2),
                MplInteger integer => (integer < (MplInteger) val2),
                _ => ((MplString) val1 < (MplString) val2)
            };
        }

        public override Part GetParent() => _parent;

        public override void Add(Token token)
        {
            switch (_state)
            {
                case AState.Empty:
                    EmptyState(token);
                    return;
                case AState.Unary:
                    UnaryState(token);
                    return;
                case AState.UnarySubPart:
                case AState.Op1SubPart:
                    Op1SubPartState(token);
                    return;
                case AState.Operanded:
                    OperandedState(token);
                    return;
                case AState.Operatored:
                    OperatoredState(token);
                    return;
                case AState.Op2SubPart:
                    Op2SubPartState(token);
                    return;
                case AState.Done:
                    throw new InvalidSyntaxException($"Expected line terminator. Not {token.TokenString}", token.Line, token.Position);
            }

        }

        private void Op2SubPartState(Token token)
        {
            if (token.TokenString == ")")
            {
                _parens--;
                if (_parens > 0)
                {
                    _sub2.Add(token);
                    return;
                }
                if (!_sub2.Exit())
                    throw new InvalidSyntaxException("Invalid closing paren", token.Line, token.Position);
                _state = AState.Done;
                return;
            }
            if (token.TokenString == "(")
                _parens++;
            _sub2.Add(token);
        }

        private void Op1SubPartState(Token token)
        {
            if (token.TokenString == ")")
            {
                _parens--;
                if (_parens > 0)
                {
                    _sub1.Add(token);
                    return;
                }

                if (!_sub1.Exit())
                    throw new InvalidSyntaxException("Invalid closing paren", token.Line, token.Position);
                _state = AState.Operanded;
                return;
            }
            if (token.TokenString == "(")
                _parens++;
            _sub1.Add(token);
        }

        private void OperatoredState(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.Control:
                    if (token.TokenString != "(")
                        throw new InvalidSyntaxException($"Invalid TokenString for binary operator {token.TokenString}", token.Line, token.Position);
                    _parens++;
                    _state = AState.Op2SubPart;
                    _sub2 = new Assignment(this, token.Line, token.Position);
                    return;
                case TokenType.Name:
                    _def2 = GetDefinition(token.TokenString);
                    if (_def2 == null) throw new InvalidSyntaxException($"Use of undefined variable {token.TokenString}", token.Line, token.Position);
                    if (CheckType()) _state = AState.Done;
                    return;
                case TokenType.Number:
                    _def2 = new Definition(this, int.Parse(token.TokenString), token.Line, token.Position);
                    if (CheckType()) _state = AState.Done;
                    return;
                case TokenType.String:
                    _def2 = new Definition(this, token.TokenString);
                    if (CheckType()) _state = AState.Done;
                    return;
            }
        }

        private bool CheckType()
        {
            bool res = _operator switch
            {
                Operator.Addition => (
                    ((_def1?.GetType() ?? _sub1.GetType())
                     == (_def2?.GetType() ?? _sub2.GetType()))
                    && ((_def1?.GetType() ?? _sub1.GetType()) != Type.Bool)
                ),
                Operator.Negation => (_def1?.GetType() ?? _sub1.GetType()) == Type.Bool,
                Operator.And => (
                    (_def1?.GetType() ?? _sub2.GetType()) == Type.Bool
                    && (_def2?.GetType() ?? _sub2.GetType()) == Type.Bool
                ),
                Operator.Less => (
                    (_def1?.GetType() ?? _sub1.GetType())
                    == (_def2?.GetType() ?? _sub2.GetType())
                ),
                Operator.Equality => (
                    (_def1?.GetType() ?? _sub1.GetType())
                    == (_def2?.GetType() ?? _sub2.GetType())
                ),
                _ => (
                    (_def1?.GetType() ?? _sub1.GetType()) == Type.Int
                    && (_def2?.GetType() ?? _sub2.GetType()) == Type.Int)
            };
            if (!res) throw new InvalidSyntaxException("Invalid operand types", _line, _position);
            return true;
        }

        private void OperandedState(Token token)
        {
            if (token.TokenType != TokenType.Control)
                throw new InvalidSyntaxException($"Expected binary operator. Got {token.TokenString}", token.Line, token.Position);
            _operator = token.TokenString switch
            {
                "+" => Operator.Addition,
                "-" => Operator.Subtraction,
                "*" => Operator.Multiplication,
                "/" => Operator.Division,
                "<" => Operator.Less,
                "=" => Operator.Equality,
                "&" => Operator.And,
                _ => throw new InvalidSyntaxException($"Expected binary operator. Got {token.TokenString}", token.Line,
                    token.Position)
            };
            _state = AState.Operatored;
        }

        private void UnaryState(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.Control:
                    if (token.TokenString != "(")
                        throw new InvalidSyntaxException($"Invalid TokenString for body of negation {token.TokenString}", token.Line, token.Position);
                    _parens++;
                    _state = AState.UnarySubPart;
                    _sub1 = new Assignment(this, token.Line, token.Position);
                    return;
                case TokenType.Name:
                    _def1 = GetDefinition(token.TokenString);
                    if (_def1 == null) throw new InvalidSyntaxException($"Use of undefined variable {token.TokenString}", token.Line, token.Position);
                    if (_def1.GetValue() is MplBoolean)
                    {
                        _state = AState.Done;
                        return;
                    }
                    throw new InvalidSyntaxException($"{token.TokenString} is not boolean. Invalid type for negation", token.Line, token.Position);
                default:
                    throw new InvalidSyntaxException($"{token.TokenString} is not boolean.", token.Line, token.Position);
            }
        }

        private void EmptyState(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.Control:
                    switch (token.TokenString)
                    {
                        case "!":
                            _operator = Operator.Negation;
                            _state = AState.Unary;
                            return;
                        case "(":
                            _state = AState.Op1SubPart;
                            _sub1 = new Assignment(this, token.Line, token.Position);
                            _parens++;
                            return;
                        default:
                            throw new InvalidSyntaxException($"Invalid TokenString at start of expression {token.TokenString}", token.Line, token.Position);
                    }
                case TokenType.Name:
                    if (Keywords.Contains(token.TokenString))
                        throw new InvalidSyntaxException($"Undefined use of reserved keyword {token.TokenString}", token.Line, token.Position);
                    _def1 = GetDefinition(token.TokenString);
                    if (_def1 == null) 
                        throw new InvalidSyntaxException($"Reference to undefined variable {token.TokenString}", token.Line, token.Position);
                    _state = AState.Operanded;
                    return;
                case TokenType.Number:
                    _def1 = new Definition(this, int.Parse(token.TokenString), _line, _position);
                    _state = AState.Operanded;
                    return;
                case TokenType.String:
                    _def1 = new Definition(this, token.TokenString);
                    _state = AState.Operanded;
                    return;
            }
        }

        public override bool Exit()
        {
            switch (_state)
            {
                case AState.Done:
                case AState.Operanded:
                    return true;
                case AState.Op2SubPart:
                    return _sub2.Exit();
                case AState.Op1SubPart:
                case AState.UnarySubPart:
                    return _sub1.Exit();
                default:
                    return false;
            }
        }

        public override Definition GetDefinition(string name)
        {
            return _parent.GetDefinition(name);
        }

        public new Type GetType()
        {
            return _operator switch
            {
                Operator.Equality => Type.Bool,
                Operator.And => Type.Bool,
                Operator.Less => Type.Bool,
                Operator.Division => Type.Int,
                Operator.Subtraction => Type.Int,
                Operator.Multiplication => Type.Int,
                Operator.Addition => _def1?.GetType() ?? _sub1.GetType(),
                _ => Type.Bool
            };
        }
    }
}

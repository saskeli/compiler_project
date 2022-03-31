using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using mpl.domain;
using mpl.Exceptions;

namespace mpl
{
    public class StreamLexer
    {
        internal enum State
        {
            Empty,
            Control,
            Name,
            Literal,
            Real,
            LiteralString,
            Escaped,
            LineComment,
            BlockComment
        }

        private readonly StreamReader _stream;
        private readonly bool _verbose;
        private readonly int _debug;
        private readonly bool _multiError;
        private readonly TokenParser _tokenParser;
        private readonly List<Exception> _exceptions = new List<Exception>();
        private State _state = State.Empty;
        private int _line = 1;
        private int _pos;
        private bool _lastSlash;
        private bool _lastCurl;
        private bool _lastStar;

        public StreamLexer(StreamReader stream, bool verbose, int debug, bool multiError)
        {
            _stream = stream;
            _verbose = verbose;
            _debug = debug;
            _multiError = multiError;
            _tokenParser = new TokenParser(debug, verbose);
        }

        public StreamLexer(StreamReader stream, bool verbose, int debug, bool multiError, TokenParser parser)
        {
            _stream = stream;
            _verbose = verbose;
            _debug = debug;
            _multiError = multiError;
            _tokenParser = parser;
        }

        public Program Parse()
        {
            StringBuilder sb = new StringBuilder();
            if (_verbose || _debug > 0) Console.WriteLine("Reading input stream");
            while (!_stream.EndOfStream)
            {
                int c = _stream.Read();
                char cc = c >= 32 && c <= 126 ? (char)c : (char)0;
                if (_debug > 1) Console.WriteLine($"Read {c} from stream, \"{cc}\".");
                try
                {
                    sb = ProcessCharacter(c, cc, sb);
                }
                catch (Exception e)
                {
                    if (!_multiError) throw;
                    if (e is InvalidSyntaxException ||
                        e is UnexpectedCharacterException ||
                        e is UnsupportedCharacterException)
                    {
                        _exceptions.Add(e);
                        _state = State.Empty;
                        sb = new StringBuilder();
                        _lastSlash = _lastStar = _lastCurl = false;
                    }
                    else throw;
                }

            }
            if (_verbose || _debug > 0) Console.WriteLine("Reached end of stream");
            if (_state == State.Control && sb.Length > 0)
            {
                try
                {
                    SplitControls(sb.ToString());
                }
                catch (Exception e)
                {
                    if (!_multiError) throw;
                    if (e is InvalidSyntaxException ||
                        e is UnexpectedCharacterException ||
                        e is UnsupportedCharacterException)
                    {
                        _exceptions.Add(e);
                        _state = State.Empty;
                        sb = new StringBuilder();
                        _lastSlash = _lastStar = _lastCurl = false;
                    }
                    else throw;
                }

            }
            if (_multiError && _exceptions.Count > 0)
                throw new MultiException(_exceptions);
            return _tokenParser.GetProgram();
        }

        private StringBuilder ProcessCharacter(int c, char cc, StringBuilder sb)
        {
            if (c == 10)
            {
                if (_state == State.LineComment) _state = State.Empty;
                sb = _state switch
                {
                    State.Empty => StateEmpty(Ctype.Whitespace, cc, sb),
                    State.Literal => StateLiteral(Ctype.Whitespace, cc, sb),
                    State.Real => StateReal(Ctype.Whitespace, cc, sb),
                    State.Control => StateControl(Ctype.Whitespace, cc, sb),
                    State.Name => StateName(Ctype.Whitespace, cc, sb),
                    _ => sb
                };
                _line++;
                _pos = 0;
                return sb;
            }

            // Switch for states that should not be handled based on character type
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (_state)
            {
                case State.LineComment:
                    return sb;
                case State.BlockComment:
                    if (_lastStar && cc == '}') _state = State.Empty;
                    _lastStar = cc == '*';
                    _pos++;
                    return sb;
                case State.Escaped:
                    sb = AddToString(sb, cc);
                    _state = State.LiteralString;
                    _pos++;
                    return sb;
                case State.LiteralString:
                    _pos++;
                    switch (cc)
                    {
                        case '\\':
                            _state = State.Escaped;
                            break;
                        case '"':
                            _state = State.Empty;
                            _tokenParser.ParseToken(new Token(TokenType.String, _line, _pos, sb.ToString()));
                            sb = new StringBuilder();
                            break;
                        default:
                            sb.Append(cc);
                            break;
                    }

                    return sb;
                default:
                    if (cc == '"')
                    {
                        _pos++;
                        sb = StartString(sb);
                        return sb;
                    }

                    break;
            }

            Ctype ctype = CharacterType(c);
            sb = _state switch
            {
                State.Empty => StateEmpty(ctype, cc, sb),
                State.Literal => StateLiteral(ctype, cc, sb),
                State.Control => StateControl(ctype, cc, sb),
                State.Name => StateName(ctype, cc, sb),
                _ => sb
            };
            _pos++;
            return sb;
        }

        private StringBuilder StartString(StringBuilder sb)
        {
            if (sb.Length > 0 && _state != State.Empty)
            {
                switch (_state)
                {
                    case State.Control:
                        SplitControls(sb.ToString());
                        break;
                    case State.Literal:
                        _tokenParser.ParseToken(new Token(TokenType.Number, _line, _pos, sb.ToString()));
                        break;
                    case State.Real:
                        _tokenParser.ParseToken(new Token(TokenType.Real, _line, _pos, sb.ToString()));
                        break;
                    case State.Name:
                        _tokenParser.ParseToken(new Token(TokenType.Name, _line, _pos, sb.ToString()));
                        break;
                }


            }
            _state = State.LiteralString;
            return new StringBuilder();
        }

        private StringBuilder StateEmpty(Ctype ctype, in char cc, StringBuilder sb)
        {
            switch (ctype)
            {
                case Ctype.Char:
                    if (cc == '_') throw new UnexpectedCharacterException("\"_\" is an invalid start for a name", _line, _pos);
                    _state = State.Name;
                    sb.Append(cc);
                    break;
                case Ctype.Control:
                    _state = State.Control;
                    if (cc == '/') _lastSlash = true;
                    if (cc == '{') _lastCurl = true;
                    sb.Append(cc);
                    break;
                case Ctype.Num:
                    _state = State.Literal;
                    sb.Append(cc);
                    break;
                case Ctype.Whitespace:
                    break;
            }
            return sb;
        }

        private StringBuilder StateLiteral(Ctype ctype, in char cc, StringBuilder sb)
        {
            switch (ctype)
            {
                case Ctype.Char:
                    throw new UnexpectedCharacterException($"{cc} is not a valid character for a numeric literal", _line, _pos);
                case Ctype.Num:
                    sb.Append(cc);
                    break;
                case Ctype.Control:
                    if (_stream.Peek() == '.' && cc == '.')
                    {
                        _stream.Read();
                        _pos++;
                        sb.Append(cc);
                        _state = State.Real;
                        break;
                    }
                    _tokenParser.ParseToken(new Token(TokenType.Number, _line, _pos, sb.ToString()));
                    if (cc == '/') _lastSlash = true;
                    if (cc == '{') _lastCurl = true;
                    sb = new StringBuilder();
                    _state = State.Control;
                    sb.Append(cc);
                    break;
                case Ctype.Whitespace:
                    _tokenParser.ParseToken(new Token(TokenType.Number, _line, _pos, sb.ToString()));
                    sb = new StringBuilder();
                    _state = State.Empty;
                    break;
            }
            return sb;
        }

        private StringBuilder StateReal(Ctype ctype, in char cc, StringBuilder sb)
        {
            switch (ctype)
            {
                case Ctype.Char:
                    if (cc != 'e') throw new UnexpectedCharacterException($"\"{cc}\" is an invalid character for a literal real.", _line, _pos);
                    sb.Append(cc);
                    break;
                case Ctype.Num:
                    sb.Append(cc);
                    break;
                case Ctype.Control:
                    if (sb[sb.Length - 1] == 'e' && (cc == '+' || cc == '-'))
                    {
                        sb.Append(cc);
                    }
                    _tokenParser.ParseToken(new Token(TokenType.Real, _line, _pos, sb.ToString()));
                    if (cc == '/') _lastSlash = true;
                    if (cc == '{') _lastCurl = true;
                    sb = new StringBuilder();
                    _state = State.Control;
                    sb.Append(cc);
                    break;
                case Ctype.Whitespace:
                    _tokenParser.ParseToken(new Token(TokenType.Real, _line, _pos, sb.ToString()));
                    sb = new StringBuilder();
                    _state = State.Empty;
                    sb.Append(cc);
                    break;
            }
            return sb;
        }

        private StringBuilder StateControl(Ctype ctype, in char cc, StringBuilder sb)
        {
            switch (ctype)
            {
                case Ctype.Char:
                    if (cc == '_') throw new UnexpectedCharacterException("\"_\" is an invalid start for a name", _line, _pos);
                    SplitControls(sb.ToString());
                    sb = new StringBuilder();
                    _state = State.Name;
                    sb.Append(cc);
                    break;
                case Ctype.Control:
                    switch (cc)
                    {
                        case '/' when _lastSlash:
                            {
                                _state = State.LineComment;
                                if (sb.Length > 1)
                                    SplitControls(sb.ToString(0, sb.Length - 1));
                                sb = new StringBuilder();
                                break;
                            }
                        case '*' when _lastCurl:
                            {
                                _state = State.BlockComment;
                                if (sb.Length > 1)
                                    SplitControls(sb.ToString(0, sb.Length - 1));
                                sb = new StringBuilder();
                                break;
                            }
                        default:
                            {
                                _lastSlash = cc == '/';
                                _lastCurl = cc == '{';
                                sb.Append(cc);
                                break;
                            }
                    }
                    break;
                case Ctype.Num:
                    SplitControls(sb.ToString());
                    sb = new StringBuilder();
                    _state = State.Literal;
                    sb.Append(cc);
                    break;
                case Ctype.Whitespace:
                    SplitControls(sb.ToString());
                    _state = State.Empty;
                    sb = new StringBuilder();
                    break;
            }
            return sb;
        }

        private void SplitControls(string token)
        {
            TokenType tt = TokenType.Control;
            int pos = _pos - token.Length + 1;
            char? prev = null;
            foreach (char c in token)
            {
                switch (c)
                {
                    case '.':
                        if (prev == '.')
                        {
                            _tokenParser.ParseToken(new Token(tt, _line, _pos, ".."));
                            prev = null;
                            break;
                        }
                        goto default;
                    case '=':
                        if (prev == ':')
                        {
                            _tokenParser.ParseToken(new Token(tt, _line, _pos, ":="));
                            prev = null;
                            break;
                        }
                        else if (prev == '<')
                        {
                            _tokenParser.ParseToken(new Token(tt, _line, _pos, "<="));
                            prev = null;
                            break;
                        }
                        else if (prev == '>')
                        {
                            _tokenParser.ParseToken(new Token(tt, _line, _pos, ">="));
                            prev = null;
                            break;
                        }
                        goto default;
                    case '>':
                        if (prev == '<')
                        {
                            _tokenParser.ParseToken(new Token(tt, _line, _pos, "<>"));
                            prev = null;
                            break;
                        }
                        goto default;
                    default:
                        if (prev != null) _tokenParser.ParseToken(new Token(tt, _line, pos, (char)prev));
                        prev = c;
                        break;
                }
            }
            if (prev != null) _tokenParser.ParseToken(new Token(tt, _line, _pos, (char)prev));
        }

        private StringBuilder StateName(Ctype ctype, char cc, StringBuilder sb)
        {
            switch (ctype)
            {
                case Ctype.Char:
                    sb.Append(cc);
                    break;
                case Ctype.Num:
                    sb.Append(cc);
                    break;
                case Ctype.Control:
                    _tokenParser.ParseToken(new Token(TokenType.Name, _line, _pos, sb.ToString().ToLower()));
                    if (cc == '/') _lastSlash = true;
                    if (cc == '{') _lastCurl = true;
                    sb = new StringBuilder();
                    sb.Append(cc);
                    _state = State.Control;
                    break;
                case Ctype.Whitespace:
                    _tokenParser.ParseToken(new Token(TokenType.Name, _line, _pos, sb.ToString().ToLower()));
                    sb = new StringBuilder();
                    _state = State.Empty;
                    break;
            }

            return sb;
        }

        private StringBuilder AddToString(StringBuilder sb, in char cc)
        {
            switch (cc)
            {
                case 'n':
                    return sb.Append("\n");
                case 't':
                    return sb.Append("\t");
                case '"':
                    return sb.Append("\"");
                case '\\':
                    return sb.Append("\\");
                case 'r':
                    return sb.Append("\r");
                default:
                    {
                        string mess = $"Undefined escape sequence at line {_line}, position {_pos}.";
                        throw new UnexpectedCharacterException(mess, _line, _pos);
                    }
            }
        }

        private Ctype CharacterType(int i)
        {
            if (i == 32 || i == 10 || i == 13) return Ctype.Whitespace;
            if (i >= 65 && i <= 90 || i >= 97 && i <= 122 || i == 95)
            {
                return Ctype.Char;
            }
            if (i >= 48 && i <= 57) return Ctype.Num;
            if (i == 33 || i == 38 || i == 40 || i == 41 || i == 42
                || i == 43 || i == 45 || i == 46 || i == 47 || i == 58 || i == 59
                || i == 60 || i == 61)
            {
                return Ctype.Control;
            }
            string mess = $"Unsupported character encountered at line {_line}, position {_pos + 1}.";
            throw new UnsupportedCharacterException(mess, _line, _pos + 1);
        }
    }
}
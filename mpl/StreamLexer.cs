using System;
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
            LiteralInt,
            LiteralString,
            Escaped,
            LineComment,
            BlockComment
        }

        private readonly StreamReader _stream;
        private readonly bool _verbose;
        private readonly int _debug;
        private readonly TokenParser _tokenParser;
        private State _state = State.Empty;
        private int _line = 1;
        private int _pos;
        private bool _lastSlash;

        public StreamLexer(StreamReader stream, bool verbose, int debug)
        {
            _stream = stream;
            _verbose = verbose;
            _debug = debug;
            _tokenParser = new TokenParser(debug, verbose);
        }

        public StreamLexer(StreamReader stream, bool verbose, int debug, TokenParser parser)
        {
            _stream = stream;
            _verbose = verbose;
            _debug = debug;
            _tokenParser = parser;
        }

        public Program Parse()
        {
            bool lastStar = false;
            StringBuilder sb = new StringBuilder();
            if (_verbose || _debug > 0) Console.WriteLine("Reading input stream");
            while (!_stream.EndOfStream)
            {
                int c = _stream.Read();
                char cc = (c >= 32 && c <= 126) ? (char) c : (char) 0;
                if (_debug > 1) Console.WriteLine($"Read {c} from stream, \"{cc}\".");
                if (c == 10)
                {
                    if (_state == State.LineComment) _state = State.Empty;
                    sb = _state switch
                    {
                        State.Empty => StateEmpty(Ctype.Whitespace, cc, sb),
                        State.LiteralInt => StateLiteralInt(Ctype.Whitespace, cc, sb),
                        State.Control => StateControl(Ctype.Whitespace, cc, sb),
                        State.Name => StateName(Ctype.Whitespace, cc, sb),
                        _ => sb
                    };
                    _line++;
                    _pos = 0;
                    continue;
                }

                // Switch for states that should not be handled based on character type
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (_state)
                {
                    case State.LineComment: 
                        continue;
                    case State.BlockComment:
                        if (lastStar && cc == '/') _state = State.Empty;
                        lastStar = cc == '*';
                        _pos++;
                        continue;
                    case State.Escaped:
                        sb = AddToString(sb, cc);
                        _state = State.LiteralString;
                        _pos++;
                        continue;
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
                        continue;
                    default:
                        if (cc == '"')
                        {
                            _pos++;
                            sb = StartString(sb);
                            continue;
                        }
                        break;
                }

                Ctype ctype = CharacterType(c);
                sb = _state switch
                {
                    State.Empty => StateEmpty(ctype, cc, sb),
                    State.LiteralInt => StateLiteralInt(ctype, cc, sb),
                    State.Control => StateControl(ctype, cc, sb),
                    State.Name => StateName(ctype, cc, sb),
                    _ => sb
                };
                _pos++;
            }
            if (_verbose || _debug > 0) Console.WriteLine("Reached end of stream");
            if (_state == State.Control && sb.Length > 0)
            {
                SplitControls(sb.ToString());
            }
            return _tokenParser.GetProgram();
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
                    case State.LiteralInt:
                        _tokenParser.ParseToken(new Token(TokenType.Number, _line, _pos, sb.ToString()));
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
                    if (cc =='/')
                        _lastSlash = true;
                    sb.Append(cc);
                    break;
                case Ctype.Num:
                    _state = State.LiteralInt;
                    sb.Append(cc);
                    break;
                case Ctype.Whitespace:
                    break;
            }
            return sb;
        }

        private StringBuilder StateLiteralInt(Ctype ctype, in char cc, StringBuilder sb)
        {
            switch (ctype)
            {
                case Ctype.Char:
                    throw new UnexpectedCharacterException($"{cc} is not a valid character for an integer literal", _line, _pos);
                case Ctype.Num:
                    sb.Append(cc);
                    break;
                case Ctype.Control:
                    _tokenParser.ParseToken(new Token(TokenType.Number, _line, _pos, sb.ToString()));
                    if (cc == '/') _lastSlash = true;
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
                        case '*' when _lastSlash:
                        {
                            _state = State.BlockComment;
                            if (sb.Length > 1)
                                SplitControls(sb.ToString(0, sb.Length - 1));
                            sb = new StringBuilder();
                            break;
                        }
                        default:
                        {
                            if (cc == '/') _lastSlash = true;
                            sb.Append(cc);
                            break;
                        }
                    }
                    break;
                case Ctype.Num:
                    SplitControls(sb.ToString());
                    sb = new StringBuilder();
                    _state = State.LiteralInt;
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
                if (prev == '.' && c != '.') throw new UnexpectedCharacterException("Expected '.' to signify range", _line, pos);
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
                    _tokenParser.ParseToken(new Token(TokenType.Name, _line, _pos, sb.ToString()));
                    if (cc == '/') _lastSlash = true;
                    sb = new StringBuilder();
                    sb.Append(cc);
                    _state = State.Control;
                    break;
                case Ctype.Whitespace:
                    _tokenParser.ParseToken(new Token(TokenType.Name, _line, _pos, sb.ToString()));
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
            string mess = $"Unsupported character encountered at line {_line}, position {_pos}.";
            throw new UnsupportedCharacterException(mess, _line, _pos);
        }
    }
}
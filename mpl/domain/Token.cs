using System;

namespace mpl.domain
{
    public struct Token
    {

        public readonly TokenType TokenType;
        public readonly int Line;
        public readonly int Position;
        public readonly string TokenString;

        public Token(TokenType tokenType, int line, int position, string tokenString)
        {
            TokenType = tokenType;
            Line = line;
            Position = position;
            TokenString = tokenString;
        }

        public Token(TokenType tokenType, int line, int position, char token)
        {
            TokenType = tokenType;
            Line = line;
            Position = position;
            TokenString = token.ToString();
        }

        public bool Equals(Token other)
        {
            return TokenType == other.TokenType && Line == other.Line && Position == other.Position && TokenString == other.TokenString;
        }

        public override bool Equals(object obj)
        {
            return obj is Token other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)TokenType, Line, Position, TokenString);
        }

        public override string ToString()
        {
            return $"Token: {TokenType}, {Line}, {Position}, \"{TokenString}\"";
        }

    }
}

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
            this.TokenType = tokenType;
            this.Line = line;
            this.Position = position;
            this.TokenString = tokenString;
        }

        public Token(TokenType tokenType, int line, int position, char token)
        {
            this.TokenType = tokenType;
            this.Line = line;
            this.Position = position;
            this.TokenString = token.ToString();
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

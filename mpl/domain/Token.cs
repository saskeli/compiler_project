using System;
using System.Collections.Generic;
using System.Text;

namespace mpl.domain
{
    public struct Token
    {

        public readonly TokenType tokenType;
        public readonly int line;
        public readonly int position;
        public readonly string token;

        public Token(TokenType tokenType, int line, int position, string token)
        {
            this.tokenType = tokenType;
            this.line = line;
            this.position = position;
            this.token = token;
        }

        public Token(TokenType tokenType, int line, int position, char token)
        {
            this.tokenType = tokenType;
            this.line = line;
            this.position = position;
            this.token = token.ToString();
        }

        public bool Equals(Token other)
        {
            return tokenType == other.tokenType && line == other.line && position == other.position && token == other.token;
        }

        public override bool Equals(object obj)
        {
            return obj is Token other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)tokenType, line, position, token);
        }

        public override string ToString()
        {
            return $"Token: {tokenType}, {line}, {position}, \"{token}\"";
        }

    }
}

using System;

namespace ExpressionParser.Parsers
{
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Type}, {Value}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Token other))
                return base.Equals(obj);

            return other.Type == Type && other.Value == Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }
    }

    public enum TokenType
    {
        NonSignificant,
        ParenthesisOpen,
        ParenthesisClose,
        Operator,
        Delimiter,
        Identifier,
        Constant,
        EndOfString,
    }
}

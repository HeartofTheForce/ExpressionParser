using System;

namespace ExpressionParser.Parsers
{
    public class MissingTokenException : Exception
    {
        public TokenType Type { get; }

        public MissingTokenException(TokenType type) : base($"Missing Token '{type}'")
        {
            Type = type;
        }
    }

    public class ExpressionTermException : Exception
    {
        public TokenType Type { get; set; }

        public ExpressionTermException(TokenType type) : base($"Invalid Expression Term '{type}'")
        {
            Type = type;
        }
    }

    public class UnexpectedTokenException : Exception
    {
        public TokenType Type { get; set; }

        public UnexpectedTokenException(TokenType type) : base($"Unexpected Token '{type}'")
        {
            Type = type;
        }
    }

    public class ExpressionReductionException : Exception
    {
        public int RemainingValues { get; }

        public ExpressionReductionException(int remainingValues) : base($"Expression incorrectly reduced to {remainingValues} values")
        {
            RemainingValues = remainingValues;
        }
    }
}

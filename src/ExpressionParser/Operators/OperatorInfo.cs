using System;

namespace ExpressionParser.Operators
{
    public class OperatorInfo
    {
        public string Keyword { get; }
        public int Precedence { get; }
        public Associativity Associativity { get; }
        public bool HasLeftArguments { get; }
        public bool HasRightArguments { get; }

        public OperatorInfo(
            string keyword,
            int precedence,
            Associativity associativity,
            bool hasLeftArguments,
            bool hasRightArguments)
        {
            Keyword = keyword;
            Precedence = precedence;
            Associativity = associativity;
            HasLeftArguments = hasLeftArguments;
            HasRightArguments = hasRightArguments;
        }

        public bool IsPrefix() => !HasLeftArguments && HasRightArguments;
        public bool IsPostfix() => HasLeftArguments && !HasRightArguments;
        public bool IsInfix() => HasLeftArguments && HasRightArguments;

        public static bool IsEvaluatedBefore(OperatorInfo left, OperatorInfo right)
        {
            if (left.Precedence == right.Precedence)
            {
                if (left.Associativity == right.Associativity)
                {
                    switch (left.Associativity)
                    {
                        case Associativity.Left: return true;
                        case Associativity.Right: return false;
                    }
                }
                else
                {
                    throw new Exception("Operators with same Precedence must have same Associativity");
                }
            }

            return left.Precedence < right.Precedence;
        }
    }

    public enum Associativity
    {
        Left,
        Right
    }
}

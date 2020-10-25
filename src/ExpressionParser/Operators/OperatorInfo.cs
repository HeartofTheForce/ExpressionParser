using System;

namespace ExpressionParser.Operators
{
    public abstract class OperatorInfo
    {
        public string Input { get; }

        public string Output { get; }
        public int Precedence { get; }
        public Associativity Associativity { get; }
        public int PreArgCount { get; }
        public int PostArgCount { get; }
        public int ParameterCount => PreArgCount + PostArgCount;

        protected OperatorInfo(string input, string output, int precedence, Associativity associativity, int preArgCount, int postArgCount)
        {
            Input = input;
            Output = output;
            Precedence = precedence;
            Associativity = associativity;
            PreArgCount = preArgCount;
            PostArgCount = postArgCount;
        }

        public static bool IsLowerPrecedence(OperatorInfo current, OperatorInfo target)
        {
            if (current.Precedence == target.Precedence)
            {
                if (current.Associativity == target.Associativity)
                {
                    switch (current.Associativity)
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

            return current.Precedence < target.Precedence;
        }
    }

    public abstract class OperatorInfo<T> : OperatorInfo
    {
        protected OperatorInfo(string input, string output, int precedence, Associativity associativity, int preArgCount, int postArgCount)
        : base(input, output, precedence, associativity, preArgCount, postArgCount)
        {
        }

        public abstract T Reduce(T[] args);
    }

    public enum Associativity
    {
        Left,
        Right
    }
}

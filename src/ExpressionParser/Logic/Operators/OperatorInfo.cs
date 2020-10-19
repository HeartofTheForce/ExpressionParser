namespace ExpressionParser.Logic.Operators
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

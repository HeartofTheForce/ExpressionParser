namespace ExpressionParser.Logic.Operators
{
    public interface IOperatorInfo
    {
        string Input { get; }
        string Output { get; }
        int Precedence { get; }
        Associativity Associativity { get; }
    }

    public interface IOperatorInfo<T> : IOperatorInfo
    {
        int ParameterCount { get; }
        T Reduce(T[] args);
    }

    public enum Associativity
    {
        Left,
        Right
    }
}

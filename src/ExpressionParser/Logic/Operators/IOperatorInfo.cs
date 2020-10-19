namespace ExpressionParser.Logic.Operators
{
    public interface IOperatorInfo
    {
        string Input { get; }
        string Output { get; }
        int Precedence { get; }
        Associativity Associativity { get; }
        int ParameterCount { get; }
    }

    public interface IOperatorInfo<T> : IOperatorInfo
    {
        T Reduce(T[] args);
    }

    public enum Associativity
    {
        Left,
        Right
    }
}

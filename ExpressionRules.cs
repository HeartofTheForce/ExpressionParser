using System;

namespace ExpressionParser
{
    public static class ExpressionRules
    {
        public static readonly IOperatorInfo<int>[] BinaryOperatorMap = new BinaryOperator<int>[]
        {
            new BinaryOperator<int>() { Input = "+", Output = "+", Precedence = 1, Associativity = Associativity.Left, Execute = (args) => args[0] + args[1] },
            new BinaryOperator<int>() { Input = "-", Output = "-", Precedence = 1, Associativity = Associativity.Left, Execute = (args) => args[0] - args[1] },
            new BinaryOperator<int>() { Input = "*", Output = "*", Precedence = 2, Associativity = Associativity.Left, Execute = (args) => args[0] * args[1] },
            new BinaryOperator<int>() { Input = "/", Output = "/", Precedence = 2, Associativity = Associativity.Left, Execute = (args) => args[0] / args[1] },
            new BinaryOperator<int>() { Input = "|", Output = "|", Precedence = 3, Associativity = Associativity.Right, Execute = (args) => args[0] | args[1] },
            new BinaryOperator<int>() { Input = "^", Output = "^", Precedence = 4, Associativity = Associativity.Right, Execute = (args) => args[0] ^ args[1] },
            new BinaryOperator<int>() { Input = "&", Output = "&", Precedence = 4, Associativity = Associativity.Right, Execute = (args) => args[0] & args[1] },
        };

        public static readonly IOperatorInfo<int>[] FunctionOperatorMap = new FunctionOperator<int>[]
        {
            new FunctionOperator<int>() { Input = "+", Output = "u+", ParameterCount = 1, Execute = (args) => +args[0] },
            new FunctionOperator<int>() { Input = "-", Output = "u-", ParameterCount = 1, Execute = (args) => -args[0] },
            new FunctionOperator<int>() { Input = "~", Output = "~", ParameterCount = 1, Execute = (args) => ~args[0] },
            new FunctionOperator<int>() { Input = "max", Output = "max", ParameterCount = 2, Execute = (args) => Math.Max(args[0], args[1]) },
        };

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
            Func<T[], T> Execute { get; }
        }

        public class BinaryOperator<T> : IOperatorInfo<T>
        {
            public string Input { get; set; }
            public string Output { get; set; }
            public int Precedence { get; set; }
            public Associativity Associativity { get; set; }
            public int ParameterCount => 2;
            public Func<T[], T> Execute { get; set; }
        }

        public class FunctionOperator<T> : IOperatorInfo<T>
        {
            public string Input { get; set; }
            public string Output { get; set; }
            public int Precedence => int.MaxValue;
            public Associativity Associativity => Associativity.Right;
            public int ParameterCount { get; set; }
            public Func<T[], T> Execute { get; set; }
        }


        public enum Associativity
        {
            Left,
            Right
        }
    }
}

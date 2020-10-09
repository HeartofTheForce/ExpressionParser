using System;
using System.Linq.Expressions;

namespace ExpressionParser.Parser
{
    public static class Rules
    {
        public static readonly IOperatorInfo<Expression>[] BinaryOperatorMap = new BinaryOperator[]
        {
            new BinaryOperator() { Input = "+", Output = "+", Precedence = 1, Associativity = Associativity.Left, Execute = (args) => Expression.Add(args[0], args[1]) },
            new BinaryOperator() { Input = "-", Output = "-", Precedence = 1, Associativity = Associativity.Left, Execute = (args) => Expression.Subtract(args[0], args[1]) },
            new BinaryOperator() { Input = "*", Output = "*", Precedence = 2, Associativity = Associativity.Left, Execute = (args) => Expression.Multiply(args[0], args[1]) },
            new BinaryOperator() { Input = "/", Output = "/", Precedence = 2, Associativity = Associativity.Left, Execute = (args) => Expression.Divide(args[0], args[1]) },
            new BinaryOperator() { Input = "&", Output = "&", Precedence = 4, Associativity = Associativity.Right, Execute = (args) => Expression.And(args[0], args[1]) },
            new BinaryOperator() { Input = "^", Output = "^", Precedence = 4, Associativity = Associativity.Right, Execute = (args) => Expression.ExclusiveOr(args[0], args[1]) },
            new BinaryOperator() { Input = "|", Output = "|", Precedence = 3, Associativity = Associativity.Right, Execute = (args) => Expression.Or(args[0], args[1]) },
        };

        public static readonly IOperatorInfo<Expression>[] FunctionOperatorMap = new FunctionOperator[]
        {
            new FunctionOperator() { Input = "+", Output = "u+", ParameterCount = 1, Execute = (args) => args[0] },
            new FunctionOperator() { Input = "-", Output = "u-", ParameterCount = 1, Execute = (args) => Expression.Negate(args[0]) },
            new FunctionOperator() { Input = "~", Output = "~", ParameterCount = 1, Execute = (args) => Expression.Not(args[0]) },
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

        public class BinaryOperator : IOperatorInfo<Expression>
        {
            public string Input { get; set; }
            public string Output { get; set; }
            public int Precedence { get; set; }
            public Associativity Associativity { get; set; }
            public int ParameterCount => 2;
            public Func<Expression[], Expression> Execute { get; set; }
        }

        public class FunctionOperator : IOperatorInfo<Expression>
        {
            public string Input { get; set; }
            public string Output { get; set; }
            public int Precedence => int.MaxValue;
            public Associativity Associativity => Associativity.Right;
            public int ParameterCount { get; set; }
            public Func<Expression[], Expression> Execute { get; set; }
        }


        public enum Associativity
        {
            Left,
            Right
        }
    }
}
